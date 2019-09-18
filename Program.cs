using System;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            SymbolTable symbolTable = new SymbolTable();
            symbolTable.LoadSymbols("../../../SYMBOLS.DAT");
            symbolTable.Print();
            Console.WriteLine();

            Console.WriteLine("Please enter a file path: ");
            string filePath = Console.ReadLine();
        }
    }
}
