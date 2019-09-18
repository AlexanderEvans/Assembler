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

            Console.WriteLine("Please enter the search file path: ");
            string filePath = Console.ReadLine();
        }
    }
}
