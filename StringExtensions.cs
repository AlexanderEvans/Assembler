using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    //*******************************************************************
    //*** NAME : Alex Evans
    //*** CLASS : CSc 354 Intro to systems
    //*** ASSIGNMENT : 1
    //*** DUE DATE : 9/18/2019
    //*** INSTRUCTOR : GAMRADT 
    //********************************************************************
    //*** DESCRIPTION :   This class adds string extensions for string 
    //***                 parsing
    //********************************************************************
    public static class StringExtensions
    {
        //************************************************************************
        //***  FUNCTION CountStringCharachters 
        //*** ********************************************************************
        //***  DESCRIPTION  :  Counts the occurances of a charachter in a string
        //***  INPUT ARGS   :  this string str, char myChar
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  int count
        //************************************************************************
        public static int CountStringCharachters(this string str, char myChar)
        {
            int count = 0;
            foreach (char c in str)
            {
                if (c == myChar)
                    count++;
            }
            return count;
        }
        //*************************************************************************
        //***  FUNCTION CountStringCharachters 
        //*** *********************************************************************
        //***  DESCRIPTION  :  Converts string to string builder 
        //***                     and calls CompactWhitespaces
        //***  INPUT ARGS   :  this string str
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  string N/A 
        //*************************************************************************
        public static String CompactAndTrimWhitespaces(this string str)
        {
            return CompactWhitespaces(new StringBuilder(str)).ToString();
        }
        //*************************************************************************
        //***  FUNCTION CountStringCharachters 
        //*** *********************************************************************
        //***  DESCRIPTION  :  trims leading and trailing whitespace and converts
        //***                     all other whitespace to a single space
        //***  INPUT ARGS   :  StringBuilder stringBuilder
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  StringBuilder stringBuilder
        //*************************************************************************
        public static StringBuilder CompactWhitespaces(StringBuilder stringBuilder)
        {
            if (sb.Length != 0)
            {
                int first;
                for (first = 0; first < sb.Length && Char.IsWhiteSpace(sb[first]); first++)
                {

                }

                // if sb has only whitespaces, then return empty string
                if (first == sb.Length)
                {
                    sb.Length = 0;
                }
                else
                {
                    // set [end] to last not-whitespace char

                    int last;
                    for (last = sb.Length - 1; last >= 0 && Char.IsWhiteSpace(sb[last]); last--)
                    {

                    }
                    // compact string

                    int current = 0;
                    bool previousIsWhitespace = false;

                    for (int i = first; i <= last; i++)
                    {
                        if (Char.IsWhiteSpace(sb[i]) && previousIsWhitespace!=true)
                        {
                            previousIsWhitespace = true;
                            sb[current] = ' ';
                            current++;
                        }
                        else if(Char.IsWhiteSpace(sb[i])==false)
                        {
                            previousIsWhitespace = false;
                            sb[current] = sb[i];
                            current++;
                        }
                    }

                    sb.Length = current;
                }
            }

            return sb;
        }
    }
}
