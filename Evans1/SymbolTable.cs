using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Evans1
{
    //*********************************************************************
    //*** NAME : Alex Evans
    //*** CLASS : CSc 354 Intro to systems
    //*** ASSIGNMENT : 1
    //*** DUE DATE : 9/18/2019
    //*** INSTRUCTOR : GAMRADT 
    //*********************************************************************
    //*** DESCRIPTION :   A wrapper for a SortedDictionary(BST ADT) that 
    //***                   adds helper functions for parsing and setting 
    //**                    values for the SymbolTable to be used by the 
    //**                    assembler.
    //*********************************************************************
    class SymbolTable
    {
        /// <summary>
        /// Sorted dictionay is implemented with a Binary Search Tree that searches/ 
        /// inserts in (O)log(n) time.
        /// </summary>
        SortedDictionary<string, Globals.Symbol> SymbolTableBST = new SortedDictionary<string, Globals.Symbol>(new AsciiComparer());

        //************************************************************************
        //***  FUNCTION LoadSymbols 
        //*** ********************************************************************
        //***  DESCRIPTION  :  opens a file and populates SymbolTableBST
        //***  INPUT ARGS   :  string filePath
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A 
        //************************************************************************
        public void LoadSymbols(string filePath)
        {
            Globals.Symbol symbol = default;
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
                    Debug.LogError("Extra colon seperators in current line, skipping: \"" + currentLine + "\"", "Adding Symbol");
                    discardLine = true;
                }
                if (!discardLine)
                {
                    string[] symbolSubstrings = currentLine.Split(':');

                    //validate Label
                    string label = symbolSubstrings[0].Trim();
                    discardLine = !ValidateLabel(label, currentLine, "Adding Symbol");

                    if (!discardLine)
                    {
                        rflagAndValueStrings = symbolSubstrings[1].Trim().Split(' ');

                        //validate RFlag
                        rFlag = rflagAndValueStrings[0].Trim();
                        discardLine = TestAndSetRFlag(rFlag, out symbol.RFlag, currentLine, "Adding Symbol");
                        if (!discardLine)
                        {
                            string value = rflagAndValueStrings[1].Trim();
                            if (!int.TryParse(value, out symbol.value) || value[0] == '+')
                            {
                                discardLine = true;
                                Debug.LogError("Invalid integer value(" + value + "), skipping: \"" + currentLine + "\"", "Adding Symbol");
                            }
                            if (!discardLine)
                            {
                                //parse data into Symbol struct
                                int symbolLabelIndex = 0;
                                for (symbolLabelIndex = 0; symbolLabelIndex < 6 && symbolLabelIndex < label.Length; symbolLabelIndex++)
                                {
                                    tmpLabel[symbolLabelIndex] = label[symbolLabelIndex];
                                }
                                tmpLabel[symbolLabelIndex] = '\0';
                                StringBuilder sb = new StringBuilder("");
                                foreach (char c in tmpLabel.TakeWhile(c => c != '\0'))
                                {
                                    sb.Append(c);
                                }
                                symbol.label = sb.ToString().Trim();
                                symbol.MFlag = false;
                                symbol.IFlag = true;
                                if (SymbolTableBST.ContainsKey(symbol.label))
                                {
                                    Debug.LogError("Symbol with same Label('" + symbol.label + "') already exists!  Setting MFlag and \n\tskipping: \"" + currentLine + "\"", "Adding Symbol");
                                    Globals.Symbol sym = SymbolTableBST.GetValueOrDefault(symbol.label);
                                    if (sym.MFlag == false)
                                    {
                                        sym.MFlag = true;
                                        SymbolTableBST.Remove(symbol.label);
                                        SymbolTableBST.Add(symbol.label, sym);
                                    }
                                }
                                else
                                {
                                    Debug.Write("Adding symbol: ");
                                    symbol.Print();
                                    SymbolTableBST.Add(symbol.label, symbol);
                                }
                            }
                        }
                    }
                }
            }
            streamReader.Dispose();
            fileStream.Dispose();
        }

        bool TestAndSetRFlag(string rFlagIn, out bool rFlagOut, string currentLine, string errorPrefix = "")
        {
            bool discardLine = false;
            if (rFlagIn == "true" || rFlagIn == "1")
            {
                rFlagOut = true;
            }
            else if (rFlagIn == "false" || rFlagIn == "0")
            {
                rFlagOut = false;
            }
            else
            {
                rFlagOut = default;
                discardLine = true;
                Debug.LogError("Invalid RFlag value(" + rFlagIn + "), skipping: \"" + currentLine + "\"", errorPrefix);
            }
            return discardLine;
        }

        public bool ValidateLabel(string label, string currentLine, string errorPrefix="")
        {
            bool isValid = true;
            if (label.Length > 12)
            {
                Debug.LogError("Symbol Label(" + label + ") is too long, \n\tmust be less than 12 charachters in length, skipping: \"" + currentLine + "\"", errorPrefix);
                isValid = false;
            }
            else if (label.Length == 0)
            {
                Debug.LogError("Symbol Label(" + label + ") is empty, skipping: \"" + currentLine + "\"", errorPrefix);
                isValid = false;
            }
            else if (!char.IsLetter(label[0]))
            {
                Debug.LogError("Symbol Label(" + label + ") does not start with a letter(" + label[0] + "), \n\tskipping: \"" + currentLine + "\"", errorPrefix);
                isValid = false;
            }
            else//only continue validation on short Label that fit in the 12 chars
            {
                for (int i = 0; i < label.Length && isValid == false; i++)
                {
                    if (!char.IsLetterOrDigit(label[i]))
                    {
                        Debug.LogError("Invalid special charachters('" + label[i] + "') detected in Symbol Label(" + label + "), \n\tskipping: \"" + currentLine + "\"", errorPrefix);
                        isValid = false;
                    }
                }
            }
            return isValid;
        }

        //************************************************************************
        //***  FUNCTION SearchSymbol 
        //*** ********************************************************************
        //***  DESCRIPTION  :  Takes a string and searches for a symbol 
        //***                   that fits that key
        //***  INPUT ARGS   :  string str
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  Symbol? 
        //************************************************************************
        public Globals.Symbol? SearchSymbol(string str)
        {
            Globals.Symbol? tempN = null;
            Globals.Symbol temp;
            if (SymbolTableBST.TryGetValue(str, out temp))
            {
                tempN = temp;
            }
            return tempN;
        }

        //************************************************************************
        //***  FUNCTION SearchSymbol 
        //*** ********************************************************************
        //***  DESCRIPTION  :  Takes a file path to parse and validate symbol 
        //***                   labels which are then search for in the BST and 
        //***                   the output is dumped to the console
        //***  INPUT ARGS   :  string filePath
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //************************************************************************
        public void SearchSymbols(string filePath)
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
            StreamReader streamReader = new StreamReader(fileStream);

            while (!streamReader.EndOfStream)
            {
                //get the line and trim whitespace
                string currentLine = streamReader.ReadLine().CompactAndTrimWhitespaces();

                if (ValidateLabel(currentLine, currentLine, "Seeking Symbol"))
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < currentLine.Length && i < 6; i++)
                        stringBuilder.Append(currentLine[i]);
                    Globals.Symbol? temp = SearchSymbol(stringBuilder.ToString());
                    if (temp.HasValue)
                    {
                        Debug.Write("Found symbol: ");
                        temp.Value.Print();
                    }
                    else
                    {
                        Debug.WriteLine("Symbol(" + currentLine + ") not found");
                    }
                }

            }

            streamReader.Dispose();
            fileStream.Dispose();

        }

        //************************************************************************
        //***  FUNCTION Print 
        //*** ********************************************************************
        //***  DESCRIPTION  :  prints the current contents of the symbol table
        //***  INPUT ARGS   :  N/A 
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //************************************************************************
        public void Print()
        {
            Debug.WriteLine("Symbol\tRFlag\tValue \tMFlag \tIFlag");
            Debug.WriteLine("=====================================");
            foreach (KeyValuePair<string, Globals.Symbol> keyValuePair in SymbolTableBST)
            {
                keyValuePair.Value.Print();
            }
        }
    }
}
