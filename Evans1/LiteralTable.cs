using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

//*******************************************************************
//*** NAME : Alex Evans
//*** CLASS : CSc 354 Intro to systems
//*** ASSIGNMENT : 2
//*** DUE DATE : 10/9/2019
//*** INSTRUCTOR : GAMRADT 
//********************************************************************
//*** DESCRIPTION :   This class adds a literal table
//********************************************************************

namespace Evans1
{
    //*********************************************************************
    //*** class : LiteralTable
    //*********************************************************************
    //*** DESCRIPTION :   Stores a single literal's information using a
    //***                   Linked list
    //*********************************************************************
    public class LiteralTable
    {
        //*********************************************************************
        //*** struct : LiteralValue
        //*********************************************************************
        //*** DESCRIPTION :   Stores a single literal's information 
        //*********************************************************************
        public class LiteralValue
        {
            public enum DataType { HEX, CHAR}
            public DataType dataType;
            public bool isOldStyleLiteral;
            public string label;
            public string hexValue;
            public int address;
            public int Length => hexValue.Length / 2;
        }
        int count = 0;
        public LinkedList<LiteralValue> literalTable = new LinkedList<LiteralValue>();

        public void setStartAddress(int newAdress)
        {
            if (literalTable.Count!=0)
            {
                literalTable.First.Value.address=newAdress;
            }
            int prevAdrr = -1;
            int prevLen = -1;
            foreach (LiteralValue lv in literalTable)
            {
                if(lv!=literalTable.First.Value)
                {
                    lv.address = prevAdrr + prevLen;
                }
                prevAdrr = lv.address;
                prevLen = lv.Length;
            }
        }


        //************************************************************************
        //***  FUNCTION TryGetLiteral
        //*** ********************************************************************
        //***  DESCRIPTION  :  trys to get a literal and returns false on failure
        //***  INPUT ARGS   :  string literal
        //***  OUTPUT ARGS :  LiteralValue literalValue
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool found
        //************************************************************************
        public bool TryGetLiteral(string literal, out LiteralValue literalValue)
        {
            literalValue = null;
            bool found = false;
            foreach(LiteralValue Lit in literalTable.TakeWhile(x => found))
            {
                if(Lit.label==literal)
                {
                    literalValue = Lit;
                    found = true;
                }
            }
            return found;
        }


        //************************************************************************
        //***  FUNCTION PrintTable 
        //*** ********************************************************************
        //***  DESCRIPTION  :  prints the current contents of the literal table
        //***  INPUT ARGS   :  N/A 
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //************************************************************************
        public void PrintTable(Chronicler.OutputOptions outputOptions = Chronicler.OutputOptions.IGNORE)
        {
            Chronicler.WriteLine("NAME\t\tVALUE\t\tLENGTH\tADDRESS", outputOptions);
            Chronicler.WriteLine("=====================================", outputOptions);
            StringBuilder sb = new StringBuilder("");
            foreach (LiteralValue lv in literalTable)
            {
                sb.Clear();
                for (int x = 0; x < (16 - lv.label.Length); x++)
                {
                    sb.Append(" ");
                }
                Chronicler.Write(lv.label + sb.ToString(), outputOptions);
                sb.Clear();
                for (int x = 0; x < (16 - lv.hexValue.Length); x++)
                {
                    sb.Append(" ");
                }
                Chronicler.Write(lv.hexValue+sb.ToString(), outputOptions);
                Chronicler.Write(lv.Length.ToString(), outputOptions);
                Chronicler.WriteLine("\t" + lv.address.ToString("X6"), outputOptions);
            }
        }

        //************************************************************************
        //***  FUNCTION add 
        //*** ********************************************************************
        //***  DESCRIPTION  :  add a literal to the literal table
        //***  INPUT ARGS   :  string literal
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool rtnVal
        //************************************************************************
        public void add(LiteralValue literalValue)
        {
            int curAddr = literalTable.Count!=0?literalTable.Last.Value.address:0;
            literalValue.address = curAddr + literalValue.Length;
            literalTable.AddLast(literalValue);
        }


    }
}
