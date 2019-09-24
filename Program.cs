using System;
//*****************************************************************************
//*** NAME : Alex Evans
//*** CLASS : CSc 354 Intro to systems
//*** ASSIGNMENT : 1
//*** DUE DATE : 9/18/2019
//*** INSTRUCTOR : GAMRADT
//*****************************************************************************
//*** DESCRIPTION :   This program implements a symbol table to be used by an
//***                 assembler                                
//*****************************************************************************
namespace Evans1
{
    class Program
    {
        //*************************************************************************
        //***  FUNCTION Main 
        //*** *********************************************************************
        //***  DESCRIPTION  :  This is the main program driver
        //***  INPUT ARGS   :  string[] args
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        static void Main(string[] args)
        {
            Debug.WriteLine("Loading symbols...");
            SymbolTable symbolTable = new SymbolTable();
            symbolTable.LoadSymbols("../../../SYMBOLS.DAT");
            Debug.NewLine();
            Debug.NewLine();
            Debug.WriteLine("Current symbol table values after loading:");
            symbolTable.Print();
            Debug.NewLine();
            Debug.NewLine();

            string filePath;
            if (args.Length==0)
            {
                Debug.WriteLine("Please enter the search file name: ");
                filePath = Console.ReadLine();
            }
            else
            {
                filePath = args[0];
            }
            Debug.NewLine();
            Debug.NewLine();
            symbolTable.SearchSymbols("../../../" + filePath);
        }
    }
}
