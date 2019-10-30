using System;
using System.Collections.Generic;
using System.Text;
//*******************************************************************
//*** NAME : Alex Evans
//*** CLASS : CSc 354 Intro to systems
//*** ASSIGNMENT : 2
//*** DUE DATE : 10/9/2019
//*** INSTRUCTOR : GAMRADT 
//********************************************************************
//*** DESCRIPTION :   This class adds a globals agragator class
//********************************************************************
namespace Evans1
{
    //*********************************************************************
    //*** Class : Globals
    //*********************************************************************
    //*** DESCRIPTION :   Aggragates global variables and functions
    //*********************************************************************
    class Globals
    {
        public class DataStructures
        {
            public SymbolTable symbolTable = new SymbolTable();
            public LiteralTable literalTable = new LiteralTable();
            public opcodeTable opcodeTable = new opcodeTable();
            public List<ExpresionData> expresionData = new List<ExpresionData>();
        }


        //*********************************************************************
        //*** Struct : Symbol
        //*********************************************************************
        //*** DESCRIPTION :   This struct contains the Symbol data to be used
        //***                   in the kvp as the value of the key pair
        //*********************************************************************
        public struct Symbol
        {
            //given data from file
            public string label;//the symbols's name/Identifier string/Label
            public bool RFlag;
            public int value;

            //deduced data
            public bool IFlag;
            public bool MFlag;
            //************************************************************************
            //***  FUNCTION Print 
            //*** ********************************************************************
            //***  DESCRIPTION  :  prints the current contents of the symbol 
            //***  INPUT ARGS   :  N/A 
            //***  OUTPUT ARGS :  N/A
            //***  IN/OUT ARGS   :  N/A  
            //***  RETURN :  N/A
            //************************************************************************
            public void Print(Chronicler.OutputOptions outputOptions)
            {
                Chronicler.WriteLine(label + "\t" + RFlag + "\t" + value + "\t" + MFlag + "\t" + IFlag, outputOptions);
            }
            //************************************************************************
            //***  FUNCTION AddRFlags 
            //*** ********************************************************************
            //***  DESCRIPTION  :  adds 2 symbols  rflags
            //***  INPUT ARGS   :  Symbol first, Symbol second
            //***  OUTPUT ARGS :  N/A
            //***  IN/OUT ARGS   :  N/A  
            //***  RETURN :  bool? rtnVal
            //************************************************************************
            static public bool? AddRFlags(Symbol first, Symbol second)
            {
                bool? rtnVal = null;
                int rVal = (first.RFlag.ToInt() << 1) + second.RFlag.ToInt();
                switch (rVal)
                {
                    case 0b00:
                        rtnVal = false;
                        break;
                    case 0b01:
                        rtnVal = true;
                        break;
                    case 0b10:
                        rtnVal = true;
                        break;
                    case 0b11:
                        rtnVal = null;
                        //do nothing and return null
                        break;
                }
                return rtnVal;
            }
            //************************************************************************
            //***  FUNCTION SubtractRFlags
            //*** ********************************************************************
            //***  DESCRIPTION  :  subtracts 2 symbols rflags
            //***  INPUT ARGS   :  Symbol first, Symbol second
            //***  OUTPUT ARGS :  N/A
            //***  IN/OUT ARGS   :  N/A  
            //***  RETURN :  bool? rtnVal
            //************************************************************************
            static public bool? SubtractRFlags(Symbol first, Symbol second)
            {
                bool? rtnVal = null;
                int rVal = (first.RFlag.ToInt() << 1) + second.RFlag.ToInt();
                switch (rVal)
                {
                    case 0b00:
                        rtnVal = false;
                        break;
                    case 0b01:
                        rtnVal = null;
                        //do nothing and return null
                        break;
                    case 0b10:
                        rtnVal = true;
                        break;
                    case 0b11:
                        rtnVal = false;
                        break;
                }
                return rtnVal;
            }
        }
        public class ExpresionData
        {
            public enum Contents { SYMBOL, LITERAL, EMPTY, ERROR };
            public enum Arithmetic { ADD, SUBTRACT};
            public LiteralTable.LiteralValue? literal;
            public Symbol? first;
            public Arithmetic? operatorValue;
            public bool? rflag;
            public Symbol? second;
            public bool N;
            public bool I;
            public bool X;
            public bool B;
            public bool P;
            public bool E;
            public bool IsValidContent
            {
                get
                {
                    if(literal!=null)
                    {
                        if (first != null || second != null || operatorValue != null)
                            return false;
                    }
                    else
                    {
                        if ((second != null || operatorValue != null) && first == null)
                            return false;
                        if ((first != null && second != null) && operatorValue == null)
                            return false;
                    }
                    return true;
                }
            }
            public bool IsSymbol => first != null;
            public Contents ExpresionType
            {
                get
                {
                    if (IsValidContent != true)
                        return Contents.ERROR;
                    else if (literal != null)
                        return Contents.LITERAL;
                    else if (IsSymbol)
                        return Contents.SYMBOL;
                    else
                        return Contents.EMPTY;
                }
            }
            public ExpresionData()
            {
                literal = null;
                first=null;
                operatorValue = null;
                second = null;
                N=false;
                I=false;
                X=false;
                B=false;
                P=false;
                E=false;
            }
        }
        //************************************************************************
        //***  FUNCTION CharArrToHexStr
        //*** ********************************************************************
        //***  DESCRIPTION  :  converts chars to hex byte strings
        //***  INPUT ARGS   :  char[] charArr
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  string N/A
        //************************************************************************
        public static string CharArrToHexStr(char[] charArr)
        {
            StringBuilder stringBuilder = new StringBuilder(charArr.Length * 2);
            string HexDigits = "0123456789ABCDEF";
            foreach (char c in charArr)
            {
                stringBuilder.Append(HexDigits[((byte)c) >> 4]);
                stringBuilder.Append(HexDigits[((byte)c) & 0xF]);
            }
            return stringBuilder.ToString();
        }
    }

    //*********************************************************************
    //*** Struct : BoolConverter
    //*********************************************************************
    //*** DESCRIPTION :   converts a bool to int implicitly
    //*********************************************************************
    public static class BoolConverter
    {
        //************************************************************************
        //***  FUNCTION ToInt
        //*** ********************************************************************
        //***  DESCRIPTION  :  converts Bool to int
        //***  INPUT ARGS   :  bool boolean
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  int N/A
        //************************************************************************
        public static int ToInt(this bool boolean) => boolean ? 1 : 0;
    }
    //*********************************************************************
    //*** class : AsciiComparer
    //*********************************************************************
    //*** DESCRIPTION :   compares strings by ascii values
    //*********************************************************************
    public class AsciiComparer : Comparer<string> 
    {
        public override int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}
