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
