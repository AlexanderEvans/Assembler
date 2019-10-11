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

    //*********************************************************************
    //*** class : Program
    //*********************************************************************
    //*** DESCRIPTION :   main driver class that kicks off the assembler
    //*********************************************************************
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
            TerminalOutput.WriteLine("Loading symbols...");
            SymbolTable symbolTable = new SymbolTable();
            LiteralTable literalTable = new LiteralTable();
            symbolTable.LoadSymbols("../../../SYMBOLS.DAT");
            TerminalOutput.HoldOutput(TerminalOutput.OutputOptions.INFO);
            TerminalOutput.WriteLine("Current symbol table values after loading:");
            symbolTable.Print();
            TerminalOutput.NewLine();
            TerminalOutput.NewLine();

            string filePath;
            if (args.Length==0)
            {
                TerminalOutput.WriteLine("Please enter the search file name: ");
                filePath = Console.ReadLine();
            }
            else
            {
                filePath = args[0];
            }
            TerminalOutput.HoldOutput();
            TerminalOutput.NewLine();
            TerminalOutput.NewLine();
            //symbolTable.SearchSymbols("../../../" + filePath);
            ExpresionHandler.ParseExpresionFile(symbolTable, literalTable, "../../../" + filePath);
            TerminalOutput.HoldOutput();
            literalTable.PrintTable();
        }
    }
}
