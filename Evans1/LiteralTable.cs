using System;
using System.Collections.Generic;
using System.Text;

namespace Evans1
{
    class LiteralTable
    {
        struct LiteralValue
        {
            public string label;
            public string value;
            public bool isChar;
            public int address;
        }
        int count = 0;
        LinkedList<LiteralValue> literalTable = new LinkedList<LiteralValue>();

        bool add(string literal)
        {
            bool rtnVal = true;
            LiteralValue literalValue = default;
            literal = literal.ToUpper().CompactAndTrimWhitespaces();
            literalValue.label=literal;
            if (literal[1] == 'X')
                literalValue.isChar = false;
            if (literal[1] == 'C')
                literalValue.isChar = true;
            else
            {
                Debug.LogError("Invalid literal type, please indicate hex\'X\' or Char\'C\'", "Adding Literal");
                rtnVal = false;
            }

            if(rtnVal==true)
            {
                if(literalValue.label[2] != '\'' || '\'' != literalValue.label[literalValue.label.Length-1])
                {
                    Debug.LogError("Invalid enclosing symbols, please surround your literal with single quotes '\''", "Adding Literal");
                    rtnVal = false;
                }
            }

            if (rtnVal == true)
            {
                
            }

        }
    }
}
