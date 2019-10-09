﻿using System;
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
    class LiteralTable
    {
        //*********************************************************************
        //*** struct : LiteralValue
        //*********************************************************************
        //*** DESCRIPTION :   Stores a single literal's information 
        //*********************************************************************
        public struct LiteralValue
        {
            public string label;
            public string value;
            public int address;
            public int Length => value.Length / 2;
        }
        int count = 0;
        LinkedList<LiteralValue> literalTable = new LinkedList<LiteralValue>();

        //************************************************************************
        //***  FUNCTION PrintTable 
        //*** ********************************************************************
        //***  DESCRIPTION  :  prints the current contents of the literal table
        //***  INPUT ARGS   :  N/A 
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //************************************************************************
        public void PrintTable()
        {
            Debug.WriteLine("NAME\t\tVALUE\t\tLENGTH\tADDRESS");
            foreach(LiteralValue lv in literalTable)
            {
                StringBuilder sb = new StringBuilder("");
                for (int x = 0; x < (16 - lv.label.Length); x++)
                {
                    sb.Append(" ");
                }
                Debug.Write(lv.label + sb.ToString());
                sb = new StringBuilder("");
                for (int x = 0; x < (16 - lv.value.Length); x++)
                {
                    sb.Append(" ");
                }
                Debug.Write(lv.value+sb.ToString());
                Debug.Write(lv.Length.ToString());
                Debug.Write("\t" + lv.address.ToString());
                Debug.NewLine();
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
        public bool add(string literal)
        {
            bool rtnVal = true;
            bool isChar = default;
            LiteralValue literalValue = default;
            literal = literal.CompactAndTrimWhitespaces();
            if (literal[1] == 'X' || literal[1] == 'x')
            {
                literal = literal.ToUpper();
                isChar = false;
            }
            else if (literal[1] == 'C' )
            {
                isChar = true;
            }
            else if (literal[1] == 'c')
            {
                isChar = true;
                StringBuilder sb = new StringBuilder(literal);
                sb[1] = 'C';
                literal = sb.ToString();
            }
            else
            {
                Debug.LogError("Invalid literal type, please indicate hex\'X\' or Char\'C\'\n", "Adding Literal");
                rtnVal = false;
            }
            literalValue.label = literal;

            if (rtnVal==true)
            {
                if(literal[2] != '\'' || '\'' != literal[literal.Length-1])
                {
                    Debug.LogError("Invalid enclosing symbols, please surround your literal with \n\tsingle quotes '\'' and remove any trailing garbage from the expresion\n", "Adding Literal");
                    rtnVal = false;
                }
            }

            if (rtnVal == true)
            {
                if(isChar==true)
                {
                    char[] tmpArr = literal.Substring(3, literal.Length - 4).ToCharArray();
                    literalValue.value = Globals.CharArrToHexStr(tmpArr);

                }
                else
                {
                    literalValue.value = literal.Substring(3, literal.Length - 4);
                    literalValue.value = (literalValue.value.Length % 2 == 1 ? "0" : "") + literalValue.value;
                    if (validateHex(literalValue.value) != true)
                    {
                        rtnVal = false;
                        Debug.LogError("Error in literal, Hex specified, but non Hex chars were detected", "Adding Literal");
                    }
                }
            }
            if (rtnVal == true)
            {
                foreach (LiteralValue lv in literalTable.TakeWhile( lv => { return rtnVal; }))//super slow... but linked lists are...
                {
                    if (lv.label == literal)
                    {
                        rtnVal = false;
                        Debug.LogError("Error in literal, This literal is already in the \n\tliteral table!\n", "Adding Literal");
                    }
                }
            }
            if (rtnVal == true)
            {
                literalValue.address = count;
                Debug.LogInfo("Adding: "+literalValue.label, "Adding Literal");
                literalTable.AddLast(literalValue);
                count++;
            }
            return rtnVal;
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
        bool validateHex(string charStr)
        {
            charStr = charStr.ToUpper();
            bool isHex = true;
            foreach (char c in charStr.TakeWhile(c => { return isHex; }))
            {
                if (!(char.IsDigit(c) || (((int)c) >= 'A' && ((int)c) <= 'F')))
                {
                    isHex = false;
                }
            }
            return isHex;
        }
    }
}
