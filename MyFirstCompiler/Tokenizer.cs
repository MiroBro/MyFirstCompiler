using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstCompiler
{
    internal class Tokenizer
    {
        TextReader _reader;
        char _currentChar;
        TokenType _currentToken;
        double _number;
        private List<Token> _tokens = new List<Token>();

        public Tokenizer(TextReader reader)
        {
            _reader = reader;
        }

        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            while (_reader.Peek() != -1)
            {
                AppendNextToken();
            }
            return _tokens;
        }

        //Current Token
        public TokenType Token {
            get { return _currentToken; } 
        }

        //Value of number-token
        public double Number {
            get { return _number;  } 
        }

        //read next character in input stream
        //store in currentChar or load '\0' if EOF
        public void NextChar()
        {
            int ch = _reader .Read();
            _currentChar = ch < 0 ? '\0' : (char)ch;
        }

        //Move to next token from input stream
        public void AppendNextToken()
        {
            while (char.IsWhiteSpace(_currentChar) || _currentChar.Equals('\0'))
            {
                NextChar();
            }

            //Special Characters
            switch  (_currentChar) 
            {
                case '+':
                    NextChar();
                    _currentToken = TokenType.Add;
                    _tokens.Add(new Token() { tokenType = _currentToken,});
                    return;
                case '-':
                    NextChar();
                    _currentToken = TokenType.Subtract;
                    _tokens.Add(new Token() { tokenType = _currentToken, });
                    return;
                case '*':
                    NextChar();
                    _currentToken = TokenType.Multiply;
                    _tokens.Add(new Token() { tokenType = _currentToken, });
                    return;
                case '/':
                    NextChar();
                    _currentToken = TokenType.Divide;
                    _tokens.Add(new Token() { tokenType = _currentToken, });
                    return;
                case '(':
                    NextChar();
                    _currentToken = TokenType.LeftBracket;
                    _tokens.Add(new Token() { tokenType = _currentToken, });
                    return;
                case ')':
                    NextChar();
                    _currentToken = TokenType.RightBracket;
                    _tokens.Add(new Token() { tokenType = _currentToken, });
                    return;

            }

            //Number
            if (char.IsDigit(_currentChar) || _currentChar == '.')
            {

                //Capture decimal point

                var sb = new StringBuilder();
                bool hasDecimalPoint = false;

                while (char.IsDigit(_currentChar) ||(!hasDecimalPoint && _currentChar == '.'))
                {
                    sb.Append(_currentChar);
                    hasDecimalPoint = _currentChar == '.';
                    NextChar();
                }

                //Parse it
                _number = double.Parse(sb.ToString(), CultureInfo.InvariantCulture);
                _currentToken = TokenType.Number;
                _tokens.Add(new Token() { tokenType = _currentToken, value = _number, });
                return;
            }

            if (char.IsLetter(_currentChar))
            {

                var sb2 = new StringBuilder();

                while (char.IsLetter(_currentChar))
                {
                    sb2.Append(_currentChar);
                    NextChar();
                }

                switch (sb2.ToString().ToLower()) {
                    case "sin":
                        _currentToken = TokenType.Sin;
                        _tokens.Add(new Token() { tokenType = _currentToken });
                        return;
                    case "cos":
                        _currentToken = TokenType.Sin;
                        _tokens.Add(new Token() { tokenType = _currentToken });
                        return;
                    case "tan":
                        _currentToken = TokenType.Tan;
                        _tokens.Add(new Token() { tokenType = _currentToken });
                        return;
                }
            }

            throw new InvalidDataException($"Unexpected character : {_currentChar}");
        }
    }
    
    public class Token
    {
        public TokenType tokenType { get; set; }
        public double value { get; set; }
    }

    public enum TokenType
    {
        EndOfFile,
        LeftBracket,
        RightBracket,
        Number,
        Add,
        Subtract,
        Negate,
        Multiply,
        Divide,
        Sin,
        Cos,
        Tan
    }
}
