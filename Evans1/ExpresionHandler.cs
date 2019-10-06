using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    class ExpresionHandler
    {
        static bool ResolveExpresion(SymbolTable symbolTable, string expresion, string currentLine, out Globals.Symbol? symbol)
        {
            symbol = null;
            int arithmaticOperatorCount = expresion.CountStringCharachters(out List<char> found, '+', '-');
            if (arithmaticOperatorCount <= 1)
            {
                if(found.Contains('+'))
                {

                }
                else if(found.Contains('-'))
                {

                }
            }
            else
            {
                Debug.LogError("Invalid expresion.  There is more than one arithmatic operator indicating 3 or more terms.", "Resolving Expresion");
            }
        }
    }
}
