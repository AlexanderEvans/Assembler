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
    public class Chronicler
    {
        private static int count = 1;
        public static int LinesBeforeHolding = 20;
        public enum OutputOptions : byte
        {
            ERR     =   0b1000,
            WARN    =   0b100,
            INFO    =   0b10,
            DETAIL  =   0b1,
            IGNORE  =   0b0,
        }
        public static int outputMask = (int)(OutputOptions.ERR | OutputOptions.WARN | OutputOptions.INFO);

        //*************************************************************************
        //***  FUNCTION HoldOutput 
        //*** *********************************************************************
        //***  DESCRIPTION  :  Hold the terminal for input
        //***  INPUT ARGS   :  N/A
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN :  N/A
        //*************************************************************************
        public static void HoldOutput(OutputOptions outputOptions = OutputOptions.IGNORE)
        {
            if(checkMask(outputOptions))
            {
                count = 1;
                Console.WriteLine("Holding Output: Press any key to continue...");
                Console.ReadKey();
                Console.WriteLine();
            }
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
        public static void Write(string msg, OutputOptions outputOptions = OutputOptions.IGNORE)
        {
            if (checkMask(outputOptions))
            {
                if (msg.CountStringCharachters('\n') > 0)
                {
                    string[] arr = msg.Split('\n');
                    StringBuilder stringBuilder = new StringBuilder("");
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i != arr.Length - 1)
                        {
                            stringBuilder.Append(arr[i]);
                            stringBuilder.Append('\n');
                            if (count % LinesBeforeHolding == 0)
                            {
                                Console.Write(stringBuilder);
                                HoldOutput();
                                stringBuilder = new StringBuilder("");
                            }
                            count++;
                        }
                        else
                            stringBuilder.Append(arr[i]);
                    }
                    Console.Write(stringBuilder);
                }
                else
                    Console.Write(msg);
            }
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
        public static void WriteLine(string msg, OutputOptions outputOptions = OutputOptions.IGNORE)
        {
            if(checkMask(outputOptions))
                Write(msg + '\n');
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
        public static void NewLine(OutputOptions outputOptions = OutputOptions.IGNORE)
        {
            if (checkMask(outputOptions))
                WriteLine("");
        }

        //*************************************************************************
        //***  FUNCTION checkMask 
        //*** *********************************************************************
        //***  DESCRIPTION  :  Determines if we are in a matching print mode
        //***  INPUT ARGS   :  OutputOptions outputOptions
        //***  OUTPUT ARGS :  N/A
        //***  IN/OUT ARGS   :  N/A  
        //***  RETURN : bool rtnVal
        //*************************************************************************
        static bool checkMask(OutputOptions outputOptions)
        {
            bool rtnVal = false;
            if ((((int)outputOptions & outputMask) != 0) || outputOptions == OutputOptions.IGNORE)
                rtnVal = true;
            return rtnVal;
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
            if(checkMask(OutputOptions.INFO))
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
            if (checkMask(OutputOptions.DETAIL))
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
            if (checkMask(OutputOptions.WARN))
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
            if (checkMask(OutputOptions.ERR))
            {
                if (loc == "")
                    WriteLine("Error: " + msg);
                else
                    WriteLine("Error(" + loc + "): " + msg);
            }
        }

    }
}
