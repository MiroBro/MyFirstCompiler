; program 3.1
; sample Assembly program - MASM (64-bit)

extern ExitProcess:PROC
public mainCRTStartup

section .data
    string1 db  0xa, "  Hello This is great!!!", 0xa, 0xa, 0

;.code
mainCRTStartup PROC       ; Use mainCRTStartup if making a CONSOLE app without
                          ;   any C/C++ files AND if you haven't overridden the
                          ;   ENTRY point in the Visual Studio Project.

  sub rsp, 8+32           ; Align stack on 16 byte boundary and allocate 32 bytes
                          ;   of shadow space for call to ExitProcess. Shadow space
                          ;   is required for 64-bit Windows Calling Convention as
                          ;   is ensuring the stack is aligned on a 16 byte boundary
                          ;   at the point of making a call to a C library function
                          ;   or doing a WinAPI call.
  mov rcx, 7

          ; calculate the length of string
        mov     rdi, string1        ; string1 to destination index
        xor     rcx, rcx            ; zero rcx
        not     rcx                 ; set rcx = -1
        xor     al,al               ; zero the al register (initialize to NUL)
        cld                         ; clear the direction flag
        repnz   scasb               ; get the string length (dec rcx through NUL)
        not     rcx                 ; rev all bits of negative results in absolute value
        dec     rcx                 ; -1 to skip the null-terminator, rcx contains length
        mov     rdx, rcx            ; put length in rdx
        ; write string to stdout
        mov     rsi, string1        ; string1 to source index
        mov     rax, 1              ; set write to command
        mov     rdi,rax             ; set destination index to rax (stdout)
        syscall                     ; call kernel

        ; exit 
        xor     rdi,rdi             ; zero rdi (rdi hold return value)
        mov     rax, 0x3c           ; set syscall number to 60 (0x3c hex)
        syscall                     ; call kernel

  ;xor rcx, rcx            ; Set Error Code to 0 (RCX is 1st parameter)
  call ExitProcess
mainCRTStartup ENDP
END