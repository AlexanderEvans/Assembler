using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
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
    public class AsciiComparer : Comparer<string> 
    {
        public override int Compare(string x, string y)
        {
            int rtnVal = string.CompareOrdinal(x, y);
            if (x == y)
                rtnVal = 0;
            else if (rtnVal > 0)
            {
                rtnVal = 1;
            }
            else if (rtnVal < 0)
            {
                rtnVal = -1;
            }
            return rtnVal;
        }
        public override bool Equals(object obj) => base.Equals(obj);
        public override string ToString() => base.ToString();
        public override int GetHashCode() => base.GetHashCode();
    }
}
