using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstCompiler
{
    public class Statement
    {
        public List<Token> lhs = new List<Token>();
        public List<Token> rhsPreSYA = new List<Token>();
        public Queue<Token> rhsPostSYA = new Queue<Token>();

        public StatementType statementType = StatementType.None;
        public string parameterNameIfFunction;
    }
}
