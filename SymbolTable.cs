using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Assembler
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

            string[] flagStrings;
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
                    Console.WriteLine("Extra colon seperators in current line, skipping: \"" + currentLine + "\"");
                    discardLine = true;
                }
                if (!discardLine)
                {
                    string[] symbolSubstrings = currentLine.Split(':');

                    //validate Label
                    string tempStr = symbolSubstrings[0].Trim();
                    if (tempStr.Length > 12)
                    {
                        Console.WriteLine("Symbol Label(" + tempStr + ") is too long, must be less than 12 charachters in length, skipping: \"" + currentLine + "\"");
                        discardLine = true;
                    }
                    else if (tempStr.Length == 0)
                    {
                        Console.WriteLine("Symbol Label(" + tempStr + ") is empty, skipping: \"" + currentLine + "\"");
                        discardLine = true;
                    }
                    else if (!char.IsLetter(tempStr[0]))
                    {
                        Console.WriteLine("Symbol Label(" + tempStr + ") does not start with a letter(" + tempStr[0] + "), skipping: \"" + currentLine + "\"");
                        discardLine = true;
                    }
                    else//only continue validation on short Label that fit in the 12 chars
                    {
                        for (int i = 0; i < tempStr.Length && discardLine == false; i++)
                        {
                            if (!char.IsLetterOrDigit(tempStr[i]))
                            {
                                Console.WriteLine("invalid special charachters detected in Symbol Label(" + tempStr + "), skipping: \"" + currentLine + "\"");
                                discardLine = true;
                            }
                        }
                    }

                    flagStrings = symbolSubstrings[1].Trim().Split(' ');

                    //validate RFlag
                    rFlag = flagStrings[0].Trim();
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
                        Console.WriteLine("invalid RFlag value(" + rFlag + "), skipping: \"" + currentLine + "\"");
                    }

                    if (!discardLine)
                    {
                        string value = flagStrings[1].Trim();
                        if (!int.TryParse(value, out symbol.value))
                        {
                            discardLine = true;
                            Console.WriteLine("invalid integer value(" + value + "), skipping: \"" + currentLine + "\"");
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
                                Console.WriteLine("Symbol with same Label(" + symbol.label + ") already exists!  Setting MFlag & skipping: \"" + currentLine + "\"");
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
         
        public void Print()
        {
            Console.WriteLine("Symbol\tRFlag\tValue \tMFlag \tIFlag");
            foreach (KeyValuePair<string, Symbol> keyValuePair in SymbolTableBST)
            {
                keyValuePair.Value.Print();
            }
        }
    }
}
