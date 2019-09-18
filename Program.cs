using System;
/*****************************************************************************
*** NAME : Alex Evans
*** CLASS : CSc 354 Intro to systems
*** ASSIGNMENT : 1
*** DUE DATE : 9/18/2019
*** INSTRUCTOR : GAMRADT
******************************************************************************
*** DESCRIPTION :   This program implements a symbol table to be used by an
***                 assembler                                
*****************************************************************************/
namespace Evans1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading symbols...");
            SymbolTable symbolTable = new SymbolTable();
            symbolTable.LoadSymbols("../../../SYMBOLS.DAT");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Current symbol table values after loading:");
            symbolTable.Print();
            Console.WriteLine();
            Console.WriteLine();

            string filePath;
            if (args.Length==0)
            {
                Console.WriteLine("Please enter the search file name: ");
                filePath = Console.ReadLine();
            }
            else
            {
                filePath = args[0];
            }
            Console.WriteLine();
            Console.WriteLine();
            symbolTable.SearchSymbols("../../../" + filePath);
        }
    }
}
