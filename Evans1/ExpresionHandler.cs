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

        static bool ParseNum(string digits, out Globals.Symbol? sym, string currentLine="")
        {
            Globals.Symbol tmp = default;
            bool rtnVal = true;

            if (int.TryParse(digits, out tmp.value) == true)
            {
                tmp.label = digits;
                tmp.RFlag = false;
                tmp.MFlag = false;
                tmp.IFlag = true;
                tmp.NFlag = false;
                tmp.XFlag = false;
                sym = tmp;
            }
            else
            {
                Debug.LogError("Attempted to resolve invalid integer!\n\tUnable to parse integer value, skipping: " + currentLine, "Resolving Expresion");
                sym = null;
                rtnVal = false;
            }
            return rtnVal;
        }
    }
}
