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
            public char [] symbol;//the symbols's name/Identifier string/Label
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
            Symbol symbol;
            symbol.symbol = new char[7];
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            StreamReader streamReader = new StreamReader(fileStream);

            while(!streamReader.EndOfStream)
            {
                //discard line flag
                bool discardLine = false;

                //get the line and trim whitespace
                string currentLine = streamReader.ReadLine().Trim();

                //check incorrect colon seperator count
                if (countStringCharachter(currentLine, ':') != 1)
                {
                    Console.WriteLine("Extra colon seperator's in current line, skipping: \"" + currentLine + "\"\n");
                    discardLine = true;
                }
                if(!discardLine)
                {
                    string[] parts = currentLine.Split(':');
                    string tempStr = parts[0].Trim();

                    //validate Symbol
                    foreach (char c in tempStr)
                    {
                        if (!char.IsLetterOrDigit(c))
                        {
                            Console.WriteLine("invalid special charachters detected in symbol, skipping: \"" + currentLine + "\"\n");
                            discardLine = true;
                        }
                    }

                    if(!discardLine)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < 6 && i < tempStr.Length; i++)
                        {
                            stringBuilder.Append(tempStr[i]);
                        }
                        symbol.symbol = stringBuilder.ToString().ToCharArray();
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
