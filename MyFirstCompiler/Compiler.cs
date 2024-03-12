// See https://aka.ms/new-console-template for more information //GipGap
using MyFirstCompiler;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;


internal class Compiler
{
   // private Dictionary<string, int> allSymbols = new Dictionary<string, int>();
    private static string nameOfAssembly = "run";
    private AssemblyTextCreator assemblyTextCreator;
    public static HashSet<string> allIntVariableNames = new HashSet<string>();

    public void Run(string[] expressionToCompile2)
    {
        List<string> expressionToCompile = new List<string>(File.ReadAllLines("input.txt"));

        assemblyTextCreator = new AssemblyTextCreator();

        foreach (var expression in expressionToCompile)
        {
            AssemblyTextCreator.DesiredCalc desiredCalculation = new AssemblyTextCreator.DesiredCalc() {expression = expression};
            EvaluateExpression(desiredCalculation);
            assemblyTextCreator.AddDesiredCalculation(desiredCalculation);
        }

        File.WriteAllText($"{nameOfAssembly}.asm", string.Empty);
        using (StreamWriter assemblyFile = File.AppendText($"{nameOfAssembly}.asm"))
        {
            assemblyFile.WriteLine(assemblyTextCreator.GetAssemblyText());
        };

        CreateExecutable();
    }
    private void EvaluateExpression(AssemblyTextCreator.DesiredCalc desiredCalculation)//string expression)
    {
        Tokenizer tokenizer = new Tokenizer(new StringReader(desiredCalculation.expression));
        List<Token> tokensInOrder = tokenizer.Tokenize();

        AssignmentPair assignmentPair = SplitIntoLHSandRHS(tokensInOrder);

        //check if the pair is a function delcaration? In that case.... do something strange I don't know D:
        //maybe like make a lfh shunting yard function algorithm??? (that checks if name is not a saved variable, so its a function .... and something)

        ShuntingYardAlgorithm sya = new ShuntingYardAlgorithm();
        assignmentPair.rhsPostSYA = sya.GetOutputQueue(assignmentPair.rhsPreSYA);

        CalculateOutputQueue(assignmentPair, desiredCalculation);
        PerformAssigmentIfProper(assignmentPair, desiredCalculation);
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

    private void PerformAssigmentIfProper(AssignmentPair assignmentPair, AssemblyTextCreator.DesiredCalc calcToDo)
    {
        if (assignmentPair.lhs.Count > 0)
        {
            string calc =   $"""

                                 mov     [{assignmentPair.lhs[0].valueName}], rax ; TRYING TO ASSIGN

                             """;
            calcToDo.AddInstrucion(calc);
        }
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
        if (encounteredAssignment && !allIntVariableNames.Contains(assignmentPair.lhs[0].valueName))
        {
            allIntVariableNames.Add(assignmentPair.lhs[0].valueName);
        } 

        //fix the expression if it wasnt an assignment
        if (assignmentPair.rhsPreSYA.Count == 0)
        {
            assignmentPair.rhsPreSYA = assignmentPair.lhs;
            assignmentPair.lhs = new List<Token>();
        }

        return assignmentPair;
    }

    public class AssignmentPair
    {
        public List<Token> lhs = new List<Token>();
        public List<Token> rhsPreSYA = new List<Token>();
        public Queue<Token> rhsPostSYA = new Queue<Token>();
    }

    private void CalculateOutputQueue(AssignmentPair assignmentPair, AssemblyTextCreator.DesiredCalc calcToDo)
    {
        foreach (var token in assignmentPair.rhsPostSYA)
        {
            if (token.tokenType == TokenType.Number)
            {
                string calc = $"""

                                push    {(int)token.value} 

                             """;
                calcToDo.AddInstrucion(calc);
            }
            else if (token.tokenType == TokenType.Symbol)
            {
                string calc = $"""

                                mov rax,[{token.valueName}] 
                                push    rax 

                             """;
                calcToDo.AddInstrucion(calc);
            }
            else
            {
                switch (token.tokenType)
                {
                    /*case TokenType.Assignment:
                        {
                            string calc =
                            $"""

                                 pop     rcx
                                 mov     {token.valueName}, rcx

                             """;

                            calcToDo.AddInstrucion(calc);
                        }
                        break;
                    */
                    case TokenType.Add:
                        {
                            string calc =
                            $"""

                                 pop     rcx
                                 pop     rbx
                                 add     rcx, rbx
                                 push    rcx

                             """;

                            calcToDo.AddInstrucion(calc);
                        }
                        break;
                    case TokenType.Subtract:
                        {
                            string calc =
                            $"""

                                 pop     rbx
                                 pop     rcx
                                 sub     rcx, rbx
                                 push    rcx

                             """;

                            calcToDo.AddInstrucion(calc);
                        }
                        break;
                    case TokenType.Negate:
                        {
                            string calc = // Cheating because I couldn't figure out how to multiply by -1 hahahah
                            $"""

                                 mov     rcx, 0 
                                 pop     rbx
                                 sub     rcx, rbx
                                 push    rcx

                             """;
                            calcToDo.AddInstrucion(calc);
                        }
                        break;
                    case TokenType.Divide:
                        {
                            string calc =
                            $"""

                                 mov     rdx, 0
                                 pop     rcx
                                 pop     rax
                                 div     rcx
                                 push    rax

                             """;
                            calcToDo.AddInstrucion(calc);
                        }
                        break;
                    case TokenType.Multiply:
                        {
                            string calc =
                            $"""

                                 pop     rcx
                                 pop     rax
                                 mul     rcx
                                 push    rax

                             """;
                            calcToDo.AddInstrucion(calc);
                        }
                        break;
                    case TokenType.Sin:
                        {
                        }
                        break;
                    case TokenType.Cos:
                        {
                        }
                        break;
                    case TokenType.Tan:
                        {
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
        calcToDo.AddInstrucion(finalCalc);
    }
}