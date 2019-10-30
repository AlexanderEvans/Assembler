﻿using System;
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
            Chronicler.WriteLine("Loading symbols...");
            Globals.DataStructures dataStructures = new Globals.DataStructures();
            //dataStructures.symbolTable.LoadSymbols("../../../SYMBOLS.DAT");
            Chronicler.HoldOutput(Chronicler.OutputOptions.INFO);
            Chronicler.WriteLine("Current symbol table values after loading:");
            dataStructures.symbolTable.Print();
            Chronicler.NewLine();
            Chronicler.NewLine();

            string filePath;
            if (args.Length==0)
            {
                Chronicler.WriteLine("Please enter the search file name: ");
                filePath = Console.ReadLine();
            }
            else
            {
                filePath = args[0];
            }
            Chronicler.HoldOutput();
            Chronicler.NewLine();
            Chronicler.NewLine();
            //symbolTable.SearchSymbols("../../../" + filePath);
            Chronicler.HoldOutput();
            dataStructures.literalTable.PrintTable();
        }
    }
}
