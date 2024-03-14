using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyFirstCompiler
{
    public enum StatementType
    {
        None,
        Print,
        Assignment,
        FunctionDef,
        FunctionCall,
    }

    //I don't know what I'm doing :) BUT IM GUESSING
    internal class Parser
    {
        public List<Token> Tokens;

        public Statement Parse(List<Token> outputQueue)
        {
            var pair = SplitIntoLHSandRHS(outputQueue);
            return pair;
        }

        private Statement SplitIntoLHSandRHS(List<Token> outputQueue)
        {
            Statement assignmentPair = new Statement();

            bool encounteredAssignment = false;

            foreach (Token token in outputQueue)
            {
                if (token.tokenType == TokenType.Assignment)
                {
                    encounteredAssignment = true;
                }
                else if (encounteredAssignment)
                {
                    assignmentPair.rhsPreSYA.Add(token);
                }
                else
                {
                    assignmentPair.lhs.Add(token);
                }
            }

            //save variable if first time encountered
            if (encounteredAssignment && !Compiler.allIntVariableNames.Contains(assignmentPair.lhs[0].valueName))
            {
                Compiler.allIntVariableNames.Add(assignmentPair.lhs[0].valueName);
            }

            //fix the expression if it wasnt an assignment
            if (assignmentPair.rhsPreSYA.Count == 0)
            {
                assignmentPair.rhsPreSYA = assignmentPair.lhs;
                assignmentPair.lhs = new List<Token>();
            }

            ParseTypeOfStatement(assignmentPair);

            return assignmentPair;
        }

        private void ParseTypeOfStatement(Statement assignmentPair)
        {
            bool containsBracketsLeft = false;
            bool containsBracketsRight = false;
            bool containsPreviouslyKnownFunction_Left = false;
            bool containsPreviouslyKnownFunction_Right = false;

            foreach (var token in assignmentPair.lhs)
            {
                if (token.tokenType == TokenType.LeftBracket)
                {
                    containsBracketsLeft = true;
                }
                if (Compiler.allFunctionNames.Contains(token.valueName))
                {
                    containsPreviouslyKnownFunction_Left = true;
                }
            }

            foreach (var token in assignmentPair.rhsPreSYA)
            {
                if (token.tokenType == TokenType.LeftBracket)
                {
                    containsBracketsRight = true;
                }
                if (Compiler.allFunctionNames.Contains(token.valueName))
                {
                    containsPreviouslyKnownFunction_Right = true;
                }
            }

            if (assignmentPair.lhs.Count == 0)
            {
                if (containsPreviouslyKnownFunction_Right)
                {
                    assignmentPair.statementType = StatementType.FunctionCall;
                }
                else
                {
                    assignmentPair.statementType = StatementType.Print;
                }
            }
            else
            {

                if (containsBracketsLeft)
                {
                    Token functionName;
                    for (int i = 0; i < assignmentPair.lhs.Count; i++)
                    {
                        if (assignmentPair.lhs[i].tokenType == TokenType.LeftBracket)
                        {
                            functionName = assignmentPair.lhs[i - 1];
                            Compiler.allFunctionNames.Add(functionName.valueName);
                            assignmentPair.statementType = StatementType.FunctionDef;
                            break;
                        }
                    }
                }
                else
                {
                    assignmentPair.statementType = StatementType.Assignment;
                }
            }
        }
    }
}


