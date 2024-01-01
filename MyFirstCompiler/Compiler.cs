// See https://aka.ms/new-console-template for more information //GipGap
using MyFirstCompiler;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;


internal class Compiler
{
    private static string nameOfAssembly = "calculation";

    private int assemblyCounter;
    private string assemblyCalculations = "";

    private AssemblyTextCreator assemblyTextCreator;

    public void Run(string[] expressionToCompile)
    {
        assemblyTextCreator = new AssemblyTextCreator();

        foreach (var expression in expressionToCompile)
        {
            EvaluateExpression(expression);
        }

        File.WriteAllText($"{nameOfAssembly}.asm", string.Empty);
        using (StreamWriter assemblyFile = File.AppendText($"{nameOfAssembly}.asm")) 
        {
            assemblyFile.WriteLine(assemblyTextCreator.GetAssemblyText());
            //assemblyFile.WriteLine(assemblyBoilerPlateStart);
            //assemblyFile.WriteLine(assemblyCalculations);
            //assemblyFile.WriteLine(assemblyBoilerPlateEnd);
        };

        CreateExecutable();
    }

    private static void CreateExecutable()
    {
        //Do the assembly thing!
        Process processAssembling = new Process();
        processAssembling.StartInfo.FileName = "C:\\Users\\Miro\\AppData\\Local\\bin\\NASM\\nasm";
        processAssembling.StartInfo.Arguments = $"-f win64 {nameOfAssembly}.asm";
        processAssembling.StartInfo.UseShellExecute = false;
        processAssembling.StartInfo.RedirectStandardOutput = true;
        processAssembling.Start();

        string outputAssembling = processAssembling.StandardOutput.ReadToEnd();
        processAssembling.WaitForExit();

        //Do the linky thing!
        Process processLinking = new Process();
        processLinking.StartInfo.FileName = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\VC\\Tools\\MSVC\\14.37.32822\\bin\\Hostx64\\x64\\link.exe";
        processLinking.StartInfo.Arguments = @$"{nameOfAssembly}.obj /subsystem:console /entry:WinMain  /libpath:path_to_libs /nodefaultlib kernel32.lib user32.lib /largeaddressaware:no";
        processLinking.StartInfo.UseShellExecute = false;
        processLinking.StartInfo.RedirectStandardOutput = true;
        processLinking.Start();

        string outputLinking = processLinking.StandardOutput.ReadToEnd();
        processLinking.WaitForExit();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Finished assembling {outputAssembling} and linking {outputLinking}.");
        Console.ForegroundColor = ConsoleColor.White;
    }

    private double EvaluateExpression(string expression)
    {
        Tokenizer tokenizer = new Tokenizer(new StringReader(expression));
        List<Token> tokensInOrder = tokenizer.Tokenize();

        ShuntingYardAlgorithm sya = new ShuntingYardAlgorithm();
        Queue<Token> outputQueue = sya.GetOutputQueue(tokensInOrder);

        return CalculateOutputQueue(outputQueue);
    }
    //TODO: Print a 64 bit number in decimal (based TEN, not HEX). 0-9, NOT 1.0
    // Create a NEW assembly file, dont use C#
    private double CalculateOutputQueue(Queue<Token> outputQueue)
    {
        Stack<double> stack = new Stack<double>();

        foreach (var token in outputQueue)
        {
            if (token.tokenType == TokenType.Number)
            {
                assemblyCalculations +=
                            $"""
                             push    {(int)token.value} 

                             """;
                stack.Push(token.value);
            }
            else
            {
                switch (token.tokenType)
                {
                    case TokenType.Add:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = a + b;                            
                            stack.Push(r);

                            assemblyTextCreator.AddInstructionToCompile(a, b, token.tokenType, $"{a} + {b} ");

                            assemblyCalculations +=
                            $"""
                             pop     rcx
                             pop     rbx
                             add     rcx, rbx
                             push    rcx

                             """;
                        }
                        break;
                    case TokenType.Subtract:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = b - a;

                            assemblyTextCreator.AddInstructionToCompile(b, a, token.tokenType, $"{b} - {a} ");

                            assemblyCalculations +=
                            $"""
                             pop     rbx
                             pop     rcx
                             sub     rcx, rbx
                             push    rcx

                             """;
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Negate:
                        {
                            var a = stack.Pop(); 
                            var r = - a;
                            stack.Push(r);
                            assemblyCalculations += // Cheating because I couldn't figure out how to multiply by -1 hahahah
                            $"""
                             mov     rcx, 0 
                             pop     rbx
                             sub     rcx, rbx
                             push    rcx

                             """; 
                        }
                        break;
                    case TokenType.Divide:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = a / b;
                            stack.Push(r); //Need to do integer divide! Theres a remainder func to give rest READ how divide and mult work!! add modulus operator! DONT CHUCK FLOATS
                            /*assemblyCalculations +=
                            $"""
                             pop     edx
                             pop     eax
                             div     2
                             mov     rcx, edx
                             push    rcx

                             """;*/
                            //RDX:RAX / r/m64 = RAX(Quotient), RDX (Remainder)
                            // 6 / 3
                            assemblyCalculations += 
                            $"""
                             mov     rdx, 0
                             pop     rcx
                             pop     rax
                             div     rcx
                             push    rax

                             """;
                            
                            /*assemblyCalculations += 
                            $"""
                             mov     rax, 0 
                             pop     rbx
                             ;sub     rbx, rbx
                             div     rbx
                             push    rcx

                             """;
                            */
                        }
                        break;
                    case TokenType.Multiply:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = a * b;
                            stack.Push(r);
                            assemblyCalculations +=
                            $"""
                             pop     rcx
                             pop     rax
                             mul     rcx
                             push    rax

                             """;
                        }
                        break;
                    case TokenType.Sin:
                        {
                            var a = stack.Pop();
                            var r = Math.Sin((a*Math.PI)/180);
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Cos:
                        {
                            var a = stack.Pop();
                            var r = Math.Cos((a * Math.PI) / 180);
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Tan:
                        {
                            var a = stack.Pop();
                            var r = Math.Tan((a * Math.PI) / 180);
                            stack.Push(r);
                        }
                        break;
                }
            }
        }
        assemblyCalculations +=
            $"""
            pop     rcx

            """;
        return stack.Pop();
    }


}

