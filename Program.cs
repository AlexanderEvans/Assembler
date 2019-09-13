using System;

namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            SymbolTable symbolTable = new SymbolTable();


            Console.WriteLine("Please enter a file path: ");
            string filePath = Console.ReadLine();
        }
    }
}
