using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Evans1
{
    //*********************************************************************
    //*** NAME : Alex Evans
    //*** CLASS : CSc 354 Intro to systems
    //*** ASSIGNMENT : 2
    //*** DUE DATE : 10/9/2019
    //*** INSTRUCTOR : GAMRADT 
    //*********************************************************************
    //*** DESCRIPTION :   solves an expresion and gets a symbol or sets a 
    //***                   literal.
    //*********************************************************************
    class ExpresionHandler
    {

        ////*******************************************************************************************
        ////***  FUNCTION ParseExpresionFile 
        ////*** ***************************************************************************************
        ////***  DESCRIPTION  :  Parses the Expresion File
        ////***  INPUT ARGS   :  SymbolTable symbolTable, LiteralTable literalTable, string filePath
        ////***  OUTPUT ARGS :  N/A
        ////***  IN/OUT ARGS   :  N/A  
        ////***  RETURN :  N/A
        ////*******************************************************************************************
        //public static void ParseExpresionFile(SymbolTable symbolTable, LiteralTable literalTable, string filePath)
        //{
        //    try
        //    {
        //        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
        //        {

        //            try
        //            {
        //                using (StreamReader streamReader = new StreamReader(fileStream))
        //                {
        //                    int LineNumber = 1;
        //                    Chronicler.WriteLine("EXPRESION\tVALUE\tRELOCATABLE\tN-Bit\tI-Bit\tX-Bit");
        //                    while (!streamReader.EndOfStream)
        //                    {
        //                        //get the line and trim whitespace
        //                        string currentLine = streamReader.ReadLine().Trim();
        //                        if (ResolveF3F4Expresion(symbolTable, literalTable, currentLine, currentLine, out Globals.Symbol? symbol))
        //                        {
        //                            if (symbol != null)
        //                            {
        //                                StringBuilder sb = new StringBuilder("");
        //                                for (int x = 0; x < (16 - currentLine.Length); x++)
        //                                {
        //                                    sb.Append(" ");
        //                                }
        //                                Chronicler.Write(currentLine + sb.ToString());
        //                                Chronicler.Write(symbol.Value.value.ToString());
        //                                Chronicler.Write("\t" + ((symbol.Value.RFlag) ? "RELOCATABLE" : "ABSOLUTE"));
        //                                Chronicler.NewLine();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Chronicler.LogError("Line: \"" + LineNumber + "\" Skipping: \"" + currentLine + "\"", "Expresion File Parsing");
        //                        }
        //                        LineNumber++;
        //                    }
        //                    Chronicler.LogInfo("DONE loading symbols!");
        //                }
        //            }
        //            catch (IOException e)
        //            {
        //                Chronicler.WriteLine(e.Message);
        //                Chronicler.LogError("failed to open File: " + filePath);
        //            }
        //        }
        //    }
        //    catch (IOException e)
        //    {
        //        Chronicler.WriteLine(e.Message);
        //        Chronicler.LogError("failed to open File: " + filePath);
        //    }
        //}

        //*******************************************************************************************
        //***  FUNCTION ResolveF3F4Expresion 
        //*** ***************************************************************************************
        //***  DESCRIPTION  :  Resolves an expresion and adds a literal or gets a symbol
        //***  INPUT ARGS   :  SymbolTable symbolTable, LiteralTable literalTable,
        //***                   string expresion, string currentLine
        //***  OUTPUT ARGS :  out Globals.Symbol? symbol
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool rtnVal
        //*******************************************************************************************
        public static bool ResolveF3F4Expresion(Globals.DataStructures dataStructures, string expresionString, string currentLine, out Globals.ExpresionData expresionData)
        {
            expresionData = new Globals.ExpresionData();
            bool rtnVal = true;
            Regex isLiteral = new Regex(@"^\h*={0,1}\h*[CcXx]\h*'.*$");
            Regex testXValue = new Regex(@"(^[^=][^,;]*(?<x>,x|X){0,1}.*$)|(^\h*=.*$)");
            bool hasXValue = testXValue.Match(expresionString).Groups["x"].Value != "";
            expresionString = expresionString.Trim();
            if(isLiteral.Match(expresionString).Success)
            {
                string line = expresionString.Trim();
                bool isOldStyleLiteral = false;
                if(line[0]!='=')
                {
                    line = "=" + line;
                    isOldStyleLiteral = true;

                }
                if (rtnVal == true && (Regex.Match(line, @"^=*[XxCc].*").Success != true))
                {
                    rtnVal = false;
                    Chronicler.LogError("Valid literal flags are: \"CcXx\"\n but '" + line[1] + "' was read", "parsing literal");
                }
                if (rtnVal == true && (Regex.Match(line, @"^=*[XxCc]*'.*").Success != true))
                {
                    rtnVal = false;
                    Chronicler.LogError("Couldn't detect opening apostrophe: '''", "parsing literal");
                }
                if (rtnVal == true && (Regex.Match(line, @"^=*[XxCc]*'.*'.*").Success != true))
                {
                    rtnVal = false;
                    Chronicler.LogError("Couldn't detect closing apostrophe: '''", "parsing literal");
                }
                if (rtnVal == true && (Regex.Match(line, @"^(=[XxCc]'.*'){0,}(;.*){0,1}$").Success != true))
                {
                    rtnVal = false;
                    Chronicler.LogWarn("Trailing garbage detected on line outside of comment", "parsing literal");
                }
                if (rtnVal == true)
                {
                    Match fullLine = (Regex.Match(line, @"^(?<literal>=[XxCc]'.*')\h{0,}(?<comment>;.*){0,1}$"));

                    if (fullLine.Success != true)
                    {
                        rtnVal = false;
                        Chronicler.WriteLine("Error parsing literal");
                    }
                    else
                    {
                        string literal = fullLine.Groups["literal"].Value;

                        if (dataStructures.literalTable.add(literal, isOldStyleLiteral) != true)
                            rtnVal = false;

                        dataStructures.literalTable.TryGetLiteral(literal, out expresionData.literal);
                    }
                }
            }
            else if (expresionString[0] == '@')
            {
                expresionString = expresionString.Substring(1, expresionString.Length - 1);
                hasXValue = testXValue.Match(expresionString).Groups["x"].Value != "";
                if (hasXValue != true)
                {
                    if (ParseTerms(dataStructures, expresionString, currentLine, out expresionData)!=true)
                    {
                        rtnVal = false;
                    }
                    expresionData.N = true;
                    expresionData.I = false;
                }
                else
                {
                    rtnVal = false;
                    Chronicler.LogError("Can not apply both indirect adressing \n\tand x register indexing, skipping: \"" + currentLine + "\"", "Resovling Expresion");
                }
            }
            else if (expresionString[0] == '#')
            {
                expresionString = expresionString.Substring(1, expresionString.Length - 1);
                hasXValue = testXValue.Match(expresionString).Groups["x"].Value != "";
                if (hasXValue != true)
                {
                    if (ParseTerms(dataStructures, expresionString, currentLine, out expresionData)!=true)
                    {
                        rtnVal = false;
                    }
                    expresionData.N = false;
                    expresionData.I = false;
                }
                else
                {
                    rtnVal = false;
                    Chronicler.LogError("Can not apply both immediate adressing \n\tand x register indexing, skipping:\"" + currentLine + "\"", "Resovling Expresion");
                }
            }
            else if (hasXValue == true)
            {
                expresionString = testXValue.Replace(expresionString, "$2$4$5");
                if (ParseTerms(dataStructures, expresionString, currentLine, out expresionData)!=true)
                {
                    rtnVal = false;
                }
                expresionData.N = true;
                expresionData.I = true;
                expresionData.X = true;
            }
            else
            {
                rtnVal = ParseTerms(dataStructures, expresionString, currentLine, out expresionData);
            }
            return rtnVal;
        }

        //*******************************************************************************************
        //***  FUNCTION ParseTerms 
        //*** ***************************************************************************************
        //***  DESCRIPTION  :  parses an arithmatic operation str to a symbol, or add a literal
        //***  INPUT ARGS   :  SymbolTable symbolTable, LiteralTable literalTable, 
        //***                   string expresion, string currentLine
        //***  OUTPUT ARGS :  out Globals.Symbol? result
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool rtnVal
        //*******************************************************************************************
        static bool ParseTerms(Globals.DataStructures dataStructures, string expresionString, string currentLine, out Globals.ExpresionData expresionData)
        {
            bool rtnVal=true;
            expresionString = expresionString.Trim();
            expresionData = new Globals.ExpresionData();

            Regex testForCommentOnlyLine = new Regex(@"^\h*(;.*)$");
            Match commentOnlyLine = testForCommentOnlyLine.Match(expresionString);
            if(commentOnlyLine.Success)
            {
                expresionData.comment = commentOnlyLine.Groups["$1"].Value;
                return true;
            }

            string line = expresionString.Trim();
            if (rtnVal == true && (Regex.Match(line, @" ^\h{0,}(-{0,1}\h{0,}[A-Za-z0-9]+).*$").Success != true))
            {
                rtnVal = false;
                Chronicler.LogError("Could not parse first term in: " + currentLine, "parsing terms");
            }
            if (rtnVal == true && (Regex.Match(line, @"^\h{0,}(-{0,1}\h{0,}[A-Za-z0-9]+)\h{0,}(([+-])\h{0,}(-{0,1}\h{0,}[A-Za-z0-9]+)){0,1}\h{0,}(;.*){0,1}$").Success != true))
            {
                rtnVal = false;
                Chronicler.LogError("Couldn't parse second term in: "+currentLine, "parsing terms");
            }
            string first="";
            string arithmaticOperator="";
            string second ="";
            if (rtnVal == true)
            {
                Match fullLine = (Regex.Match(line, @"^\h{0,}(?<first>-{0,1}\h{0,}[A-Za-z0-9]+)\h{0,}(?<testTermCount>(?<operand>[+-])\h{0,}(?<second>-{0,1}\h{0,}[A-Za-z0-9]+)){0,1}\h{0,}(;.*){0,1}$"));
                if (fullLine.Success != true)
                {
                    rtnVal = false;
                    Chronicler.WriteLine("Error parsing literal");
                }
                first = fullLine.Groups["first"].Value.Trim();
                arithmaticOperator = fullLine.Groups["operand"].Value.Trim();
                second = fullLine.Groups["second"].Value.Trim();

                Regex stripWhiteSpace = new Regex(@"\s+");
                first = stripWhiteSpace.Replace(first, "");
                arithmaticOperator = stripWhiteSpace.Replace(arithmaticOperator, "");
                second = stripWhiteSpace.Replace(second, "");
                if (second != "" && arithmaticOperator == "")
                    rtnVal = false;
            }
            if(rtnVal == true)
            {
                rtnVal = ParseTerm(dataStructures.symbolTable, first, out expresionData.first, currentLine);
                if(second!=null)
                {
                    if (arithmaticOperator == "+")
                    {
                        expresionData.rflag = Globals.Symbol.AddRFlags(expresionData.first.Value, expresionData.second.Value);
                        expresionData.operatorValue = Globals.ExpresionData.Arithmetic.ADD;
                    }
                    else if (arithmaticOperator == "-")
                    {
                        expresionData.rflag = Globals.Symbol.SubtractRFlags(expresionData.first.Value, expresionData.second.Value);
                        expresionData.operatorValue = Globals.ExpresionData.Arithmetic.SUBTRACT;
                    }
                    else
                    {
                        rtnVal = false;
                        Chronicler.LogError("Invalid operator value for line: " + currentLine, "term arithmatic module");
                    }
                    rtnVal = rtnVal == true ? ParseTerm(dataStructures.symbolTable, second, out expresionData.second, currentLine) : false;
                        
                }
            }
            return rtnVal;
        }

        //*******************************************************************************************
        //***  FUNCTION ParseTerm 
        //*** ***************************************************************************************
        //***  DESCRIPTION  :  parses an term str(int or symbol) to a symbol struct
        //***  INPUT ARGS   :  SymbolTable symbolTable, string term, string currentLine=""
        //***  OUTPUT ARGS :  out Globals.Symbol? sym
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool rtnVal
        //*******************************************************************************************
        static bool ParseTerm(SymbolTable symbolTable, string term, out Globals.Symbol? sym, string currentLine)
        {
            bool rtnVal = true;
            if (char.IsDigit(term[0]) || (term[0]=='-' && char.IsDigit(term[1])) )
            {
                sym = null;
                if (ParseNum(term, out Globals.Symbol? tmp, currentLine) == true)
                {
                    sym = tmp;
                }
                else
                {
                    Chronicler.LogDetailedInfo("Parse term(digit) in Expresion Handler failed");
                    rtnVal = false;
                }
            }
            else
            {
                int Mval = 1;
                if (term[0] == '-')
                {
                    term.Substring(1, term.Length - 1);
                    Mval = -1;
                }
                Chronicler.LogDetailedInfo("Parse term(sym) start");
                if ((symbolTable.SearchSymbol(term.Trim(), currentLine, out sym)) != true)
                {
                    Chronicler.LogDetailedInfo("Parse term(sym) in Expresion Handler failed");
                    rtnVal = false;
                }
                if(sym.HasValue)
                {
                    Globals.Symbol tmp = sym.Value;
                    tmp.value *= Mval;
                    sym = tmp;
                }
                Chronicler.LogDetailedInfo("Parse term(sym) end");
            }

            return rtnVal;
        }


        //*******************************************************************************************
        //***  FUNCTION ParseNum 
        //*** ***************************************************************************************
        //***  DESCRIPTION  :  parses an int str to a symbol
        //***  INPUT ARGS   :  string digits, string currentLine=""
        //***  OUTPUT ARGS :  out Globals.Symbol? sym
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool rtnVal
        //*******************************************************************************************
        static bool ParseNum(string digits, out Globals.Symbol? sym, string currentLine="")
        {
            Globals.Symbol tmp = default;
            bool rtnVal = true;

            if (int.TryParse(digits, out tmp.value) == true)
            {
                tmp.label = digits;
                tmp.RFlag = false;
                tmp.MFlag = false;
                sym = tmp;
            }
            else
            {
                Chronicler.LogError("Unable to parse integer value("+digits+"), skipping: \"" + currentLine + "\"", "Resolving Expresion");
                sym = null;
                rtnVal = false;
            }
            return rtnVal;
        }
    }
}
