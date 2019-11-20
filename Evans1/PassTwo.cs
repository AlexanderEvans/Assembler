using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    struct PassTwo
    {
        public static void Execute(Globals.DataStructures dataStructures, List<PassOne.ExpresionLine> expresionLines, string filePath)
        {

            DumpObjFile(filePath, expresionLines, dataStructures);
        }
        static void DumpObjFile(string filePath, List<PassOne.ExpresionLine> expresionLines, Globals.DataStructures dataStructures)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + ".txt"))
            {
                Chronicler.HoldOutput();
                int curLinNum = 0;
                foreach (PassOne.ExpresionLine expLine in expresionLines)
                {
                    if (expLine != null && expLine.OriginalLine != null && expLine.OriginalLine != "")
                    {
                        curLinNum = expLine.lineNumber;

                        file.Write(curLinNum.ToString() + "\t");
                        if (expLine.CommentLine != true)
                            file.Write(expLine.locationCounter.ToString("X6") + "\t");
                        file.Write(expLine.OriginalLine);
                        file.Write(expLine.Opcode);
                        file.Write("\n");

                        Chronicler.Write(curLinNum.ToString() + "\t");
                        if (expLine.CommentLine != true)
                            Chronicler.Write(expLine.locationCounter.ToString("X6") + "\t");
                        Chronicler.Write(expLine.OriginalLine);
                        Chronicler.WriteLine(expLine.Opcode);
                    }
                }
                foreach (LiteralTable.LiteralValue lv in dataStructures.literalTable.literalTable)
                {
                    curLinNum++;
                    file.Write(curLinNum.ToString() + "\t");
                    file.Write(lv.address.ToString("X6") + "\t");
                    file.Write("*" + "\t");
                    file.Write(lv.label);
                    file.Write("\n");

                    Chronicler.Write(curLinNum.ToString() + "\t");
                    Chronicler.Write(lv.address.ToString("X6") + "\t");
                    Chronicler.Write("*" + "\t");
                    Chronicler.WriteLine(lv.label);
                }
            }
            Chronicler.HoldOutput();
            dataStructures.symbolTable.Print();
            Chronicler.HoldOutput();
            dataStructures.literalTable.PrintTable();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + ".obj"))
            {
                Chronicler.HoldOutput();
                foreach (PassOne.ExpresionLine expLine in expresionLines)
                {
                    if (expLine != null && expLine.OriginalLine != null && expLine.OriginalLine != "")
                    {
                        file.Write("T" + expLine.Opcode);
                        file.Write("\n");

                        Chronicler.WriteLine(expLine.Opcode);
                    }
                }
                foreach (LiteralTable.LiteralValue lv in dataStructures.literalTable.literalTable)
                {
                    file.Write("T"+ lv.hexValue);
                    file.Write("\n");

                    Chronicler.WriteLine("T" + lv.hexValue);
                }
            }
        }
    }
}
