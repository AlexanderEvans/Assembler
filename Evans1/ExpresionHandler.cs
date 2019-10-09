using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Evans1
{
    class ExpresionHandler
    {
        public static void ParseExpresionFile(SymbolTable symbolTable, LiteralTable literalTable, string filePath)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                {

                    try
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            int LineNumber = 1;
                            Debug.WriteLine("EXPRESION\tVALUE\tRELOCATABLE\tN-Bit\tI-Bit\tX-Bit");
                            while (!streamReader.EndOfStream)
                            {
                                //get the line and trim whitespace
                                string currentLine = streamReader.ReadLine().Trim();
                                if (ResolveF3F4Expresion(symbolTable, literalTable, currentLine, currentLine, out Globals.Symbol? symbol))
                                {
                                    if (symbol != null)
                                    {
                                        StringBuilder sb = new StringBuilder("");
                                        for (int x = 0; x < (16 - currentLine.Length); x++)
                                        {
                                            sb.Append(" ");
                                        }
                                        Debug.Write(currentLine + sb.ToString());
                                        Debug.Write(symbol.Value.value.ToString());
                                        Debug.Write("\t" + ((symbol.Value.RFlag) ? "RELOCATABLE" : "ABSOLUTE"));
                                        Debug.Write("\t" + ((symbol.Value.NFlag) ? "1" : "0"));
                                        Debug.Write("\t" + ((symbol.Value.IFlag) ? "1" : "0"));
                                        Debug.Write("\t" + ((symbol.Value.XFlag) ? "1" : "0"));
                                        Debug.NewLine();
                                    }
                                }
                                else
                                {
                                    Debug.LogError("Opps, something went wrong in resolving the expresion at \n\tline: \"" + LineNumber + "\"\t Please check your expresion file, skipping: \"" + currentLine + "\"" + "\n", "Expresion File Parsing");
                                }
                                LineNumber++;
                            }
                            Debug.LogInfo("Done");
                            Debug.WriteLine("Done");
                        }
                    }
                    catch (IOException e)
                    {
                        Debug.WriteLine(e.Message);
                        Debug.LogError("failed to open File: " + filePath);
                    }
                }
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
                Debug.LogError("failed to open File: " + filePath);
            }
        }

        public static bool ResolveF3F4Expresion(SymbolTable symbolTable, LiteralTable literalTable, string expresion, string currentLine, out Globals.Symbol? symbol)
        {
            symbol = null;
            bool rtnVal = true;
            expresion = expresion.Trim();
            if (expresion[0] == '@')
            {
                if (!((expresion[expresion.Length - 1] == 'X' || expresion[expresion.Length - 1] == 'x') && expresion[expresion.Length - 2] == ','))
                {
                    expresion = expresion.Substring(1, expresion.Length - 1);
                    if (ParseTerms(symbolTable, literalTable, expresion, currentLine, out symbol))
                    {
                        Globals.Symbol tmp = symbol.Value;
                        tmp.NFlag = true;
                        tmp.IFlag = false;
                        symbol = tmp;
                    }
                    else
                    {
                        rtnVal = false;
                    }
                }
                else
                {
                    rtnVal = false;
                    Debug.LogError("Can not apply both indirect adressing \n\tand x register indexing, skipping: \"" + currentLine + "\""+"\n", "Resovling Expresion");
                }
            }
            else if (expresion[0] == '#')
            {
                if (!((expresion[expresion.Length - 1] == 'X' || expresion[expresion.Length - 1] == 'x') && expresion[expresion.Length - 2] == ','))
                {
                    expresion = expresion.Substring(1, expresion.Length - 1);
                    if (ParseTerms(symbolTable, literalTable, expresion, currentLine, out symbol))
                    {
                        Globals.Symbol tmp = symbol.Value;
                        tmp.NFlag = false;
                        tmp.IFlag = true;
                        symbol = tmp;
                    }
                    else
                    {
                        rtnVal = false;
                    }
                }
                else
                {
                    rtnVal = false;
                    Debug.LogError("Can not apply both immediate adressing \n\tand x register indexing, skipping:\"" + currentLine + "\""+"\n", "Resovling Expresion");
                }
            }
            else if ((expresion[expresion.Length-1] == 'X' || expresion[expresion.Length - 1] == 'x') && expresion[expresion.Length - 2] == ',')
            {
                expresion = expresion.Substring(0, expresion.Length - 2);
                if (ParseTerms(symbolTable, literalTable, expresion, currentLine, out symbol))
                {
                    Globals.Symbol tmp = symbol.Value;
                    tmp.NFlag = true;
                    tmp.IFlag = true;
                    tmp.XFlag = true;
                    symbol = tmp;
                }
                else
                {
                    rtnVal = false;
                }
            }
            else
            {
                rtnVal = ParseTerms(symbolTable, literalTable, expresion, currentLine, out symbol);
            }
            return rtnVal;
        }

        static bool ParseTerms(SymbolTable symbolTable, LiteralTable literalTable, string expresion, string currentLine, out Globals.Symbol? result)
        {
            result = null;
            bool rtnVal=true;
            expresion = expresion.Trim();
            int arithmaticOperatorCount = expresion.CountStringCharachters(out List<char> found, '+', '-');
            if (arithmaticOperatorCount <= 1)
            {
                if (found.Count == 1)
                {
                    string[] terms = expresion.Split('+', StringSplitOptions.RemoveEmptyEntries);
                    if(terms.Length==1)
                        terms = expresion.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    if (terms.Length == 2)
                    {
                        Globals.Symbol? first;
                        Globals.Symbol? second;
                        terms[0] = terms[0].Trim();
                        terms[1] = terms[1].Trim();
                        if(ParseTerm(symbolTable, terms[0], out first, currentLine)==true && ParseTerm(symbolTable, terms[1], out second, currentLine)==true)
                        {
                            if (found.Contains('+'))
                            {
                                result = first.Value + second.Value;
                            }
                            else
                            {
                                result = first.Value - second.Value;
                            }
                            if (result == null)
                            {
                                rtnVal = false;
                                Debug.LogDetailedInfo("\t...Arithmetic operation in Expresion Handler failed");
                            }
                        }
                        else
                        {
                            rtnVal = false;
                        }
                    }
                    else
                    {
                        Debug.LogError("Attempted to resolve invalid expresion!\n\tArithmitic must be performed on exactly 2 terms(detected "+terms.Length+"), skipping: \"" + currentLine + "\"" + "\n", "Resolving Expresion");
                        rtnVal = false;
                    }
                }
                else
                {
                    if (expresion[0] == '=')
                    {
                        if (literalTable.add(expresion) != true)
                            rtnVal = false;
                    }
                    else
                    {
                        if (ParseTerm(symbolTable, expresion, out result, currentLine)!=true)
                            rtnVal = false;
                    }
                }
            }
            else
            {
                Debug.LogError("Attempted to resolve invalid expresion!\n\tThere is more than one arithmatic operator indicating 3 or more terms, skipping: \"" + currentLine + "\""+ "\n", "Resolving Expresion");
                rtnVal = false;
            }
            return rtnVal;
        }

        static bool ParseTerm(SymbolTable symbolTable, string term, out Globals.Symbol? sym, string currentLine)
        {
            bool rtnVal = true;
            if (char.IsDigit(term[0]))
            {
                sym = null;
                if (ParseNum(term, out Globals.Symbol? tmp, currentLine) == true)
                {
                    sym = tmp;
                }
                else
                {
                    Debug.LogDetailedInfo("Parse term(digit) in Expresion Handler failed");
                    rtnVal = false;
                }
            }
            else
            {
                Debug.LogDetailedInfo("Parse term(sym) start");
                if ((symbolTable.SearchSymbol(term.Trim(), out sym)) != true)
                {
                    Debug.LogDetailedInfo("Parse term(sym) in Expresion Handler failed");
                    rtnVal = false;
                }
                Debug.LogDetailedInfo("Parse term(sym) end");
            }

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
                Debug.LogError("Attempted to resolve invalid integer!\n\tUnable to parse integer value, skipping: \"" + currentLine + "\"" + "\n", "Resolving Expresion");
                sym = null;
                rtnVal = false;
            }
            return rtnVal;
        }
    }
}
