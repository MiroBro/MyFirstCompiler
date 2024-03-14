using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstCompiler
{   public class Token
    {
        public TokenType tokenType { get; set; }
        public double value { get; set; }
        public string? valueName { get; set; }
    }
}
