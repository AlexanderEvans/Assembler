﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Evans1
{
    struct PassOne
    {
        public class ExpresionLine
        {
            public bool validLine = true;
            public bool CommentLine = false;
            public bool IsEQU = false;
            public bool DeferExpresionResolutiontoPass2 = false;
            public int lineNumber=0;
            public int locationCounter=0;
            public opcodeTable.operationData? operationData;
            public int instructionFormat=0;
            public static int startLoc = 0;
            /// <summary>
            /// string representation of operand hex
            /// </summary>
            public string Opcode = "";
            public Globals.ExpresionData expresionData = new Globals.ExpresionData();
            /// <summary>
            /// the byte representation of the operand field
            /// </summary>
            byte[] _ObjCode;
            public byte[] ObjCode
            {
                get
                {
                    return _ObjCode;
                }
                set
                {
                    switch(value.Length)
                    {
                        case 1:
                            instructionFormat = 1;
                            _ObjCode = value;
                            break;
                        case 2:
                            instructionFormat = 2;
                            _ObjCode = value;
                            break;
                        case 3:
                            instructionFormat = 3;
                            _ObjCode = value;
                            break;
                        case 4:
                            instructionFormat = 4;
                            _ObjCode = value;
                            break;
                        default:
                            Chronicler.LogError("Error, objCode is not a vaalide number of bytes for any instruction format 1-4!");
                            break;
                    }
                }
            }
            public string label="";
            public string format4indicator = "";
            public string operation="";
            public string operandFieldAndComment = "";
            public string OriginalLine = "";

            public ExpresionLine()
            {
                Opcode = "";
                ObjCode = new byte[4];
            }
        }
        public static void Execute(Globals.DataStructures dataStructures, out List<ExpresionLine> expresionLines, string filePath)
        {
            int locationCounter = 0;
            int lineNumber = 0;
            expresionLines = new List<ExpresionLine>();
            Regex regex = new Regex(@"^([\t ]*(?<label>[^\s]+:)[\t ]*){0,1}[\t ]*(?<flag>\+{0,1}[\t ]*)(?<operation>[^\s]+)(?<operandFieldAndComment>[\t ].*){0,1}[\t ]*($|(\r?\n))");
            int programLength = 0;
            bool skipOperandParsing = false;
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan))
                {

                    try
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            while (!streamReader.EndOfStream)
                            {
                                lineNumber++;
                                string currentLine = streamReader.ReadLine().Trim();
                                Match match = regex.Match(currentLine);
                                skipOperandParsing = false;

                                if (match.Success)
                                {
                                    ExpresionLine expresionLine = new ExpresionLine();
                                    expresionLine.OriginalLine = currentLine;
                                    expresionLine.lineNumber = lineNumber;
                                    expresionLine.locationCounter = locationCounter;
                                    expresionLine.label = match.Groups["label"].Value.Trim();
                                    expresionLine.operation = match.Groups["operation"].Value.Trim();
                                    expresionLine.operandFieldAndComment = match.Groups["operandFieldAndComment"].Value.Trim();
                                    expresionLine.lineNumber = lineNumber;
                                    expresionLine.format4indicator = match.Groups["flag"].Value.Trim();
                                    expresionLine.DeferExpresionResolutiontoPass2 = false;
                                    if (expresionLine.label!="")
                                    {
                                        if (expresionLine.label[expresionLine.label.Length - 1] == ':')
                                            expresionLine.label = expresionLine.label.Substring(0, expresionLine.label.Length - 1).Trim();
                                        else
                                        {
                                            expresionLine.validLine = false;
                                            Chronicler.LogError("Labels must end in a ':'");
                                        }
                                        if (expresionLine.validLine)
                                            SymbolTable.ValidateLabel(expresionLine.label, currentLine, "pass one");
                                    }



                                    Regex commentChecker = new Regex(@"[\t ]*(?<com>;.*)");
                                    Match comment = commentChecker.Match(currentLine);
                                    if (comment.Success)
                                    {
                                        expresionLine.OriginalLine = currentLine;
                                        expresionLine.locationCounter = 0;
                                        expresionLine.label = "";
                                        expresionLine.operation = "";
                                        expresionLine.operandFieldAndComment = "";
                                        expresionLine.lineNumber = lineNumber;
                                        expresionLine.format4indicator = "";
                                        expresionLine.CommentLine = true;
                                        expresionLine.DeferExpresionResolutiontoPass2 = false;
                                        skipOperandParsing = true;
                                        expresionLines.Add(expresionLine);
                                    }
                                    else
                                    {
                                        if (dataStructures.opcodeTable.assemblerDirectives.Contains(expresionLine.operation))
                                        {
                                            switch (expresionLine.operation)
                                            {
                                                case "EQU":
                                                    skipOperandParsing = true;
                                                    expresionLine.IsEQU = true;
                                                    if (expresionLine.operandFieldAndComment[0] == '*' && expresionLine.validLine)
                                                    {
                                                        dataStructures.symbolTable.addSymbol(expresionLine.label, true, locationCounter, currentLine);
                                                    }
                                                    else if (expresionLine.validLine)
                                                    {
                                                        if (ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData) != true)
                                                        {
                                                            expresionLine.validLine = false;
                                                        }
                                                        else
                                                        {
                                                            if (expresionLine.expresionData.first.HasValue)
                                                            {
                                                                int value = expresionLine.expresionData.first.Value.value;
                                                                if (expresionLine.expresionData.second.HasValue)
                                                                {
                                                                    if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.SUBTRACT)
                                                                        value = (expresionLine.expresionData.first.Value.value) - expresionLine.expresionData.second.Value.value;
                                                                    if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.ADD)
                                                                        value = (expresionLine.expresionData.first.Value.value) + expresionLine.expresionData.second.Value.value;

                                                                    if (expresionLine.expresionData.rflag.HasValue != true)
                                                                        expresionLine.validLine = false;

                                                                    if (expresionLine.validLine)
                                                                    {
                                                                        expresionLine.locationCounter = value;
                                                                        dataStructures.symbolTable.addSymbol(expresionLine.label, expresionLine.expresionData.rflag.Value, value, currentLine);
                                                                    }
                                                                    else
                                                                        Chronicler.LogError("Couldn't add \"" + currentLine + "\" to symbol table.", "pass one");
                                                                }
                                                                else
                                                                {
                                                                    expresionLine.expresionData.rflag = expresionLine.expresionData.first.Value.RFlag;

                                                                    if (expresionLine.validLine)
                                                                    {
                                                                        expresionLine.locationCounter = value;
                                                                        dataStructures.symbolTable.addSymbol(expresionLine.label, expresionLine.expresionData.rflag.Value, value, currentLine);
                                                                    }
                                                                    else
                                                                        Chronicler.LogError("Couldn't add \"" + currentLine + "\" to symbol table.", "pass one");
                                                                }

                                                            }
                                                            else
                                                            {
                                                                Chronicler.LogError("EQU can not be a literal value", "pass one");
                                                                expresionLine.validLine = false;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case "BYTE":
                                                    if (ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData) != true)
                                                    {
                                                        expresionLine.validLine = false;
                                                    }
                                                    else
                                                    {
                                                        skipOperandParsing = false;
                                                        if (expresionLine.expresionData.first.HasValue)
                                                        {
                                                            Chronicler.LogError("BYTE doesn't accept integer values", "pass one");
                                                            expresionLine.validLine = false;
                                                        }
                                                        else if (expresionLine.expresionData.literal != null)
                                                        {
                                                            if (expresionLine.expresionData.literal.isOldStyleLiteral)
                                                            {
                                                                if (expresionLine.label != "")
                                                                    dataStructures.symbolTable.addSymbol(expresionLine.label, true, locationCounter, currentLine);
                                                                locationCounter += expresionLine.expresionData.literal.Length;
                                                                expresionLine.Opcode = expresionLine.expresionData.literal.hexValue;
                                                            }
                                                            else
                                                            {
                                                                expresionLine.validLine = false;
                                                                Chronicler.LogError("New style literals are not allowed in assembler directives");
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case "WORD":
                                                    expresionLine.DeferExpresionResolutiontoPass2 = true;
                                                    if (ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData) != true)
                                                    {
                                                        expresionLine.validLine = false;
                                                    }
                                                    else
                                                    {
                                                        if (expresionLine.expresionData.first.HasValue)
                                                        {
                                                            int value = expresionLine.expresionData.first.Value.value;
                                                            if (expresionLine.expresionData.second.HasValue)
                                                            {
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.SUBTRACT)
                                                                    value = (expresionLine.expresionData.first.Value.value) - expresionLine.expresionData.second.Value.value;
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.ADD)
                                                                    value = (expresionLine.expresionData.first.Value.value) + expresionLine.expresionData.second.Value.value;
                                                            }

                                                            if (expresionLine.expresionData.rflag.HasValue != true)
                                                                expresionLine.validLine = false;

                                                            if (expresionLine.validLine && expresionLine.label != "")
                                                                dataStructures.symbolTable.addSymbol(expresionLine.label, true, locationCounter, currentLine);
                                                            if (expresionLine.validLine)
                                                            {
                                                                expresionLine.Opcode = value.ToString("X");
                                                                locationCounter += 3;
                                                            }
                                                            else
                                                                Chronicler.LogError("Couldn't parse \"" + currentLine + "\"", "pass one");
                                                        }
                                                        else if (expresionLine.expresionData.literal != null)
                                                        {
                                                            Chronicler.LogError("BYTE doesn't accept Charachter/Hex values", "pass one");
                                                            expresionLine.validLine = false;
                                                        }
                                                    }
                                                    break;
                                                case "START":
                                                    skipOperandParsing = true;
                                                    Regex testBlankLine = new Regex(@"[\t ]*;.*");
                                                    if (expresionLine.operandFieldAndComment == "")
                                                        locationCounter = 0;
                                                    else if (testBlankLine.IsMatch(expresionLine.operandFieldAndComment))
                                                    {
                                                        locationCounter = 0;
                                                    }
                                                    else
                                                    {
                                                        if (ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData) != true)
                                                        {
                                                            expresionLine.validLine = false;
                                                        }
                                                        else
                                                        {
                                                            Regex stripComment = new Regex(@"[\t ]*(?<min>-{0,1})[\t ]*(?<val>[0-9]+)[\t ]*(;.*){0,1}");
                                                            Match match1 = stripComment.Match(expresionLine.operandFieldAndComment);
                                                            if (match1.Success)
                                                            {
                                                                Globals.Symbol? tmp;
                                                                ExpresionHandler.ParseNum(match1.Groups["min"].Value + match1.Groups["val"].Value, out tmp, "(" + match1.Groups["min"].Value + match1.Groups["val"].Value + ")" + currentLine);
                                                                expresionLine.expresionData.comment = match1.Groups["$3"].Value;
                                                                if (tmp.HasValue && locationCounter==0)
                                                                {
                                                                    locationCounter = tmp.Value.value;
                                                                    ExpresionLine.startLoc = locationCounter;
                                                                }
                                                                else
                                                                {
                                                                    Chronicler.LogError("couldn't parse starting adress number, or start was not at beggining of the program", "Pass One");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                expresionLine.validLine = false;
                                                                Chronicler.LogError("couldn't parse starting adress", "Pass One");
                                                            }
                                                        }
                                                    }
                                                    if (expresionLine.label != "")
                                                        dataStructures.symbolTable.addSymbol(expresionLine.label, true, locationCounter, currentLine);
                                                    break;
                                                case "END":
                                                    skipOperandParsing = true;
                                                    testBlankLine = new Regex(@"[\t ]*;.*");
                                                    if (expresionLine.operandFieldAndComment == "")
                                                        locationCounter = 0;
                                                    else if (testBlankLine.IsMatch(expresionLine.operandFieldAndComment))
                                                    {
                                                        locationCounter = 0;
                                                    }
                                                    else
                                                    {
                                                        ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData);
                                                        if (expresionLine.expresionData.first.HasValue)
                                                        {
                                                            int value = expresionLine.expresionData.first.Value.value;
                                                            if (expresionLine.expresionData.second.HasValue)
                                                            {
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.SUBTRACT)
                                                                    value = (expresionLine.expresionData.first.Value.value) - expresionLine.expresionData.second.Value.value;
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.ADD)
                                                                    value = (expresionLine.expresionData.first.Value.value) + expresionLine.expresionData.second.Value.value;
                                                            }

                                                            if (expresionLine.expresionData.second.HasValue && expresionLine.expresionData.rflag.HasValue != true)
                                                                expresionLine.validLine = false;

                                                            if (expresionLine.validLine)
                                                            {
                                                                Globals.Symbol? symbol;
                                                                if (dataStructures.symbolTable.SearchSymbol(expresionLine.expresionData.first.Value.label, currentLine, out symbol))
                                                                {
                                                                    programLength = locationCounter - symbol.Value.value;
                                                                }
                                                            }
                                                            else
                                                                Chronicler.LogError("Couldn't add \"" + currentLine + "\" to symbol table.", "pass one");
                                                        }
                                                        else
                                                        {
                                                            Chronicler.LogError("END can not be a literal value", "pass one");
                                                            expresionLine.validLine = false;
                                                        }
                                                    }
                                                    if (expresionLine.label != "")
                                                        dataStructures.symbolTable.addSymbol(expresionLine.label, true, locationCounter, currentLine);
                                                    break;
                                                case "RESB":
                                                    skipOperandParsing = true;
                                                    if (ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData) != true)
                                                    {
                                                        expresionLine.validLine = false;
                                                    }
                                                    else
                                                    {
                                                        if (expresionLine.expresionData.first.HasValue)
                                                        {
                                                            int value = expresionLine.expresionData.first.Value.value;
                                                            if (expresionLine.expresionData.second.HasValue)
                                                            {
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.SUBTRACT)
                                                                    value = (expresionLine.expresionData.first.Value.value) - expresionLine.expresionData.second.Value.value;
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.ADD)
                                                                    value = (expresionLine.expresionData.first.Value.value) + expresionLine.expresionData.second.Value.value;
                                                            }


                                                            if (expresionLine.validLine && expresionLine.label != "")
                                                            {
                                                                dataStructures.symbolTable.addSymbol(expresionLine.label, true, locationCounter, currentLine);
                                                            }
                                                            else
                                                                Chronicler.LogError("Couldn't add \"" + currentLine + "\" to symbol table.", "pass one");
                                                            if (expresionLine.validLine)
                                                                locationCounter += value;
                                                        }
                                                        else if (expresionLine.expresionData.literal != null)
                                                        {
                                                            Chronicler.LogError("BYTE doesn't accept Charachter/Hex values", "pass one");
                                                            expresionLine.validLine = false;
                                                        }
                                                    }
                                                    break;
                                                case "RESW":
                                                    skipOperandParsing = true;
                                                    if (ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData) != true)
                                                    {
                                                        expresionLine.validLine = false;
                                                    }
                                                    else
                                                    {
                                                        if (expresionLine.expresionData.first.HasValue)
                                                        {
                                                            int value = expresionLine.expresionData.first.Value.value;
                                                            if (expresionLine.expresionData.second.HasValue)
                                                            {
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.SUBTRACT)
                                                                    value = (expresionLine.expresionData.first.Value.value) - expresionLine.expresionData.second.Value.value;
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.ADD)
                                                                    value = (expresionLine.expresionData.first.Value.value) + expresionLine.expresionData.second.Value.value;
                                                            }


                                                            if (expresionLine.validLine && expresionLine.label != "")
                                                            {
                                                                dataStructures.symbolTable.addSymbol(expresionLine.label, true, locationCounter, currentLine);
                                                            }
                                                            else
                                                                Chronicler.LogError("Couldn't add \"" + currentLine + "\" to symbol table.", "pass one");
                                                            if (expresionLine.validLine)
                                                                locationCounter += value * 3;
                                                        }
                                                        else if (expresionLine.expresionData.literal != null)
                                                        {
                                                            Chronicler.LogError("BYTE doesn't accept Charachter/Hex values", "pass one");
                                                            expresionLine.validLine = false;
                                                        }
                                                    }
                                                    break;
                                                case "EXTREF":
                                                    string[] arr = expresionLine.operandFieldAndComment.Split(',', StringSplitOptions.RemoveEmptyEntries);
                                                    for (int i = 0; i < arr.Length; i++)
                                                    {
                                                        arr[i] = arr[i].Trim();
                                                        dataStructures.symbolTable.addSymbol(arr[i], false, 0, currentLine);
                                                    }
                                                    break;
                                                default:
                                                    Chronicler.LogDetailedInfo("Doing: " + expresionLine.operation);
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            opcodeTable.operationData operDat;
                                            if (dataStructures.opcodeTable.operationTableDictionary.TryGetValue(expresionLine.operation, out operDat))
                                            {
                                                if (expresionLine.label != "")
                                                {
                                                    dataStructures.symbolTable.addSymbol(expresionLine.label, true, locationCounter, currentLine);
                                                }
                                                switch (operDat.format)
                                                {
                                                    case 1:
                                                        locationCounter += 1;
                                                        break;
                                                    case 2:
                                                        locationCounter += 2;
                                                        break;
                                                    case 3:
                                                        if (expresionLine.format4indicator == "")
                                                            locationCounter += 3;
                                                        else
                                                            locationCounter += 4;
                                                        break;
                                                    case 4:
                                                        locationCounter += 4;
                                                        break;
                                                    default:

                                                        break;
                                                }
                                                if (Parser.guessOperandType(expresionLine.operandFieldAndComment) == Parser.OperandType.DISPLACEMENT_OR_ADDRESS)
                                                    expresionLine.DeferExpresionResolutiontoPass2 = true;
                                                else
                                                    expresionLine.DeferExpresionResolutiontoPass2 = false;
                                                expresionLine.operationData = operDat;
                                            }
                                            else
                                                expresionLine.operationData = null;
                                            Chronicler.Write("Parsing operation: " + expresionLine.operation + "\n", Chronicler.OutputOptions.INFO);
                                        }
                                    }


                                    if(expresionLine.validLine && skipOperandParsing!=true && expresionLine.operandFieldAndComment!="" && !expresionLine.DeferExpresionResolutiontoPass2)
                                    {
                                        if(ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData)!=true)
                                        {
                                            expresionLine.validLine = false;
                                        }
                                    }
                                    expresionLines.Add(expresionLine);
                                }
                                else
                                {
                                        Chronicler.LogError("Couldn't parse fields on line: "+lineNumber);
                                }
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        Chronicler.LogError("failed to open File: " + filePath);
                    }
                }
            }
            catch (IOException e)
            {
                Chronicler.LogError("failed to open File: " + filePath);
            }

            dataStructures.literalTable.setStartAddress(locationCounter);

            DumpIntermidiateFile(filePath, expresionLines, dataStructures);
        }

        static void DumpIntermidiateFile(string filePath, List<ExpresionLine> expresionLines, Globals.DataStructures dataStructures)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + ".tmp"))
            {
                Chronicler.HoldOutput();
                int curLinNum = 0;
                foreach (ExpresionLine expLine in expresionLines)
                {
                    if (expLine!=null && expLine.OriginalLine!=null && expLine.OriginalLine!="")
                    {
                        curLinNum = expLine.lineNumber;

                        file.Write(curLinNum.ToString() + "\t");
                        if (expLine.CommentLine != true)
                            file.Write(expLine.locationCounter.ToString("X6") + "\t");
                        file.Write(expLine.OriginalLine);
                        file.Write("\n");

                        Chronicler.Write(curLinNum.ToString() + "\t");
                        if (expLine.CommentLine != true)
                            Chronicler.Write(expLine.locationCounter.ToString("X6") + "\t");
                        Chronicler.WriteLine(expLine.OriginalLine);
                    }
                }
                foreach(LiteralTable.LiteralValue lv in dataStructures.literalTable.literalTable)
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
        }
    }
}
