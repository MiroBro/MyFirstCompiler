using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstCompiler
{
    internal class AssemblyTextCreator
    {
        private const string countCodeWord = "{REPLACE_WITH_COUNT}";

        private const string calcToDo = "{CALC_TO_PERFORM}";

        private const string replaceNumbersInDataSections = "{REPLACE_DATA_NUMBERS_HERE}";

        private const string variableCodeWord = "{REPLACE_WITH_VARIABLE_NAME}";

        private const string calcMessageName = $"""
            message{countCodeWord}:
                db      '{calcToDo} = ', 10 ; db is defined byte
            message_end{countCodeWord}:
            """;

        private const string variableNameData = $"""
            {variableCodeWord}: dq 0
            """;


        private const string calcData = $"""
            {replaceNumbersInDataSections}
        """;

        private const string beginningOfData = $"""
    
            section .data
        """;
        private const string standardData = $"""
    
            
            divisor db 10 ; Divisor for division by 10

            newline db 10  ; newline character (\n)
            newline_len equ $ - newline  ; length of the newline character

            remainder_array db 10 dup(0) ; Array to store remainders
            array_size equ 10
        message:
            db      '          ', 10 ; db is defined byte
        message_end:
            section .bss
        woho:
            resq  1   ;quadword - reserve space for EIGHT bytes which is ONE quadword

        """;

        private const string printCalcAndResult = $"""
                call    IntToASCII  

                ;prepping for printing calc
                lea    rdx, message{countCodeWord} ;lea = load effective adress
                mov    r8, message_end{countCodeWord} - message{countCodeWord}
                call PrintString

                ;moving stuff so it can be used for printing
                lea    rdx, message ;lea = load effective adress
                mov    r8, message_end - message
                call PrintString
                call PrintNewLine
                call ResetASCIIInt

                ;mov     rcx, rax
            
            """;        
        
        private const string resetASCII = $"""

            ResetASCIIInt:
            _start:
                mov esi, message        ; Move the address of message into esi
                mov edi, esi            ; Save the start address of message in edi
                add edi, 10             ; Calculate the end address of message

            fill_loop:
                cmp esi, edi            ; Compare current address with end address
                je loop_end             ; If they are equal, exit the loop

                mov byte [esi], 0       ; Set the byte at the address pointed to by esi to null
                inc esi                 ; Move to the next byte in the message
                jmp fill_loop           ; Jump back to fill_loop

            loop_end:
                ; mov byte [esi], 0       ; Null-terminate the string   

            """;

        private const string printNewLine = $"""

            PrintNewLine:

                lea     rdx, [newline]   ; address of newline character
                mov     r8, newline_len  ; length of newline character
                call    PrintString

            """;


        //This assembly works for NASM on Windows x64 bit
        private const string assemblyStart = """
                global WinMain
                extern  GetStdHandle
                extern  WriteFile
                extern  ExitProcess

                section .text

            """;

        private const string winMainStart = """
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

        private const string intToASCIIFunction = """

            ;This assembly function will transform the value in rax into ASCII and put it into 'message'
            IntToASCII:
                sub     rsp, 8 ;It's so iternal stack is aligned to 16 bytes, if not doing this it would be misaligned with 8 bytes

                mov     rcx, 10 ; Divisor for division by 10
                lea     rdi, [message + array_size - 1] ; Set destination buffer (end of buffer)

                cmp     rax, 0 ; compare input number to zero
                jl      is_negative ; if negative jump to adding -
                jmp     not_negative ; if not negative dont add -

            is_negative:
                mov     rbx, 1 ; set rbx to 1 indicating a negative number
                neg     rax     ; negate rax to make it positive
                jmp     divide_loop_ ; jump to divide loop to start converting to ASCII

            not_negative:
                mov     rbx, 0 ; set rbx to 1 indicating a negative number
                ; continue without adding the negative sign

            divide_loop_:
                xor     rdx, rdx ; Clear remainder
                div     rcx ; Divide rax by 10
                add     dl, '0' ; Convert remainder to ASCII
                mov     [rdi], dl ; Store the digit

                dec     rdi ; Move to the previous position in the buffer

                test    rax, rax ; Check if quotient is zero
                jnz     divide_loop_ ; If not, continue the loop

            add_negative_sign:
                test    rbx,rbx
                jz      end_loop_

                mov     byte[rdi], '-' ;add negative sign to beginning of buffer

            end_loop_:
                add     rsp, 8
            	ret

            """;

        private const string printString = """

           ;Assumes the adress of the message being printed is stored in rdx, and assumes the length of the message is stored in r8 
           PrintString:
               sub     rsp, 8
               sub     rsp, 32 ;¨This is to reserve space for shadow stack space, you always reserve space for four variables
               mov     ecx,-11
               call    GetStdHandle 

               add    rsp, 32 ;¨This unreserves the space

               sub    rsp, 32 
               sub    rsp, 8+8 ;¨The first 8 is for the FIFTH parameret, but you cant just move it 8 because it needs to be 16 byte aligned when we call a function! 
               mov    rcx, rax
               ;lea    rdx, message ;lea = load effective adress
               ;mov    r8, message_end - message
               lea    r9, woho ; this stores the lenght of the file that it wrote - used to CHECK, CHANGE WOHO name to something sensible
               mov    qword[rsp+4*8],0

               call   WriteFile

               add    rsp, 8+8
               add    rsp, 32   

               add     rsp, 8
           	   ret

           """;

        public List<DesiredCalc> calcToDos = new List<DesiredCalc>();

        public void AddDesiredCalculation(DesiredCalc desiredCalc)
        {
            calcToDos.Add(desiredCalc);
        }

        public class DesiredCalc 
        {
            public string expression = "";
            public List<string> allInstructions = new List<string>();

            public void AddInstrucion(string instrucion) 
            {
                allInstructions.Add(instrucion);
            }
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
            sb.AppendLine(intToASCIIFunction);
            sb.AppendLine(resetASCII);
            sb.AppendLine(printNewLine);
            sb.AppendLine(printString);
            sb.AppendLine(winMainStart);

            for (int i = 0; i < calcToDos.Count; i++)
            {
                sb.AppendLine("\n;Start of invokation of stack calculations");
                //calculate the actual assembly
                for (int j = 0; j < calcToDos[i].allInstructions.Count; j++)
                {
                    sb.AppendLine(calcToDos[i].allInstructions[j]);
                }
                sb.AppendLine("\n;End of stack calculations\n");

                sb.AppendLine(printCalcAndResult);

                sb.Replace(calcToDo, calcToDos[i].expression);
                sb.Replace(countCodeWord, i.ToString());
            }

            sb.AppendLine(assemblyEnd);
            sb.AppendLine(beginningOfData);

            for (int i = 0; i < calcToDos.Count; i++)
            {
                sb.AppendLine(calcData);

                StringBuilder sbData = new StringBuilder();
                sbData.AppendLine(calcMessageName);
                //ADD DICTIORNAY OF THINGS TO ADD HERE
                sbData.Replace(countCodeWord, i.ToString());
                sbData.Replace(calcToDo, calcToDos[i].expression);
                sb.Replace(replaceNumbersInDataSections, sbData.ToString());
            }

            foreach (var varName in Compiler.allIntVariableNames)
            {
                sb.AppendLine(variableNameData);
                sb.Replace(variableCodeWord, varName);
            }

            sb.AppendLine(standardData);

            return sb.ToString();
        }
    }
}
