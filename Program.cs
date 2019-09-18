using System;

namespace Assembler
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


            Console.WriteLine(args.Length);
            foreach (string arg in args)
                Console.WriteLine(arg);
            string filePath;
            if (args.Length==0)
            {
                Console.WriteLine("Please enter the search file path: ");
                filePath = Console.ReadLine();
            }
            else
            {
                filePath = args[0];
            }
            symbolTable.SearchSymbols(filePath);
        }
    }
}
