    global WinMain
    extern  GetStdHandle
    extern  WriteFile
    extern  ExitProcess

    section .text


;This assembly function will transform the value in rax into ASCII and put it into 'message'
IntToASCII:
    sub     rsp, 8 ;It's so iternal stack is aligned to 16 bytes, if not doing this it would be misaligned with 8 bytes

    mov     rcx, 10 ; Divisor for division by 10
    lea     rdi, [message + array_size - 1] ; Set destination buffer (end of buffer)

divide_loop_:
    xor     rdx, rdx ; Clear remainder
    div     rcx ; Divide rax by 10
    add     dl, '0' ; Convert remainder to ASCII
    mov     [rdi], dl ; Store the digit

    dec     rdi ; Move to the previous position in the buffer

    test    rax, rax ; Check if quotient is zero
    jnz     divide_loop_ ; If not, continue the loop


end_loop_:

    add     rsp, 8
	ret


;TODO: Add adress and length passed in (as in I put htem in THESE registers) - MAKE SURE SFTER EXPRESSION CALCULATOR PUTS IN RCX move it to the right register
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

    ;On Wndows 64bit ABI (application binary interface) the first four parameters are passed in rcx,rdx,r8,r9 in that order. 
    ;The return from a functions is returned to rax
    ;In addition we are required to reserve space for shadow stack. It's custom to reserve 32 bytes in the shadow stack
WinMain:
    sub     rsp, 8 ;It's so iternal stack is aligned to 16 bytes, if not doing this it would be misaligned with 8 bytes

;Start of invokation of stack calculations

   push    5 


   push    8 


    pop     rcx
    pop     rbx
    add     rcx, rbx
    push    rcx

    pop     rcx
    mov     rax, rcx

;End of stack calculations

    call    IntToASCII  

    ;prepping for printing calc
    lea    rdx, message0 ;lea = load effective adress
    mov    r8, message_end0 - message0
    call PrintString

    ;moving stuff so it can be used for printing
    lea    rdx, message ;lea = load effective adress
    mov    r8, message_end - message
    call PrintString

    ;mov     rcx, rax


    call    ExitProcess

    ; never here
    hlt

    section .data
    message0:
    db      '5 + 8 = ', 10 ; db is defined byte
message_end0:


    
    divisor db 10 ; Divisor for division by 10
    
    remainder_array db 10 dup(0) ; Array to store remainders
    array_size equ 10
message:
    db      '          ', 10 ; db is defined byte
message_end:
    section .bss
woho:
    resq  1   ;quadword - reserve space for EIGHT bytes which is ONE quadword


