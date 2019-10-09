using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    class Globals
    {
        public enum AddressingMode : byte 
        {
            IMMEDIATE = 1,
            INDIRECT = 2,
            SIMPLE_DIRECT = 3,
            ERROR,
        };

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
                Debug.WriteLine(label + "\t" + RFlag + "\t" + value + "\t" + MFlag + "\t" + IFlag);
            }
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

    struct BoolConverter
    {
        bool myBool;
        public static implicit operator BoolConverter(bool first)
        {
            BoolConverter boolConverter=default;
            boolConverter.myBool = first;
            return boolConverter;
        }
        public static implicit operator int(BoolConverter first)
        {
            if (first.myBool)
                return 1;
            else
                return 0;
        }
    }

    public class AsciiComparer : Comparer<string> 
    {
        public override int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}
