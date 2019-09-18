using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler
{
    // Extension methods must be defined in a static class.
    public static class StringExtensions
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
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

        public static String CompactAndTrimWhitespaces(this string str)
        {
            return CompactWhitespaces(new StringBuilder(str)).ToString();
        }

        public static StringBuilder CompactWhitespaces(StringBuilder sb)
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
