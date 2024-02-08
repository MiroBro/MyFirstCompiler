using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstCompiler
{
    public abstract class Node
    {
        public abstract double Evaluate();
    }

    public class NodeNumber : Node
    {
        double _number;

        public NodeNumber(double number)
        {
            _number = number;
        }

        public override double Evaluate()
        {
            return _number;
        }
    }

    public class NodeBinary : Node
    {
        Node _lhs;
        Node _rhs;
        Func<double, double, double> _op;

        public NodeBinary(Node lhs, Node rhs, Func<double, double, double> op)
        {
            _lhs = lhs;
            _rhs = rhs;
            _op = op;
        }

        public override double Evaluate()
        {
            //Evaluate both sides
            var lhsVal = _lhs.Evaluate();
            var rhsVal = _rhs.Evaluate();

            //Evaluate and return
            var result = _op(lhsVal, rhsVal);
            return result;
        }
    }
}
