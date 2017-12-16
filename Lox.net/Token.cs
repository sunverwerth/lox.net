using System;

namespace Lox
{
    public class Token
    {
        public readonly TokenType type;
        public readonly String lexeme;
        public readonly Object literal;
        public readonly int line;

        public Token(TokenType type, String lexeme, Object literal, int line)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString()
        {
            return type + " " + lexeme + " " + literal;
        }
    }
}
