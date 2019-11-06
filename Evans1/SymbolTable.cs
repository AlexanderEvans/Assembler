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
            symbol.IFlag = true;
            char[] tmpLabel = new char[7];

            //discard line flag
            bool discardLine;

            //get the line and trim whitespace
            string currentLine;

            string[] rflagAndValueStrings;
            string rFlag;

            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                {

                    try
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            while (!streamReader.EndOfStream)
                            {
                                //discard line flag
                                discardLine = false;

                                //get the line and trim whitespace
                                currentLine = streamReader.ReadLine().CompactAndTrimWhitespaces();

                                
                                if (currentLine == "")
                                {
                                    Chronicler.LogError("blank line, skipping: \"" + currentLine + "\"" + "\n", "Adding Symbol");
                                    discardLine = true;
                                }
                                else if (currentLine.CountStringCharachters(':') != 1)//check incorrect colon seperator count
                                {
                                    Chronicler.LogError("Extra colon seperators in current line, skipping: \"" + currentLine + "\"" + "\n", "Adding Symbol");
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
                                                Chronicler.LogError("Invalid integer value(" + value + "), skipping: \"" + currentLine + "\"" + "\n", "Adding Symbol");
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
                                                if (SymbolTableBST.ContainsKey(symbol.label))
                                                {
                                                    Chronicler.LogError("Symbol with same Label('" + symbol.label + "') already exists!  Setting MFlag and \n\tskipping: \"" + currentLine + "\"" + "\n", "Adding Symbol");
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
                                                    Chronicler.Write("Adding symbol: ", Chronicler.OutputOptions.INFO);
                                                    symbol.Print(Chronicler.OutputOptions.INFO);
                                                    SymbolTableBST.Add(symbol.label, symbol);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        Chronicler.LogError("failed to open File: " + filePath);
                    }
                }
            }
            catch (IOException e)
            {
                Chronicler.LogError("failed to open File: " + filePath);
            }
        }

        public bool addSymbol(string label, bool rflag, int value, string currentLine)
        {
            label = label.Trim();
            if (ValidateLabel(label, currentLine, "Pass One adding symbol"))
            {
                Globals.Symbol newSym = default;
                newSym.label = label;
                if(newSym.label.Length>6)
                    newSym.label = newSym.label.Substring(0,6);
                newSym.RFlag = rflag;
                newSym.value = value;
                newSym.MFlag = false;
                if (SymbolTableBST.ContainsKey(newSym.label))
                {
                    Chronicler.LogError("Symbol with same Label('" + newSym.label + "') already exists!  Setting MFlag and \n\tskipping: \"" + currentLine + "\"" + "\n", "Adding Symbol");
                    Globals.Symbol sym = SymbolTableBST.GetValueOrDefault(newSym.label);
                    if (sym.MFlag == false)
                    {
                        sym.MFlag = true;
                        SymbolTableBST.Remove(newSym.label);
                        SymbolTableBST.Add(newSym.label, sym);
                    }
                    return false;
                }
                else
                {
                    Chronicler.Write("Adding symbol: ", Chronicler.OutputOptions.INFO);
                    newSym.Print(Chronicler.OutputOptions.INFO);
                    SymbolTableBST.Add(newSym.label, newSym);
                }
            }
            else
                return false;
            return true;
        }

        //******************************************************************************
        //***  FUNCTION TestAndSetRFlag 
        //*** **************************************************************************
        //***  DESCRIPTION  :  maps rflag string vals to boolean vals
        //***  INPUT ARGS   :  string rFlagIn, string currentLine,
        //***                   string errorPrefix = ""
        //***  OUTPUT ARGS :  bool rFlagOut
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool discardLine
        //******************************************************************************
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
                Chronicler.LogError("Invalid RFlag value(" + rFlagIn + "), skipping: \"" + currentLine + "\"" + "\n", errorPrefix);
            }
            return discardLine;
        }

        //******************************************************************************
        //***  FUNCTION ValidateLabel 
        //*** **************************************************************************
        //***  DESCRIPTION  :  Takes a string and ensures the string is a valid
        //***                   symbol label
        //***  INPUT ARGS   :  string label, string currentLine, string errorPrefix=""
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  bool isValid
        //******************************************************************************
        public static bool ValidateLabel(string label, string currentLine, string errorPrefix="")
        {
            bool isValid = true;
            if (label.Length > 12)
            {
                Chronicler.LogError("Symbol Label(" + label + ") is too long, \n\tmust be less than 12 charachters in length, skipping: \"" + currentLine + "\"" + "\n", errorPrefix);
                isValid = false;
            }
            else if (label.Length == 0)
            {
                Chronicler.LogError("Symbol Label(" + label + ") is empty, skipping: \"" + currentLine + "\"" + "\n", errorPrefix);
                isValid = false;
            }
            else if (!char.IsLetter(label[0]))
            {
                Chronicler.LogError("Symbol Label(" + label + ") does not start with a letter(" + label[0] + "), \n\tskipping: \"" + currentLine + "\"" + "\n", errorPrefix);
                isValid = false;
            }
            else//only continue validation on short Label that fit in the 12 chars
            {
                for (int i = 0; i < label.Length && isValid == true; i++)
                {
                    if (!char.IsLetterOrDigit(label[i]))
                    {
                        Chronicler.LogError("Invalid special charachters('" + label[i] + "') detected in Symbol Label(" + label + "), \n\tskipping: \"" + currentLine + "\"" + "\n", errorPrefix);
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
        public bool SearchSymbol(string symbolLabel, string currentLine, out Globals.Symbol? tempN)
        {
            bool rtnVal = true;
            tempN = null;
            symbolLabel = symbolLabel.Trim();

            if (ValidateLabel(symbolLabel, currentLine, "Searching symbol"))
            {
                if (symbolLabel.Length > 6)
                    symbolLabel = symbolLabel.Substring(0, 6);
            }
            else
                return false;

            Globals.Symbol temp;
            if (SymbolTableBST.TryGetValue(symbolLabel, out temp))
            {
                tempN = temp;
            }
            else
            {
                Chronicler.LogError("Symbol(" + symbolLabel + ") not found", "Searching for symbol");
                rtnVal = false;
            }
            return rtnVal;
        }

        ////************************************************************************
        ////***  FUNCTION SearchSymbol 
        ////*** ********************************************************************
        ////***  DESCRIPTION  :  Takes a file path to parse and validate symbol 
        ////***                   labels which are then search for in the BST and 
        ////***                   the output is dumped to the console
        ////***  INPUT ARGS   :  string filePath
        ////***  OUTPUT ARGS :  N/A
        ////***  IN/OUT ARGS   :  N/A  
        ////***  RETURN :  N/A
        ////************************************************************************
        //public void SearchSymbols(string filePath)
        //{
        //    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan);
        //    StreamReader streamReader = new StreamReader(fileStream);

        //    while (!streamReader.EndOfStream)
        //    {
        //        //get the line and trim whitespace
        //        string currentLine = streamReader.ReadLine().CompactAndTrimWhitespaces();

        //        bool discardLine = false;
        //        if (currentLine == "")
        //        {
        //            Chronicler.LogError("blank line, skipping: \"" + currentLine + "\"" + "\n", "Adding Symbol");
        //            discardLine = true;
        //        }

        //        if (discardLine !=true && ValidateLabel(currentLine, currentLine, "Seeking Symbol"))
        //        {
        //            StringBuilder stringBuilder = new StringBuilder();
        //            for (int i = 0; i < currentLine.Length && i < 6; i++)
        //                stringBuilder.Append(currentLine[i]);
                        
        //            if (SearchSymbol(stringBuilder.ToString(), out Globals.Symbol? temp))
        //            {
        //                Chronicler.Write("Found symbol: ");
        //                temp.Value.Print(Chronicler.OutputOptions.IGNORE);
        //            }
        //            else
        //            {
        //                Chronicler.WriteLine("Symbol(" + currentLine + ") not found");
        //            }
        //        }
        //    }
        //    streamReader.Dispose();
        //    fileStream.Dispose();
        //}

        //************************************************************************
        //***  FUNCTION Print 
        //*** ********************************************************************
        //***  DESCRIPTION  :  prints the current contents of the symbol table
        //***  INPUT ARGS   :  N/A 
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //************************************************************************
        public void Print(Chronicler.OutputOptions outputOptions = Chronicler.OutputOptions.IGNORE)
        {
            Chronicler.WriteLine("Symbol\tRFlag\tValue \tMFlag \tIFlag", outputOptions);
            Chronicler.WriteLine("=====================================", outputOptions);
            foreach (KeyValuePair<string, Globals.Symbol> keyValuePair in SymbolTableBST)
            {
                keyValuePair.Value.Print(outputOptions);
            }
        }
    }
}
