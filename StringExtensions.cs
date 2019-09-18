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
    }
}
