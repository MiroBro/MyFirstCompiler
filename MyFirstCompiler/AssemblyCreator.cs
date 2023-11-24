using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MyFirstCompiler
{
    //using https://sharplab.io/#v2:C4LglgNgNAJiDUAfAAgJgIwFgBQyDMABGgQMIEDeOB1RhyALAQLIAUAlBVTdwG4CGAJwJ8CAXgLoAdAFYAZgG4u3avyEAjMQVQyFS5aoIBjTSPgE1i7Mpp7eggjE1S5l6yvsBTTdpe2aB2U1HAFoCD1c3P3chAHMnHQjrAwALbwSoggMwTTiAKgJkxOUMgwAreN8rNwMAazTKt0z7CE1SgHoaou4okvsAWwrdKqT7ADt6ocaDAHtNPtymPmBkgDFJAGUwUZY+ti6bYYIAXxwjoA=
    //as a reference for IL-instructions.
    public class AssemblyCreator
    {
        private string fileName;
        public int countOfLineIL = 0;
        public int storedVariables = 0;

        public AssemblyCreator(string assemblySuffix, string expressionAssemblied)
        {
            fileName = $"AwesomeAssembly_{assemblySuffix}.txt";
            CreateAsseblyFile(expressionAssemblied);
        }

        private void CreateAsseblyFile(string expressionAssemblied)
        {   
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (FileStream fs = File.Create(fileName)){}

            WriteAssemblyTitle(expressionAssemblied);
        }
        private void WriteAssemblyTitle(string expressionAssemblied)
        {
            string[] text = { 
                "//Amazing assembly produced by NICE compiler \n",
                $"//Expression being assemblied below: {expressionAssemblied}\n"};
            File.AppendAllLines(fileName, text);
        }

        //private void AppendAssemblyInstructions(string[] instructions)
        //{
        //    File.AppendAllLines(fileName, instructions);
        //}


        public void WriteInstruction(TokenType instructionType, double a)
        {
            var storeIndexOfA = storedVariables++;
            var storeIndexOfResults = storedVariables++;

            string[] instructions = {
                $"//{instructionType} instruction",
                $"{GetNextILRow()}: ldc.r4 {a}",
                $"{GetNextILRow()}: stloc.{storeIndexOfA}",

                $"{GetNextILRow()}: ldloc.{storeIndexOfA}",
                $"{GetNextILRow()}: {GetInstructionString(instructionType)}",
                $"{GetNextILRow()}: stloc.{storeIndexOfResults}\n",
            };

            File.AppendAllLines(fileName, instructions);
        }
            public void WriteInstruction(TokenType instructionType, double a, double b)
        {
            var storeIndexOfA = storedVariables++;
            var storeIndexOfB = storedVariables++;
            var storeIndexOfResults = storedVariables++;

            string[] instructions = { 
                $"//{instructionType} instruction",
                $"{GetNextILRow()}: ldc.r4 {a}",
                $"{GetNextILRow()}: stloc.{storeIndexOfA}",

                $"{GetNextILRow()}: ldc.r4 {b}",
                $"{GetNextILRow()}: stloc.{storeIndexOfB}",

                $"{GetNextILRow()}: ldloc.{storeIndexOfA}",
                $"{GetNextILRow()}: ldloc.{storeIndexOfB}",
                $"{GetNextILRow()}: {GetInstructionString(instructionType)}",

                $"{GetNextILRow()}: stloc.{storeIndexOfResults}\n",
            };

            File.AppendAllLines(fileName, instructions);

        }

        public void FinishWritingAssembly()
        {
            string[] instructions = {
                $"{GetNextILRow()}: ret",
            };

            File.AppendAllLines(fileName, instructions);
        }

        private string GetInstructionString(TokenType instructionType)
        {
            switch (instructionType)
            {
                case TokenType.Add:
                    return "add";
                case TokenType.Subtract:
                    return "sub";
                case TokenType.Multiply:
                    return "mul";
                case TokenType.Divide:
                    return "div";
                case TokenType.Sin:
                    return "call float32 [System.Runtime]System.MathF::Sin(float32)";
                case TokenType.Cos:
                    return "call float32 [System.Runtime]System.MathF::Cos(float32)";
                case TokenType.Tan:
                    return "call float32 [System.Runtime]System.MathF::Tan(float32)";
                default:
                    return "ERROR - no matching instruction";
            }
        }

        private string GetNextILRow() 
        {
            return $"IL_{countOfLineIL++.ToString("D4")}";
        }
    }
}
