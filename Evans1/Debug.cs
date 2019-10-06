﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    class Debug
    {
        private static int count = 0;
        public static int LinesBeforeHolding = 20;
        public enum outputOptions
        {
            ERR     =   0b1000,
            WARN    =   0b100,
            DETAIL  =   0b10,
            INFO    =   0b1
        }
        public static int outputMask = (int)(outputOptions.ERR | outputOptions.WARN | outputOptions.INFO);
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
        public static void WriteLine(string msg)
        {
            string[] arr = msg.Split('\n');
            foreach (string str in arr)
                PrintLine(str);
        }

        public static void NewLine()
        {
            PrintLine("");
        }

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