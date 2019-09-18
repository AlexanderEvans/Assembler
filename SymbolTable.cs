﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Evans1
{
    /// <summary>
    /// A wrapper for a SortedDictionary(BST ADT) that adds helper functions for parsing and setting values for 
    /// the SymbolTable to be used by the assembler.
    /// </summary>
    class SymbolTable
    {
        //symbol value data
        public struct Symbol
        {
            //given data from file
            public string label;//the symbols's name/Identifier string/Label
            public bool RFlag;
            public int value;

            //deduced data
            public bool IFlag;
            public bool MFlag;

            public void Print()
            {
                Console.WriteLine(label + "\t" + RFlag + "\t" + value + "\t" + MFlag + "\t" + IFlag);
            }
        }

        /// <summary>
        /// Sorted dictionay is implemented with a Binary Search Tree that searches/ 
        /// inserts in (O)log(n) time.
        /// </summary>
        SortedDictionary<string, Symbol> SymbolTableBST = new SortedDictionary<string, Symbol>();

        public void LoadSymbols(string filePath)
        {
            Symbol symbol = default;
            symbol.label = "";
            char[] tmpLabel = new char[7];
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            StreamReader streamReader = new StreamReader(fileStream);

            //discard line flag
            bool discardLine;

            //get the line and trim whitespace
            string currentLine;

            string[] rflagAndValueStrings;
            string rFlag;

            while (!streamReader.EndOfStream)
            {
                //discard line flag
                discardLine = false;

                //get the line and trim whitespace
                currentLine = streamReader.ReadLine().CompactAndTrimWhitespaces();

                //check incorrect colon seperator count
                if (currentLine.CountStringCharachters(':') != 1)
                {
                    Console.WriteLine("Error Adding Symbol: Extra colon seperators in current line, skipping: \"" + currentLine + "\"");
                    discardLine = true;
                }
                if (!discardLine)
                {
                    string[] symbolSubstrings = currentLine.Split(':');

                    //validate Label
                    string tempStr = symbolSubstrings[0].Trim();
                    if (tempStr.Length > 12)
                    {
                        Console.WriteLine("Error Adding Symbol: Symbol Label(" + tempStr + ") is too long, must be less than 12 charachters in length, skipping: \"" + currentLine + "\"");
                        discardLine = true;
                    }
                    else if (tempStr.Length == 0)
                    {
                        Console.WriteLine("Error Adding Symbol: Symbol Label(" + tempStr + ") is empty, skipping: \"" + currentLine + "\"");
                        discardLine = true;
                    }
                    else if (!char.IsLetter(tempStr[0]))
                    {
                        Console.WriteLine("Error Adding Symbol: Symbol Label(" + tempStr + ") does not start with a letter(" + tempStr[0] + "), skipping: \"" + currentLine + "\"");
                        discardLine = true;
                    }
                    else//only continue validation on short Label that fit in the 12 chars
                    {
                        for (int i = 0; i < tempStr.Length && discardLine == false; i++)
                        {
                            if (!char.IsLetterOrDigit(tempStr[i]))
                            {
                                Console.WriteLine("Error Adding Symbol: invalid special charachters(" + tempStr[i] + ") detected in Symbol Label(" + tempStr + "), skipping: \"" + currentLine + "\"");
                                discardLine = true;
                            }
                        }
                    }

                    rflagAndValueStrings = symbolSubstrings[1].Trim().Split(' ');

                    //validate RFlag
                    rFlag = rflagAndValueStrings[0].Trim();
                    if (rFlag == "true" || rFlag == "1")
                    {
                        symbol.RFlag = true;
                    }
                    else if (rFlag == "false" || rFlag == "0")
                    {
                        symbol.RFlag = true;
                    }
                    else
                    {
                        discardLine = true;
                        Console.WriteLine("Error Adding Symbol: invalid RFlag value(" + rFlag + "), skipping: \"" + currentLine + "\"");
                    }

                    if (!discardLine)
                    {
                        string value = rflagAndValueStrings[1].Trim();
                        if (!int.TryParse(value, out symbol.value))
                        {
                            discardLine = true;
                            Console.WriteLine("Error Adding Symbol: invalid integer value(" + value + "), skipping: \"" + currentLine + "\"");
                        }

                        if (!discardLine)
                        {
                            //parse data into Symbol struct
                            int symbolLabelIndex = 0;
                            for (symbolLabelIndex = 0; symbolLabelIndex < 6 && symbolLabelIndex < tempStr.Length; symbolLabelIndex++)
                            {
                                tmpLabel[symbolLabelIndex] = tempStr[symbolLabelIndex];
                            }
                            tmpLabel[symbolLabelIndex] = '\0';
                            symbol.label = new string(tmpLabel);
                            symbol.MFlag = false;
                            symbol.IFlag = true;

                            if (SymbolTableBST.ContainsKey(symbol.label))
                            {
                                Console.WriteLine("Error Adding Symbol: Symbol with same Label('" + symbol.label + "') already exists!  Setting MFlag & skipping: \"" + currentLine + "\"");
                                Symbol sym = SymbolTableBST.GetValueOrDefault(symbol.label);
                                if (sym.MFlag == false)
                                {
                                    sym.MFlag = true;
                                    SymbolTableBST.Remove(symbol.label);
                                    SymbolTableBST.Add(symbol.label, sym);
                                }
                            }
                            else
                            {
                                Console.Write("Adding symbol: ");
                                symbol.Print();
                                SymbolTableBST.Add(symbol.label, symbol);
                            }
                        }
                    }
                }
            }
            streamReader.Dispose();
            fileStream.Dispose();
        }

        //Takes a string and searches for a symbol that fits that key
        Symbol? SearchSymbol(string str)
        {
            Symbol? tempN = null;
            Symbol temp;
            if(SymbolTableBST.TryGetValue(str, out temp))
            {
                tempN = temp;
            }
            return tempN;
        }

        //takes a file path and farses the file and searches the symboltable for the parsed lables, printing them to the console
        public void SearchSymbols(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            StreamReader streamReader = new StreamReader(fileStream);

            while (!streamReader.EndOfStream)
            {
                //get the line and trim whitespace
                string currentLine = streamReader.ReadLine().CompactAndTrimWhitespaces();

                if (currentLine.Length > 12)
                {
                    Console.WriteLine("Error Seeking Symbol: Symbol Label(" + currentLine + ") is too long, must be less than 12 charachters in length, skipping: \"" + currentLine + "\"");
                }
                else if (currentLine.Length == 0)
                {
                    Console.WriteLine("Error Seeking Symbol: Symbol Label(" + currentLine + ") is empty, skipping: \"" + currentLine + "\"");
                }
                else if (!char.IsLetter(currentLine[0]))
                {
                    Console.WriteLine("Error Seeking Symbol: Symbol Label(" + currentLine + ") does not start with a letter(" + currentLine[0] + "), skipping: \"" + currentLine + "\"");
                }
                else//only continue validation on short Label that fit in the 12 chars
                {
                    bool discardLine = false;
                    for (int i = 0; i < currentLine.Length && discardLine == false; i++)
                    {
                        if (!char.IsLetterOrDigit(currentLine[i]))
                        {
                            Console.WriteLine("Error Seeking Symbol: invalid special charachters('" + currentLine[i] + "') detected in Symbol Label(" + currentLine + "), skipping: \"" + currentLine + "\"");
                            discardLine = true;
                        }
                    }
                    if(discardLine==false)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < currentLine.Length && i < 6; i++)
                            stringBuilder.Append(currentLine[i]);
                        Symbol? temp = SearchSymbol(stringBuilder.ToString());
                        if(temp.HasValue)
                        {
                            Console.Write("Found symbol: ");
                            temp.Value.Print();
                        }
                        else
                        {
                            Console.WriteLine("Symbol(" + currentLine + ") not found");
                        }
                    }
                }

            }

            streamReader.Dispose();
            fileStream.Dispose();

        }

        //prints the current contents of the symbol table
        public void Print()
        {
            Console.WriteLine("Symbol\tRFlag\tValue \tMFlag \tIFlag");
            Console.WriteLine("=====================================");
            foreach (KeyValuePair<string, Symbol> keyValuePair in SymbolTableBST)
            {
                keyValuePair.Value.Print();
            }
        }
    }
}
