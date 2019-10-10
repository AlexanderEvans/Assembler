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

            public bool NFlag;
            public bool XFlag;
            //************************************************************************
            //***  FUNCTION Print 
            //*** ********************************************************************
            //***  DESCRIPTION  :  prints the current contents of the symbol 
            //***  INPUT ARGS   :  N/A 
            //***  OUTPUT ARGS :  N/A
            //***  IN/OUT ARGS   :  N/A  
            //***  RETURN :  N/A
            //************************************************************************
            public void Print()
            {
                TerminalOutput.WriteLine(label + "\t" + RFlag + "\t" + value + "\t" + MFlag + "\t" + IFlag);
            }
            //************************************************************************
            //***  FUNCTION operator + 
            //*** ********************************************************************
            //***  DESCRIPTION  :  adds 2 symbols 
            //***  INPUT ARGS   :  Symbol first, Symbol second
            //***  OUTPUT ARGS :  N/A
            //***  IN/OUT ARGS   :  N/A  
            //***  RETURN :  Symbol? rtnVal
            //************************************************************************
            static public Symbol? operator +(Symbol first, Symbol second)
            {
                Symbol? rtnVal = null;
                Symbol tmp = default;
                tmp.label = "";
                tmp.NFlag = first.NFlag | second.NFlag;
                tmp.IFlag = true;
                tmp.XFlag = false;
                tmp.MFlag = first.MFlag | second.MFlag;
                tmp.value = first.value + second.value;
                int rVal = (((int)(BoolConverter)first.RFlag) << 1) + ((int)(BoolConverter)second.RFlag);
                switch (rVal)
                {
                    case 0b00:
                        tmp.RFlag = false;
                        rtnVal = tmp;
                        break;
                    case 0b01:
                        tmp.RFlag = true;
                        rtnVal = tmp;
                        break;
                    case 0b10:
                        tmp.RFlag = true;
                        rtnVal = tmp;
                        break;
                    case 0b11:
                        //do nothing and return null
                        break;
                }
                return rtnVal;
            }
            //************************************************************************
            //***  FUNCTION operator - 
            //*** ********************************************************************
            //***  DESCRIPTION  :  subtracts 2 symbols 
            //***  INPUT ARGS   :  Symbol first, Symbol second
            //***  OUTPUT ARGS :  N/A
            //***  IN/OUT ARGS   :  N/A  
            //***  RETURN :  Symbol? rtnVal
            //************************************************************************
            static public Symbol? operator -(Symbol first, Symbol second)
            {
                Symbol? rtnVal = null;
                Symbol tmp = default;
                tmp.label = "";
                tmp.NFlag = first.NFlag | second.NFlag;
                tmp.IFlag = true;
                tmp.XFlag = false;
                tmp.MFlag = first.MFlag | second.MFlag;
                tmp.value = first.value - second.value;
                int rVal = (((int)(BoolConverter)first.RFlag) << 1) + ((int)(BoolConverter)second.RFlag);
                switch (rVal)
                {
                    case 0b00:
                        tmp.RFlag = false;
                        rtnVal = tmp;
                        break;
                    case 0b01:
                        //do nothing and return null
                        break;
                    case 0b10:
                        tmp.RFlag = true;
                        rtnVal = tmp;
                        break;
                    case 0b11:
                        tmp.RFlag = false;
                        rtnVal = tmp;
                        break;
                }
                return rtnVal;
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
    struct BoolConverter
    {
        bool myBool;
        //************************************************************************
        //***  FUNCTION implicit operator BoolConverter
        //*** ********************************************************************
        //***  DESCRIPTION  :  converts bool to BoolConverter
        //***  INPUT ARGS   :  bool first
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  BoolConverter boolConverter
        //************************************************************************
        public static implicit operator BoolConverter(bool first)
        {
            BoolConverter boolConverter=default;
            boolConverter.myBool = first;
            return boolConverter;
        }
        //************************************************************************
        //***  FUNCTION implicit operator BoolConverter
        //*** ********************************************************************
        //***  DESCRIPTION  :  converts BoolConverter to int
        //***  INPUT ARGS   :  BoolConverter first
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool N/A
        //************************************************************************
        public static implicit operator int(BoolConverter first)
        {
            if (first.myBool)
                return 1;
            else
                return 0;
        }
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
