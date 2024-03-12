using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstCompiler
{
    //I don't know what I'm doing :) BUT IM GUESSING
    internal class Parser
    {
        public List<Token> Tokens;

        public AssignmentPair Parse(List<Token> outputQueue)
        {
            return SplitIntoLHSandRHS(outputQueue);
        }

        private AssignmentPair SplitIntoLHSandRHS(List<Token> outputQueue)
        {
            AssignmentPair assignmentPair = new AssignmentPair();

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

            return assignmentPair;
        }

    }

    public enum AssignmentType
    {
        None,
        Print,
        Assignment,
        Function,
    }

    internal class AssignmentPair
    {
        public List<Token> lhs = new List<Token>();
        public List<Token> rhsPreSYA = new List<Token>();
        public Queue<Token> rhsPostSYA = new Queue<Token>();

        public AssignmentType assignmentType = AssignmentType.None;
    }
}
