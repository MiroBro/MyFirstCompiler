using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstCompiler
{
    internal class AssemblyTextCreator
    {
        private const string countCodeWord = "{REPLACE_WITH_COUNT}";
        private const string instructionCodeWord = "{CALCULATION_INSTRUCTION}";

        //private const string variableNameNumberA = $"numberA{countCodeWord}";
        private const string calcToDo = "{CALC_TO_PERFORM}";
        //private const string variableNameNumberB = $"numberB{countCodeWord}";

        private const string actualNumberA = "{REPLACE_NUMBER_A}";
        private const string actualNumberB = "{REPLACE_NUMBER_B}";
        //private const string putCalculationsHere = "{PUT_CALCULATIONS_HERE}";
        

        private const string replaceNumbersInDataSections = "{REPLACE_DATA_NUMBERS_HERE}";

        private const string calcMessageName = $"""
            message{countCodeWord}:
                db      '{calcToDo} = ', 10 ; db is defined byte
            message_end{countCodeWord}:
            """;

        /*
        private const string assemblyDataQuadNbrs = 
        $"""

            {variableNameNumberA} dq {actualNumberA}
            {variableNameNumberB} dq {actualNumberB}

            
        """;
        */
        /*
        private const string assemblyCalculation = $"""
    
            mov rax, [{variableNameNumberA}]
            mov rbx, [{variableNameNumberB}]
            {instructionCodeWord} rax,rbx

        """;
        */

        private const string assemblyData = $"""
    
            section .data
            divisor db 10 ; Divisor for division by 10
            {replaceNumbersInDataSections}
            remainder_array db 10 dup(0) ; Array to store remainders
            array_size equ 10
        message:
            db      '          ', 10 ; db is defined byte
        message_end:
            section .bss
        woho:
            resq  1   ;quadword - reserve space for EIGHT bytes which is ONE quadword

        """;

        private const string assemblyPrint = $"""
                sub     rsp, 8 ;It's so iternal stack is aligned to 16 bytes, if not doing this it would be misaligned with 8 bytes

                mov     rcx, 10 ; Divisor for division by 10
                lea     rdi, [message + array_size - 1] ; Set destination buffer (end of buffer)

            divide_loop{countCodeWord}:
                xor     rdx, rdx ; Clear remainder
                div     rcx ; Divide rax by 10
                add     dl, '0' ; Convert remainder to ASCII
                mov     [rdi], dl ; Store the digit

                dec     rdi ; Move to the previous position in the buffer

                test    rax, rax ; Check if quotient is zero
                jnz     divide_loop{countCodeWord} ; If not, continue the loop


            end_loop{countCodeWord}:

                ;TRY PRINTING CALCULATION
            
                add     rsp, 8
                mov     rcx, rax
            
                sub     rsp, 32 ;¨This is to reserve space for shadow stack space, you always reserve space for four variables
                mov     ecx,-11
                call    GetStdHandle 
            
                add    rsp, 32 ;¨This unreserves the space
            
                sub    rsp, 32 
                sub    rsp, 8+8 ;¨The first 8 is for the FIFTH parameret, but you cant just move it 8 because it needs to be 16 byte aligned when we call a function! 
                mov    rcx, rax
                lea    rdx, message{countCodeWord} ;lea = load effective adress
                mov    r8, message_end{countCodeWord} - message{countCodeWord}
                lea    r9, woho 
                mov    qword[rsp+4*8],0
            
                call   WriteFile
            
                add    rsp, 8+8
                add    rsp, 32   
            
                add     rsp, 8
                mov     rcx, rax

                ;PRINT RESULT

                sub     rsp, 32 ;¨This is to reserve space for shadow stack space, you always reserve space for four variables
                mov     ecx,-11
                call    GetStdHandle 

                add    rsp, 32 ;¨This unreserves the space

                sub    rsp, 32 
                sub    rsp, 8+8 ;¨The first 8 is for the FIFTH parameret, but you cant just move it 8 because it needs to be 16 byte aligned when we call a function! 
                mov    rcx, rax
                lea    rdx, message ;lea = load effective adress
                mov    r8, message_end - message
                lea    r9, woho 
                mov    qword[rsp+4*8],0

                call   WriteFile

                add    rsp, 8+8
                add    rsp, 32   
            
                add     rsp, 8
                mov     rcx, rax
            
            """;


        //This assembly works for NASM on Windows x64 bit
        private const string assemblyStart = """
                global WinMain
                extern  GetStdHandle
                extern  WriteFile
                extern  ExitProcess

                section .text

                ;On Wndows 64bit ABI (application binary interface) the first four parameters are passed in rcx,rdx,r8,r9 in that order. 
                ;The return from a functions is returned to rax
                ;In addition we are required to reserve space for shadow stack. It's custom to reserve 32 bytes in the shadow stack
            WinMain:
                sub     rsp, 8 ;It's so iternal stack is aligned to 16 bytes, if not doing this it would be misaligned with 8 bytes
            """;

        private const string assemblyEnd = """

                call    ExitProcess

                ; never here
                hlt
            """;


        //private List<Instruction> allInstruction = new List<Instruction>();
        private List<string> allInstruction = new List<string>();

        public List<string> theCalculationStrings = new List<string>();

        public void AddCalculationString(string calculationString) 
        { 
            theCalculationStrings.Add(calculationString);   
        }

        public void AddInstrucion(string instrucion)
        {
            allInstruction.Add(instrucion);
        }

        private class Instruction
        {
            public int numberA;
            public int numberB;
            public TokenType instruction;
            public string instructionString;
            public string instructionAssemblyString;

            public Instruction(int numberA, int numberB, TokenType instruction, string instructionString, string instructionAssemblyString)
            {
                this.numberA = numberA;
                this.numberB = numberB;
                this.instruction = instruction;
                this.instructionString = instructionString;
                this.instructionAssemblyString = instructionAssemblyString;
            }
        }

        public string GetAssemblyText() 
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(assemblyStart);
            sb.AppendLine(";HERE COME THE STACK CALCS");
            //calculate the actual assembly
            for (int i = 0; i < allInstruction.Count; i++)
            {
                sb.AppendLine(allInstruction[i]);

                //sb.Append(allInstruction[i].instructionAssemblyString);

                //sb.AppendLine(assemblyCalculation);

                //sb.Replace(actualNumberA, allInstruction[i].numberA.ToString());
                //sb.Replace(actualNumberB, allInstruction[i].numberB.ToString());

                //sb.Replace(countCodeWord,i.ToString());

                //sb = ReplaceInstruction(sb, allInstruction[i].instruction);
                
            }
            sb.AppendLine(";HERE THE STACK CALCS END!!!");
            sb.AppendLine(assemblyPrint);

            sb.AppendLine(assemblyEnd);
            sb.AppendLine(assemblyData);

            //Add the print
            StringBuilder sbData = new StringBuilder();
            for (int i = 0; i < allInstruction.Count; i++)
            {
                sbData.AppendLine(allInstruction[i]);
                //sbData.Replace(countCodeWord, i.ToString());
                //sbData.Replace(calcToDo, allInstruction[i].instructionString);
                //sbData.Replace(actualNumberA, allInstruction[i].numberA.ToString());
                //sbData.Replace(actualNumberB, allInstruction[i].numberB.ToString());
            }
            sbData.Replace(countCodeWord, "0");
            //sbData.AppendLine(assemblyDataQuadNbrs);
            sbData.AppendLine(calcMessageName);
            sb.Replace(replaceNumbersInDataSections, sbData.ToString());

            sb.Replace(calcToDo, theCalculationStrings[0].ToString());
            sb.Replace(countCodeWord, "0");
            return sb.ToString();
        }

        /*
        private StringBuilder ReplaceInstruction(StringBuilder sb, TokenType instruction)
        {
            switch (instruction)
            {
                case TokenType.Add:
                    sb.Replace(instructionCodeWord, "add");
                    break;
                case TokenType.Subtract:
                    sb.Replace(instructionCodeWord, "sub");
                    break;
                default:
                    sb.Replace(instructionCodeWord, "add");
                    break;
            }
            return sb; 
        }
        */

    }
}
