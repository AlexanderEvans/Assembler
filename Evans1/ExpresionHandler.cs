using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    class ExpresionHandler
    {
        static bool ResolveExpresion(SymbolTable symbolTable, string expresion, string currentLine, out Globals.Symbol? symbol, out Globals.AddressingMode addressingMode)
        {
            addressingMode = Globals.AddressingMode.SIMPLE_DIRECT;
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
                else
                {

                }
            }
            else
            {
                Debug.LogError("Invalid expresion.  There is more than one arithmatic operator indicating 3 or more terms.", "Resolving Expresion");
            }
        bool ParseNum(string digits, out Globals.Symbol? sym, string currentLine="")
        {
            bool rtnVal = true;
            Globals.Symbol tmp = default;
            tmp.label = digits;
            tmp.RFlag = false;
            tmp.MFlag = false;
            tmp.IFlag = true;
            if (int.TryParse(digits, out tmp.value) != true)
            {
                rtnVal = false;
                Debug.LogError("Attempted to resolve invalid expresion!\n\tArithmitic must be performed on exactly 2 terms, skipping: " + currentLine, "Resolving Expresion");
            }

            if (rtnVal==true)
                sym = tmp;
            else
                sym = null;
            return rtnVal;
        }
    }
}