//        EvaluateExpression(expressionsToCompile[0]);

//assemblyCalculations += $"  mov rcx, {a + b}";
/*assemblyCalculations += 
 $"""
 mov     rcx, {a}
 mov     rbx, {b}
 add     rcx, rbx
 """;
*/

/*
 
     private string[] expressionsToCompile = new string[] { //CHANGE THIS TO COMMANDLINE ARGS
           "0",
           "- 1",
           "1 - 3",
           "40 + 2",
           "1.0 + 2.3 * sin 90",
           "sin 90",
           "sin 45 + 45",
            "1.0 + 2.3 * tan 0.5" };
 */

// Old attempt at creating assembly
/*foreach (var expression in expressionsToCompile)
{
    var assemblyCreator = new AssemblyCreator(assemblyCounter++.ToString(), expression);
    Console.WriteLine(EvaluateExpression(expression, assemblyCreator));
}*/

/*
 

    private double CalculateOutputQueue(Queue<Token> outputQueue, AssemblyCreator compiler)
    {
        Stack<double> stack = new Stack<double>();

        foreach (var token in outputQueue)
        {
            if (token.tokenType == TokenType.Number)
            {
                stack.Push(token.value);
            }
            else
            {
                switch (token.tokenType)
                {
                    case TokenType.Add:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = a + b;
                            compiler.WriteInstruction(TokenType.Add, a, b);
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Subtract:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = b - a;
                            compiler.WriteInstruction(TokenType.Subtract, a, b);
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Negate:
                        {
                            var a = stack.Pop(); //Ended here somewhere with negate! 
                            var r = - a;
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Divide:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = a / b;
                            compiler.WriteInstruction(TokenType.Divide, a, b);
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Multiply:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = a * b;
                            compiler.WriteInstruction(TokenType.Multiply, a, b);
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Sin:
                        {
                            var a = stack.Pop();
                            var r = Math.Sin((a*Math.PI)/180);
                            compiler.WriteInstruction(TokenType.Sin,a);
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Cos:
                        {
                            var a = stack.Pop();
                            var r = Math.Cos((a * Math.PI) / 180);
                            compiler.WriteInstruction(TokenType.Cos, a);
                            stack.Push(r);
                        }
                        break;
                    case TokenType.Tan:
                        {
                            var a = stack.Pop();
                            var r = Math.Tan((a * Math.PI) / 180);
                            compiler.WriteInstruction(TokenType.Tan, a);
                            stack.Push(r);
                        }
                        break;
                }
            }
        }
        return stack.Pop();
    }
 
 */



//public int assemblyCounter;
/*
         foreach (var expression in expressions)
    {
        var assemblyCreator = new AssemblyCreator(assemblyCounter++.ToString(), expression);
        Console.WriteLine(EvaluateExpression(expression, assemblyCreator));
    }
 */



/*
    private void RecursiveCalculate(Queue<Token> outputQueue)
    {
        while (outputQueue.Count > 0)
        {
            switch (outputQueue.Dequeue().tokenType)
            {
                case TokenType.Add:
                    break;
                case TokenType.Subtract:
                    break;
                case TokenType.Multiply:
                    break;
                case TokenType.Divide:
                    break;
                case TokenType.Number:
                    break;
            }
        }
    }
 */











/*
class BinaryOperation{
    Operator left;
    Operator right;
    Operator ownToken;

    public BinaryOperation(Operator left, Operator right, Operator ownToken)
    {
        this.left = left;
        this.right = right;
        this.ownToken = ownToken;
    }
}

class Operator
{

}

class Number
{
    int value;
}


class Node
{
    public Token token;
    public ValueType value;
    //T value where T : ValueType;
}

 

    public void RunProgram()
    {
        string text = File.ReadAllText(@"C:\Users\Miro\source\repos\MyFirstCompiler\MyFirstCompiler\CompileThis.txt");
        string[] split = text.Split(' ');

        //Create Tree
        List<Node> allNodes= new List<Node>();
        foreach (string part in split)
        {
            if (!string.IsNullOrEmpty(part))
            {
                Node n = CreateNode(part);
                allNodes.Add(n);
            }
        }

        Console.WriteLine("finished");

    }

    public void CalculateExpression(List<Node> tree)
    {

    }


    public Node CreateNode(string part)
    {
        if (part == "+")
        {
            return new Node() { token = Token.Add, value = '+' };
        } else if (part == "*")
        {
            return new Node() { token = Token.Multiply, value = '*' };
        } else
        {
            return new Node() { token = Token.Number, value = int.Parse(part)}; 
        }
    }

    public void Test()
    {
        var n = new Node();
        n.value = '+';
        n.token = Token.Add;

    }
 */