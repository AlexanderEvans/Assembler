using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    //*****************************************************************************
     //*** NAME : Alex Evans
     //*** CLASS : CSc 354 Intro to systems
     //*** ASSIGNMENT : 2
     //*** DUE DATE : 10/9/2019
     //*** INSTRUCTOR : GAMRADT
     //*****************************************************************************
     //*** DESCRIPTION :   This class handles holding output and output filters
     //*****************************************************************************
    class Debug
    {
        private static int count = 0;
        public static int LinesBeforeHolding = 20;
        public enum outputOptions : byte
        {
            ERR     =   0b1000,
            WARN    =   0b100,
            INFO    =   0b10,
            DETAIL  =   0b1,
        }
        public static int outputMask = (int)(outputOptions.ERR | outputOptions.WARN);

        //*************************************************************************
        //***  FUNCTION HoldOutput 
        //*** *********************************************************************
        //***  DESCRIPTION  :  Hold the terminal for input
        //***  INPUT ARGS   :  N/A
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        public static void HoldOutput()
        {
            count = 1;
            Console.WriteLine("Holding Input: Press any key to continue...");
            Console.ReadKey();
            Console.WriteLine();
        }

        //*************************************************************************
        //***  FUNCTION Write 
        //*** *********************************************************************
        //***  DESCRIPTION  :  prints msg
        //***  INPUT ARGS   :  string msg
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        public static void Write(string msg)
        {
            if(msg.CountStringCharachters('\n')>0)
            {
                string[] arr = msg.Split('\n');
                foreach (string str in arr)
                {
                    Console.Write(str);
                    if (arr.Length > 1)
                    {
                        Console.WriteLine();
                        count++;
                        if (count % LinesBeforeHolding == 0)
                        {
                            Console.WriteLine("Holding Input: Press any key to continue...");
                            Console.ReadKey();
                            Console.WriteLine();
                        }
                    }
                }
            }
            else
                Console.Write(msg);
        }
        //*************************************************************************
        //***  FUNCTION WriteLine 
        //*** *********************************************************************
        //***  DESCRIPTION  :  prints msg with endline
        //***  INPUT ARGS   :  string msg
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        public static void WriteLine(string msg)
        {
            string[] arr = msg.Split('\n');
            foreach (string str in arr)
                PrintLine(str);
        }

        //*************************************************************************
        //***  FUNCTION NewLine 
        //*** *********************************************************************
        //***  DESCRIPTION  :  prints endline
        //***  INPUT ARGS   :  N/A
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        public static void NewLine()
        {
            PrintLine("");
        }

        //*************************************************************************
        //***  FUNCTION PrintLine 
        //*** *********************************************************************
        //***  DESCRIPTION  :  prints msg with endline
        //***  INPUT ARGS   :  string msg
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        private static void PrintLine(string msg)
        {
            count++;
            if (count % LinesBeforeHolding == 0)
            {
                Console.WriteLine("Holding Input: Press any key to continue...");
                Console.ReadKey();
                Console.WriteLine();
            }
            Console.WriteLine(msg);
        }

        //*************************************************************************
        //***  FUNCTION LogInfo 
        //*** *********************************************************************
        //***  DESCRIPTION  :  logs msg with endline
        //***  INPUT ARGS   :  string msg, string loc=""
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        public static void LogInfo(string msg, string loc="")
        {
            int flags = (int)outputOptions.INFO;
            if((flags & outputMask)>0)
            {
                if (loc == "")
                    WriteLine("Info: " + msg);
                else
                    WriteLine("Info(" + loc + "): " + msg);
            }
        }

        //*************************************************************************
        //***  FUNCTION LogDetailedInfo 
        //*** *********************************************************************
        //***  DESCRIPTION  :  Log Detailed Info msg with endline
        //***  INPUT ARGS   :  string msg, string loc=""
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        public static void LogDetailedInfo(string msg, string loc="")
        {
            int flags = (int)outputOptions.DETAIL;
            if ((flags & outputMask) > 0)
            {
                if (loc == "")
                    WriteLine("Detail: " + msg);
                else
                    WriteLine("Detail(" + loc + "): " + msg);
            }
        }
        //*************************************************************************
        //***  FUNCTION LogWarn 
        //*** *********************************************************************
        //***  DESCRIPTION  :  Log warning msg with endline
        //***  INPUT ARGS   :  string msg, string loc=""
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        public static void LogWarn(string msg, string loc="")
        {
            int flags = (int)outputOptions.WARN;
            if ((flags & outputMask) > 0)
            {
                if (loc == "")
                    WriteLine("Warning: " + msg);
                else
                    WriteLine("Warning(" + loc + "): " + msg);
            }
        }



        //*************************************************************************
        //***  FUNCTION LogError 
        //*** *********************************************************************
        //***  DESCRIPTION  :  Log error msg with endline
        //***  INPUT ARGS   :  string msg, string loc=""
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        public static void LogError(string msg, string loc="")
        {
            int flags = (int)outputOptions.ERR;
            if ((flags & outputMask) > 0)
            {
                if (loc == "")
                    WriteLine("Error: " + msg);
                else
                    WriteLine("Error(" + loc + "): " + msg);
            }
        }

    }
}
