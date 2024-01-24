﻿// See https://aka.ms/new-console-template for more information //GipGap
using MyFirstCompiler;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;


internal class Compiler
{
    private static string nameOfAssembly = "calculation";

    private AssemblyTextCreator assemblyTextCreator;

    public void Run(string[] expressionToCompile)
    {
        assemblyTextCreator = new AssemblyTextCreator();

        foreach (var expression in expressionToCompile)
        {
            assemblyTextCreator.AddCalculationString(expression);
            EvaluateExpression(expression);
        }

        File.WriteAllText($"{nameOfAssembly}.asm", string.Empty);
        using (StreamWriter assemblyFile = File.AppendText($"{nameOfAssembly}.asm"))
        {
            assemblyFile.WriteLine(assemblyTextCreator.GetAssemblyText());
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

    private double CalculateOutputQueue(Queue<Token> outputQueue)
    {
        Stack<double> stack = new Stack<double>();

        foreach (var token in outputQueue)
        {
            if (token.tokenType == TokenType.Number)
            {
                stack.Push(token.value);

                string calc = $"""
                             push    {(int)token.value} 

                             """;
                assemblyTextCreator.AddInstrucion(calc);
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

                            string calc =
                            $"""
                             pop     rcx
                             pop     rbx
                             add     rcx, rbx
                             push    rcx

                             """;

                            assemblyTextCreator.AddInstrucion(calc);
                        }
                        break;
                    case TokenType.Subtract:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = b - a;

                            string calc =
                            $"""
                             pop     rbx
                             pop     rcx
                             sub     rcx, rbx
                             push    rcx

                             """;

                            assemblyTextCreator.AddInstrucion(calc);

                            stack.Push(r);
                        }
                        break;
                    case TokenType.Negate:
                        {
                            var a = stack.Pop();
                            var r = -a;
                            stack.Push(r);
                            string calc = // Cheating because I couldn't figure out how to multiply by -1 hahahah
                            $"""
                             mov     rcx, 0 
                             pop     rbx
                             sub     rcx, rbx
                             push    rcx

                             """;
                            assemblyTextCreator.AddInstrucion(calc);
                        }
                        break;
                    case TokenType.Divide:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = a / b;
                            stack.Push(r);
                            string calc =
                            $"""
                             mov     rdx, 0
                             pop     rcx
                             pop     rax
                             div     rcx
                             push    rax

                             """;
                            assemblyTextCreator.AddInstrucion(calc);
                        }
                        break;
                    case TokenType.Multiply:
                        {
                            var a = stack.Pop();
                            var b = stack.Pop();
                            var r = a * b;
                            stack.Push(r);
                            string calc =
                            $"""
                             pop     rcx
                             pop     rax
                             mul     rcx
                             push    rax

                             """;
                            assemblyTextCreator.AddInstrucion(calc);
                        }
                        break;
                    case TokenType.Sin:
                        {
                            var a = stack.Pop();
                            var r = Math.Sin((a * Math.PI) / 180);
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
        string finalCalc =
            $"""
            pop     rcx
            mov     rax, rcx
            """;
        assemblyTextCreator.AddInstrucion(finalCalc);
        return stack.Pop();
    }
}