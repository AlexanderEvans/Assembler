using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    struct PassTwo
    {
        public static void Execute(Globals.DataStructures dataStructures, List<PassOne.ExpresionLine> expresionLines, string filePath)
        {
            Chronicler.Write("\nPassTwo\n");
            foreach(PassOne.ExpresionLine expresionLine in expresionLines)
            {
                //evaluate expresion
                if(expresionLine.DeferExpresionResolutiontoPass2 && (expresionLine.instructionFormat == 3 || expresionLine.instructionFormat == 4))
                {
                    if (ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, expresionLine.operandFieldAndComment, out expresionLine.expresionData) != true)
                    {
                        expresionLine.validLine = false;
                    }
                }

                //convert expresion format
                if(expresionLine.Opcode=="" && expresionLine.IsEQU!=true && (expresionLine.instructionFormat == 3 || expresionLine.instructionFormat == 4))//don't redo work
                {
                    switch (expresionLine.expresionData.ExpresionType)
                    {
                        case Globals.ExpresionData.Contents.SYMBOL:
                            int? value=null;
                            if (expresionLine.expresionData.second.HasValue)
                            {
                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.SUBTRACT)
                                    value = (expresionLine.expresionData.first.Value.value) - expresionLine.expresionData.second.Value.value;
                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.ADD)
                                    value = (expresionLine.expresionData.first.Value.value) + expresionLine.expresionData.second.Value.value;
                            }
                            if(value.HasValue && expresionLine.instructionFormat==4)
                                expresionLine.Opcode = value.Value.ToString("X5");
                            else if (value.HasValue && expresionLine.instructionFormat == 3)
                                expresionLine.Opcode = value.Value.ToString("X3");
                            break;
                        case Globals.ExpresionData.Contents.ERROR:
                            Chronicler.WriteLine("This is wrong for some reason, you should look at it:\n" + expresionLine.OriginalLine, Chronicler.OutputOptions.ERR);
                            break;
                        case Globals.ExpresionData.Contents.LITERAL:
                            if(expresionLine.expresionData.literal.isOldStyleLiteral!=true)
                            {
                                expresionLine.Opcode = expresionLine.locationCounter.ToString("X6");
                            }
                            break;
                        case Globals.ExpresionData.Contents.EMPTY:
                            //consider break
                            break;
                    }
                }

                if(expresionLine.operationData.HasValue && expresionLine.operationData.Value.format==2)
                {
                    string[] tmp = expresionLine.operandFieldAndComment.Split(";", StringSplitOptions.RemoveEmptyEntries);
                    if (tmp.Length > 0)
                        expresionLine.operandFieldAndComment = tmp[0];
                    string[] subStrs = expresionLine.operandFieldAndComment.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if(subStrs.Length==1)
                    {
                        int val;
                        if(dataStructures.opcodeTable.RegisterTable.TryGetValue(subStrs[0].Trim(), out val))
                        {
                            expresionLine.Opcode = val.ToString();
                        }
                        else
                            Chronicler.LogError("failed parsing F2: " + expresionLine.OriginalLine);
                    }
                    else if (subStrs.Length == 2)
                    {
                        int val;
                        if (dataStructures.opcodeTable.RegisterTable.TryGetValue(subStrs[0].Trim(), out val))
                        {
                            expresionLine.Opcode = val.ToString()+"0";
                        }
                        else
                            Chronicler.LogError("failed parsing F2: " + expresionLine.OriginalLine);
                        if (dataStructures.opcodeTable.RegisterTable.TryGetValue(subStrs[1].Trim(), out val))
                        {
                            expresionLine.Opcode += val.ToString();
                        }
                        else
                            Chronicler.LogError("failed parsing F2: " + expresionLine.OriginalLine);
                    }
                    else
                        Chronicler.LogError("failed parsing F2, incorect term count: " + expresionLine.OriginalLine);
                }

                if (expresionLine.operationData.HasValue)
                    expresionLine.Opcode = expresionLine.operationData.Value.code.ToString("X") + expresionLine.Opcode;
            }


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
                        file.Write(expLine.OriginalLine + "\t");
                        file.Write(expLine.Opcode);
                        file.Write("\n");

                        Chronicler.Write(curLinNum.ToString() + "\t");
                        if (expLine.CommentLine != true)
                            Chronicler.Write(expLine.locationCounter.ToString("X6") + "\t");
                        Chronicler.Write(expLine.OriginalLine + "\t");
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
                string buffer = "";
                string prevOpCode = "start";
                int sl = 0;
                if (expresionLines.Count > 0)
                    sl = expresionLines[0].locationCounter;
                Chronicler.WriteLine("Object code:");
                foreach (PassOne.ExpresionLine expLine in expresionLines)
                {
                    if (expLine != null && expLine.OriginalLine != null && expLine.OriginalLine != "")
                    {
                        if((buffer.Length + expLine.Opcode.Length)>60 || (prevOpCode=="" && buffer!=""))
                        {
                            //dump buffer contents
                            file.Write("T"+sl.ToString("X5")+ buffer.Length.ToString("X2") + buffer);
                            file.Write("\n");
                            Chronicler.WriteLine("T" + buffer);

                            sl = expLine.locationCounter;
                            //clear buffer
                            buffer = "";
                        }

                        prevOpCode = expLine.Opcode;//check continuity
                        buffer += expLine.Opcode;//add curr line to buffer
                    }
                }
                if (buffer != "")
                {
                    //dump buffer contents
                    file.Write("T" + sl.ToString("X5") + buffer.Length.ToString("X2") + buffer);
                    file.Write("\n");
                    Chronicler.WriteLine("T" + buffer);

                    //clear buffer
                    buffer = "";
                }
                foreach (LiteralTable.LiteralValue lv in dataStructures.literalTable.literalTable)
                {

                    if (buffer != "" && (buffer.Length + lv.hexValue.Length) > 60)
                    {
                        //dump buffer contents
                        file.Write("T" + sl.ToString("X5") + buffer.Length.ToString("X2") + buffer);
                        file.Write("\n");
                        Chronicler.WriteLine("T" + buffer);

                        //clear buffer
                        buffer = "";
                    }
                    buffer += lv.hexValue;//add curr line to buffer
                }
                if (buffer != "")
                {
                    //dump buffer contents
                    file.Write("T" + sl.ToString("X5") + buffer.Length.ToString("X2") + buffer);
                    file.Write("\n");
                    Chronicler.WriteLine("T" + buffer);

                    //clear buffer
                    buffer = "";
                }
                dataStructures.symbolTable.PrintFile(file);
                dataStructures.symbolTable.Print();
            }
        }
    }
}
