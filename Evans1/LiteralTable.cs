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
        }
    }
}
