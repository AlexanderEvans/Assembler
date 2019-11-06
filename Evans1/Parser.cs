using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Evans1
{
    public static class Parser
    {
        public enum OperandType { NEW_STYLE_LITERAL, OLD_STYLE_LITERAL, DISPLACEMENT_OR_ADDRESS}
        public static OperandType guessOperandType(string operand)
        {
            Regex isNewStyleLiteral = new Regex(@"^[\t ]*=[^;]*'.*$");
            Regex isOldStyleLiteral = new Regex(@"^[^;]*'.*$");
            if (isNewStyleLiteral.IsMatch(operand))
                return OperandType.NEW_STYLE_LITERAL;
            else if (isOldStyleLiteral.IsMatch(operand))
                return OperandType.OLD_STYLE_LITERAL;
            else
                return OperandType.DISPLACEMENT_OR_ADDRESS;

        }

        public static bool ParseNewStyleLiteral(string operand, out LiteralTable.LiteralValue literalValue, out string comment, string callingModule = "")
        {
            operand = operand.Trim();
            callingModule = callingModule == "" ? "Parse New Style Literal" : callingModule;
            literalValue = new LiteralTable.LiteralValue();
            comment = "";
            if (operand[0] != '=')
            {
                Chronicler.LogError("New style literals must begin with '='", callingModule);
                return false;
            }
            operand = operand.Substring(1, operand.Length - 1).Trim();
            bool rtnVal = ParseOldStyleLiteral(operand, out literalValue, out comment,  callingModule);
            literalValue.isOldStyleLiteral = false;
            return rtnVal;
            
        }

        public static bool ParseOldStyleLiteral(string operand, out LiteralTable.LiteralValue literalValue, out string comment, string callingModule="")
        {
            Regex literalRegex = new Regex(@"(^[\t ]*(?<literalCharID>[Cc])[\t ]*(?<literalValueString>'.*')[\t ]*(?<comment>;.*){0,1}$)|(^[\t ]*(?<literalCharIDx>[Xx])[\t ]*(?<literalValueStringx>'[a-fA-F0-9]*')[\t ]*(?<commentx>;.*){0,1}$)");
            Match match = literalRegex.Match(operand);
            literalValue = new LiteralTable.LiteralValue();
            comment = "";

            if (match.Success)//if we succeed, format it
            {
                literalValue.isOldStyleLiteral = true;
                if (match.Groups["literalCharID"].Value!="")
                {
                    literalValue.label = match.Groups["literalCharID"].Value + match.Groups["literalValueString"].Value;
                    comment = match.Groups["comment"].Value;

                    StringBuilder sb = new StringBuilder(literalValue.label);
                    sb[0] = 'C';
                    literalValue.label = sb.ToString();

                    char[] tmpArr = literalValue.label.Substring(2, literalValue.label.Length - 3).ToCharArray();
                    literalValue.hexValue = Globals.CharArrToHexStr(tmpArr);
                    literalValue.dataType = LiteralTable.LiteralValue.DataType.CHAR;
                }
                else
                {
                    literalValue.label = match.Groups["literalCharIDx"].Value + match.Groups["literalValueStringx"].Value;
                    comment = match.Groups["commentx"].Value;

                    literalValue.label = literalValue.label.ToUpper();
                    literalValue.hexValue = literalValue.label.Substring(2, literalValue.label.Length - 3);
                    literalValue.hexValue = (literalValue.hexValue.Length % 2 == 1 ? "0" : "") + literalValue.hexValue;
                    literalValue.dataType = LiteralTable.LiteralValue.DataType.HEX;
                }

                return true;
            }

            //if we fail try to figure out how and provide detailed feedback

            StringBuilder operandMutaion = new StringBuilder(operand.Trim());
            callingModule = callingModule == "" ? "Parse Old Style Literal" : callingModule;
            if (operandMutaion.Length == 0)
                Chronicler.LogError("No literal value was given", callingModule);
            operandMutaion[0] = char.ToUpper(operandMutaion[0]);
            bool isChar = false;
            if (operandMutaion[0] != 'C' && operandMutaion[0] == 'X')
            {
                Chronicler.LogError("The charachter '" + operandMutaion[0] + "' is not a valid literal identifier.  Valid identifiers are 'XxCc'.", callingModule);
                return false;
            }
            else if (operandMutaion[0] == 'C')
                isChar = true;

            //remove whitespace
            while(operandMutaion.Length>2 && char.IsWhiteSpace(operandMutaion[1]))
            {
                operandMutaion = operandMutaion.Remove(1, 1);
            }

            if(operandMutaion[1]!='\'')
            {
                Chronicler.LogError("Was expecting ''', but found '" + operandMutaion[1] + "'", callingModule);
                return false;
            }

            int lastApos = -1;
            for(int i = 2; i< operandMutaion.Length;i++)
            {
                if (operandMutaion[i] == '\'')
                    lastApos = i;
            }

            if(lastApos==-1)
            {
                Chronicler.LogError("Was unable to find closing '''", callingModule);
                return false;
            }
            else
            {
                string subStr = operandMutaion.ToString().Substring(2, lastApos-2);
                if(!isChar)
                {
                    char? errorChar;
                    if (!validateHex(subStr, out errorChar) && errorChar!='\'')
                    {
                        Chronicler.LogError("Invalid Hex char detected in hex literal: "+ errorChar.Value, callingModule);
                        return false;
                    }
                    else
                    {
                        lastApos = -1;
                        for (int i = 2; i < operandMutaion.Length && lastApos==-1; i++)
                        {
                            if (operandMutaion[i] == '\'')
                                lastApos = i;
                        }
                    }
                }

                int lastComma = operandMutaion.Length;
                for (int i = lastApos; i < operandMutaion.Length && lastComma == operandMutaion.Length; i++)
                {
                    if (operandMutaion[i] == ';')
                        lastComma = i;
                }

                subStr = operandMutaion.ToString().Substring(lastApos, lastComma - lastApos);
                Chronicler.LogError("Parsing literal failed; was their garbage after the literal before the comment?  Found:(" + subStr + ")", callingModule);
                return false;
            }
        }

        //************************************************************************
        //***  FUNCTION validateHex 
        //*** ********************************************************************
        //***  DESCRIPTION  :  Ensures all chars of a string are hex digists
        //***  INPUT ARGS   :  string charStr
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool isHex
        //************************************************************************
        public static bool validateHex(string charStr, out char? invalidChar)
        {
            invalidChar = null;
            charStr = charStr.ToUpper();
            bool isHex = true;
            foreach (char c in charStr.TakeWhile(c => { return isHex; }))
            {
                if (!(char.IsDigit(c) || (((int)c) >= 'A' && ((int)c) <= 'F')))
                {
                    isHex = false;
                    invalidChar = c;
                }
            }
            return isHex;
        }
    }
}
