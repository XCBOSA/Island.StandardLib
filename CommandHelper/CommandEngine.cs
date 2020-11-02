using EXTS;
using System.Collections.Generic;

namespace Island.StandardLib.CommandHelper
{
    public class CommandEngine
    {
        int compilingPos;
        string compilingCode;
        const char EOF = (char)0;

        public readonly string[] Result;

        public CommandEngine(string input)
        {
            compilingCode = input;
            compilingPos = 0;
            char ch = EOF;
            List<string> parts = new List<string>();
            while ((ch = Peek()) != EOF)
            {
                if (ch == '\"') parts.Add(PeekString(true));
                else if (ch == ' ') continue;
                else parts.Add(ch + PeekString(false));
            }
            Result = parts.ToArray();
        }

        char Peek()
        {
            if (compilingPos < compilingCode.Length)
                return compilingCode[compilingPos++];
            else
            {
                compilingPos++;
                return EOF;
            }
        }

        string PeekString(bool useEndQuote)
        {
            string str = "";
            char ch;
            while (true)
            {
                ch = Peek();
                if (ch == '\\')
                {
                    char ct = Peek();
                    switch (ct)
                    {
                        case 'n': str += '\n'; break;
                        case 't': str += '\t'; break;
                        case '\"': str += '\"'; break;
                        case '\\': str += '\\'; break;
                        default: throw new SyntaxException("未识别的转义符。", compilingPos);
                    }
                }
                if (ch == ' ' && !useEndQuote) return str;
                if (ch == EOF)
                {
                    if (useEndQuote)
                        throw new SyntaxException("字符串直到文件结尾都未结束，请检查引号是否完整。", compilingPos);
                    else return str;
                }
                if (ch == '\"') break;
                str += ch;
            }
            return str;
        }

        public static implicit operator string[](CommandEngine engine) => engine.Result;
    }
}
