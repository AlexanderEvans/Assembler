using System;
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
            public int lineNumber=0;
            public int locationCounter=0;
            public int instructionFormat=0;
            public string Opcode
            {
                get => Opcode;
                set
                {
                    //switch on format lookup
                    Opcode = value;
                }
            }
            public Globals.ExpresionData expresionData = new Globals.ExpresionData();
            public byte[] ObjCode
            {
                get
                {
                    return ObjCode;
                }
                set
                {
                    switch(value.Length)
                    {
                        case 1:
                            instructionFormat = 1;
                            ObjCode = value;
                            break;
                        case 2:
                            instructionFormat = 2;
                            ObjCode = value;
                            break;
                        case 3:
                            instructionFormat = 3;
                            ObjCode = value;
                            break;
                        case 4:
                            instructionFormat = 4;
                            ObjCode = value;
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
            public string operandFieldAndComment="";

            public ExpresionLine()
            {
                Opcode = "";
                ObjCode = new byte[4];
            }
        }
        void Execute(Globals.DataStructures dataStructures, out List<ExpresionLine> expresionLines, string filePath)
        {
            int locationCounter = 0;
            int lineNumber = 0;
            expresionLines = new List<ExpresionLine>();
            Regex regex = new Regex(@"^(\h{0,}(?<label>[^\h]*)\h){0,1}\h{0,}(?<flag>\+{0,1}\h{0,})(?<operation>[^\h]*)\h(?<operandFieldAndComment>.*){0,1}$");

            bool useLine = true;
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
                                useLine = true;
                                skipOperandParsing = false;
                                lineNumber++;
                                string currentLine = streamReader.ReadLine().Trim();
                                Match match = regex.Match(currentLine);

                                if (match.Success)
                                {
                                    ExpresionLine expresionLine = new ExpresionLine();
                                    expresionLine.lineNumber = lineNumber;
                                    expresionLine.locationCounter = locationCounter;
                                    expresionLine.label = match.Groups["label"].Value.Trim();
                                    expresionLine.operation = match.Groups["operation"].Value.Trim();
                                    expresionLine.operandFieldAndComment = match.Groups["operandFieldAndComment"].Value.Trim();
                                    expresionLine.lineNumber = lineNumber;
                                    expresionLine.format4indicator = match.Groups["flag"].Value.Trim(); ;
                                    if (expresionLine.label!="")
                                    {
                                        if (expresionLine.label[expresionLine.label.Length - 1] == ':')
                                            expresionLine.label = expresionLine.label.Substring(0, expresionLine.label.Length - 1);
                                        else
                                        {
                                            useLine = false;
                                            Chronicler.LogError("Labels must end in a ':'");
                                        }
                                        if (dataStructures.symbolTable.addSymbol(expresionLine.label, true, locationCounter, currentLine) != true)
                                        {
                                            useLine = false;
                                        }
                                    }
                                    if(dataStructures.opcodeTable.assemblerDirectives.Contains(expresionLine.operation))
                                    {
                                        switch(expresionLine.operation)
                                        {
                                            case "EQU":
                                                skipOperandParsing = true;
                                                if (expresionLine.operandFieldAndComment[0] == '*')
                                                    dataStructures.symbolTable.addSymbol(expresionLine.label, 1, locationCounter, currentLine);
                                                else
                                                {
                                                    if (ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData) != true)
                                                    {
                                                        useLine = false;
                                                    }
                                                    else
                                                    {
                                                        int? value = null;
                                                        if (expresionLine.expresionData.first.HasValue)
                                                        {
                                                            if(expresionLine.expresionData.second.HasValue)
                                                            {
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.SUBTRACT)
                                                                    value = (expresionLine.expresionData.first.Value.value) - expresionLine.expresionData.second.Value.value;
                                                                if (expresionLine.expresionData.operatorValue != null && expresionLine.expresionData.operatorValue == Globals.ExpresionData.Arithmetic.ADD)
                                                                    value = (expresionLine.expresionData.first.Value.value) + expresionLine.expresionData.second.Value.value;
                                                            }
                                                        }

                                                        if (expresionLine.expresionData.rflag.HasValue != true || value.HasValue != true)
                                                            useLine = false;

                                                        if (useLine)
                                                            dataStructures.symbolTable.addSymbol(expresionLine.label, expresionLine.expresionData.rflag.Value, value.Value, currentLine);
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {

                                    }
                                    if(useLine && skipOperandParsing!=true && expresionLine.operandFieldAndComment!="")
                                    {
                                        if(ExpresionHandler.ResolveF3F4Expresion(dataStructures, expresionLine.operandFieldAndComment, currentLine, out expresionLine.expresionData)!=true)
                                        {
                                            useLine = false;
                                        }
                                    }
                                }
                                else
                                {

                                    Chronicler.LogError("Couldn't parse fields on line: ");
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
        
        }
    }
}
