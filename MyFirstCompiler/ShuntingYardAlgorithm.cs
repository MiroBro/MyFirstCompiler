using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstCompiler
{

    public enum Precedence
    {
        Plus = 0,
        Subtract = 0,
        Multiply = 1,
        Divide = 1,
        Sin = 2,
        Cos = 2,
        Tan = 2,
        Negate = 3,
        Assign = -1,
    }
    internal class ShuntingYardAlgorithm
    {
        private Queue<Token> outputQueue = new Queue<Token>();
        private Token lastEnqueued;
        bool wasLastTokenOperatorOrEmpty = true;


        private bool IsLastTokenOperator()
        {
            return lastEnqueued != null 
                && (lastEnqueued.tokenType != TokenType.EndOfFile
                && lastEnqueued.tokenType != TokenType.LeftBracket
                && lastEnqueued.tokenType != TokenType.RightBracket
                && lastEnqueued.tokenType != TokenType.Number);
        }

        private bool WasLastTokenOperatorOrEmpty()
        {
            return (IsLastTokenOperator() || outputQueue.Count == 0); 
        }

        public Queue<Token> GetOutputQueue(List<Token> tokenList)
        {
            Stack<Token> operatorStack = new Stack<Token>();
            for (int i = 0; i < tokenList.Count; i++)
            {
                Token currentToken = tokenList[i];
                if (currentToken.tokenType == TokenType.Number)
                {
                    outputQueue.Enqueue(tokenList[i]);
                }
                else if (currentToken.tokenType == TokenType.Symbol)
                {
                    outputQueue.Enqueue(tokenList[i]);
                }
                else if (IsOperator(currentToken.tokenType))
                {
                    if (WasLastTokenOperatorOrEmpty() && (lastEnqueued == null || lastEnqueued.tokenType != TokenType.Number)) 
                    {
                        if (currentToken.tokenType == TokenType.Subtract)
                        {
                            currentToken.tokenType = TokenType.Negate;
                        }
                    }
                    while (operatorStack.Count > 0 && GetTokenPrecedence(currentToken) <= GetTokenPrecedence(operatorStack.Peek()))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Push(currentToken);
                }
                else if (currentToken.tokenType == TokenType.LeftBracket)
                {
                    operatorStack.Push(currentToken);
                }
                else if (currentToken.tokenType == TokenType.RightBracket)
                {
                    while (operatorStack.Peek().tokenType != TokenType.LeftBracket)
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    //Should find left bracket eventually, if not, mismatched brackets and should throw error here!
                    operatorStack.Pop();
                }
                lastEnqueued = currentToken;
            }

            //pop rest of the operations left in stack if there are any
            while (operatorStack.Count > 0)
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }

            return outputQueue;
        }

        private bool IsOperator(TokenType token)
        {
            return token == TokenType.Add || token == TokenType.Subtract || token == TokenType.Multiply || token == TokenType.Divide || token == TokenType.Sin || token == TokenType.Cos || token == TokenType.Tan || token == TokenType.Negate || token == TokenType.Assignment;
        }

        private int GetTokenPrecedence(Token token)
        {
            switch (token.tokenType)
            {
                case TokenType.Add:
                    return (int)Precedence.Plus;
                case TokenType.Subtract:
                    return (int)Precedence.Subtract;
                case TokenType.Multiply:
                    return (int)Precedence.Multiply;
                case TokenType.Divide:
                    return (int)Precedence.Divide;
                case TokenType.Sin:
                    return (int)Precedence.Sin;
                case TokenType.Cos:
                    return (int)Precedence.Cos;
                case TokenType.Tan:
                    return (int)Precedence.Tan;
                case TokenType.Negate:
                    return (int)Precedence.Negate;
                case TokenType.Assignment:
                    return (int)Precedence.Assign;
            }
            return -1;
        }
    }
}