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
            public char [] label;//the symbols's name/Identifier string/Label
            public bool RFlag;
            public int value;

            //deduced data
            public bool IFlag;
            public bool MFlag;
        }

        /// <summary>
        /// Sorted dictionay is implemented with a Binary Search Tree that searches/ 
        /// inserts in (O)log(n) time.
        /// </summary>
        SortedDictionary<char[], Symbol> SymbolTableBST = new SortedDictionary<char[], Symbol>();

        void LoadSymbols(string filePath)
        {
            Symbol symbol = default;
            symbol.label = new char[7];
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
                currentLine = streamReader.ReadLine().Trim();

                //check incorrect colon seperator count
                if (countStringCharachter(currentLine, ':') != 1)
                {
                    Console.WriteLine("Extra colon seperators in current line, skipping: \"" + currentLine + "\"\n");
                    discardLine = true;
                }
                if(!discardLine)
                {
                    string[] symbolSubstrings = currentLine.Split(':');

                    //validate Label
                    string tempStr = symbolSubstrings[0].Trim();
                    if (tempStr.Length > 12)
                    {
                        Console.WriteLine("Symbol Label(" + tempStr + ") is too long, must be less than 12 charachters in length, skipping: \"" + currentLine + "\"\n");
                        discardLine = true;
                    }
                    else//only continue validation on short Label that fit in the 12 chars
                    {
                        foreach (char c in tempStr)
                        {
                            if (!char.IsLetterOrDigit(c))
                            {
                                Console.WriteLine("invalid special charachters detected in Symbol Label("+ tempStr + "), skipping: \"" + currentLine + "\"\n");
                                discardLine = true;
                            }
                        }
                    }

                    flagStrings = symbolSubstrings[1].Trim().Split(' ');

                    //validate RFlag
                    rFlag = flagStrings[0].Trim();
                    if(rFlag == "true" || rFlag == "1")
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
                        Console.WriteLine("invalid RFlag value("+rFlag+"), skipping: \"" + currentLine + "\"\n");
                    }

                    if(!discardLine)
                    {
                        string value = flagStrings[1].Trim();
                        if (!int.TryParse(value, out symbol.value))
                        {
                            discardLine = true;
                            Console.WriteLine("invalid integer value(" + value + "), skipping: \"" + currentLine + "\"\n");
                        }

                        if (!discardLine)
                        {
                            //parse data into Symbol struct
                            int symbolLabelIndex = 0;
                            for (symbolLabelIndex = 0; symbolLabelIndex < 6 && symbolLabelIndex < tempStr.Length; symbolLabelIndex++)
                            {
                                symbol.label[symbolLabelIndex] = tempStr[symbolLabelIndex];
                            }
                            symbol.label[symbolLabelIndex] = '\0';
                            symbol.MFlag = false;
                            symbol.IFlag = false;

                            if(!SymbolTableBST.TryAdd(symbol.label, symbol))
                            {
                                Console.WriteLine("Symbol with same Label(" + symbol.label + ") already exists, skipping: \"" + currentLine + "\"\n");
                            }
                        }
                    }
                }
            }
        }

        int countStringCharachter(string str, char myChar)
        {
            int count = 0;
            foreach(char c in str)
            {
                if (c == myChar)
                    count++;
            }
            return count;
        }
    }
}
