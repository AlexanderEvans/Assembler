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

        bool ParseTerm(SymbolTable symbolTable, string term, out Globals.Symbol? sym, string currentLine)
        {
            if (char.IsDigit(term[0]))
            {
                sym = null;
                if (ParseNum(term, out Globals.Symbol? tmp, currentLine) == true)
                {
                    sym = tmp;
                }
            }
            else
            {
                sym = symbolTable.SearchSymbol(term.CompactAndTrimWhitespaces());
            }
            bool rtnVal = true;
            if (sym == null)
                rtnVal = false;
            return rtnVal;
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
