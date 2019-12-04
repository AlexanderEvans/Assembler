using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    class opcodeTable
    {
        public struct operationData
        {
            public string name;
            public int code;
            public int format;
        }
        public Dictionary<string, int> RegisterTable = new Dictionary<string, int>
        {
            {"A"   , 0 },
            {"X"   , 1 },
            {"L"   , 2 },
            {"B"   , 3 },
            {"S"   , 4 },
            {"T"   , 5 },
            {"F"   , 6 },
            {"PC"   , 8 },
            {"SW"   , 9 },
        };
        public List<string> assemblerDirectives = new List<string>
        {
            {"EQU"},
            {"BASE"},
            {"BYTE"},
            {"END"},
            {"EXTDEF"},
            {"EXTREF"},
            {"RESB"},
            {"RESW"},
            {"START"},
            {"WORD"},
        };
        public Dictionary<string, operationData> operationTableDictionary = new Dictionary<string, operationData>
        {
            {"ADD"   , new operationData{name = "ADD"   , code = 0x18, format = 3} },
            {"ADDF"  , new operationData{name = "ADDF"  , code = 0x58, format = 3} },
            {"ADDR"  , new operationData{name = "ADDR"  , code = 0x90, format = 2} },
            {"AND"   , new operationData{name = "AND"   , code = 0x40, format = 3} },
            {"CLEAR" , new operationData{name = "CLEAR" , code = 0xB4, format = 2} },
            {"COMP"  , new operationData{name = "COMP"  , code = 0x28, format = 3} },
            {"COMPF" , new operationData{name = "COMPF" , code = 0x88, format = 3} },
            {"COMPR" , new operationData{name = "COMPR" , code = 0xA0, format = 2} },
            {"DIV"   , new operationData{name = "DIV"   , code = 0x24, format = 3} },
            {"DIVR"  , new operationData{name = "DIVR"  , code = 0x9C, format = 2} },
            {"FIX"   , new operationData{name = "FIX"   , code = 0xC4, format = 1} },
            {"FLOAT" , new operationData{name = "FLOAT" , code = 0xC0, format = 1} },
            {"HIO"   , new operationData{name = "HIO"   , code = 0xF4, format = 1} },
            {"J"     , new operationData{name = "J"     , code = 0x3C, format = 3} },
            {"JEQ"   , new operationData{name = "JEQ"   , code = 0x30, format = 3} },
            {"JGT"   , new operationData{name = "JGT"   , code = 0x34, format = 3} },
            {"JLT"   , new operationData{name = "JLT"   , code = 0x38, format = 3} },
            {"JSUB"  , new operationData{name = "JSUB"  , code = 0x48, format = 3} },
            {"LDA"   , new operationData{name = "LDA"   , code = 0x00, format = 3} },
            {"LDB"   , new operationData{name = "LDB"   , code = 0x68, format = 3} },
            {"LDCH"  , new operationData{name = "LDCH"  , code = 0x50, format = 3} },
            {"LDF"   , new operationData{name = "LDF"   , code = 0x70, format = 3} },
            {"LDL"   , new operationData{name = "LDL"   , code = 0x08, format = 3} },
            {"LDS"   , new operationData{name = "LDS"   , code = 0x6C, format = 3} },
            {"LDT"   , new operationData{name = "LDT"   , code = 0x74, format = 3} },
            {"LDX"   , new operationData{name = "LDX"   , code = 0x04, format = 3} },
            {"LPS"   , new operationData{name = "LPS"   , code = 0xD0, format = 3} },
            {"MUL"   , new operationData{name = "MUL"   , code = 0x20, format = 3} },
            {"MULF"  , new operationData{name = "MULF"  , code = 0x60, format = 3} },
            {"MULR"  , new operationData{name = "MULR"  , code = 0x98, format = 2} },
            {"NORM"  , new operationData{name = "NORM"  , code = 0xC8, format = 1} },
            {"OR"    , new operationData{name = "OR"    , code = 0x44, format = 3} },
            {"RD"    , new operationData{name = "RD"    , code = 0xD8, format = 3} },
            {"RMO"   , new operationData{name = "RMO"   , code = 0xAC, format = 2} },
            {"RSUB"  , new operationData{name = "RSUB"  , code = 0x4C, format = 3} },
            {"SHIFTL", new operationData{name = "SHIFTL", code = 0xA4, format = 2} },
            {"SHIFTR", new operationData{name = "SHIFTR", code = 0xA8, format = 2} },
            {"SIO"   , new operationData{name = "SIO"   , code = 0xF0, format = 1} },
            {"SSK"   , new operationData{name = "SSK"   , code = 0xEC, format = 3} },
            {"STA"   , new operationData{name = "STA"   , code = 0x0C, format = 3} },
            {"STB"   , new operationData{name = "STB"   , code = 0x78, format = 3} },
            {"STCH"  , new operationData{name = "STCH"  , code = 0x54, format = 3} },
            {"STF"   , new operationData{name = "STF"   , code = 0x80, format = 3} },
            {"STI"   , new operationData{name = "STI"   , code = 0xD4, format = 3} },
            {"STL"   , new operationData{name = "STL"   , code = 0x14, format = 3} },
            {"STS"   , new operationData{name = "STS"   , code = 0x7C, format = 3} },
            {"STSW"  , new operationData{name = "STSW"  , code = 0xE8, format = 3} },
            {"STT"   , new operationData{name = "STT"   , code = 0x84, format = 3} },
            {"STX"   , new operationData{name = "STX"   , code = 0x10, format = 3} },
            {"SUB"   , new operationData{name = "SUB"   , code = 0x1C, format = 3} },
            {"SUBF"  , new operationData{name = "SUBF"  , code = 0x5C, format = 3} },
            {"SUBR"  , new operationData{name = "SUBR"  , code = 0x94, format = 2} },
            {"SVC"   , new operationData{name = "SVC"   , code = 0xB0, format = 2} },
            {"TD"    , new operationData{name = "TD"    , code = 0xE0, format = 3} },
            {"TIO"   , new operationData{name = "TIO"   , code = 0xF8, format = 1} },
            {"TIX"   , new operationData{name = "TIX"   , code = 0x2C, format = 3} },
            {"TIXR"  , new operationData{name = "TIXR"  , code = 0xB8, format = 2} },
            {"WD"    , new operationData{name = "WD"    , code = 0xDC, format = 3} },
        };
    }
}
