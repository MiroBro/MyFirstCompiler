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

    mov rax, [numberA0]
    mov rbx, [numberB0]
    add rax,rbx

    sub     rsp, 8 ;It's so iternal stack is aligned to 16 bytes, if not doing this it would be misaligned with 8 bytes

    mov     rcx, 10 ; Divisor for division by 10
    lea     rdi, [message + array_size - 1] ; Set destination buffer (end of buffer)

divide_loop0:
    xor     rdx, rdx ; Clear remainder
    div     rcx ; Divide rax by 10
    add     dl, '0' ; Convert remainder to ASCII
    mov     [rdi], dl ; Store the digit

    dec     rdi ; Move to the previous position in the buffer

    test    rax, rax ; Check if quotient is zero
    jnz     divide_loop0 ; If not, continue the loop


end_loop0:

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
    lea    rdx, message0 ;lea = load effective adress
    mov    r8, message_end0 - message0
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


    mov rax, [numberA1]
    mov rbx, [numberB1]
    add rax,rbx

    sub     rsp, 8 ;It's so iternal stack is aligned to 16 bytes, if not doing this it would be misaligned with 8 bytes

    mov     rcx, 10 ; Divisor for division by 10
    lea     rdi, [message + array_size - 1] ; Set destination buffer (end of buffer)

divide_loop1:
    xor     rdx, rdx ; Clear remainder
    div     rcx ; Divide rax by 10
    add     dl, '0' ; Convert remainder to ASCII
    mov     [rdi], dl ; Store the digit

    dec     rdi ; Move to the previous position in the buffer

    test    rax, rax ; Check if quotient is zero
    jnz     divide_loop1 ; If not, continue the loop


end_loop1:

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
    lea    rdx, message1 ;lea = load effective adress
    mov    r8, message_end1 - message1
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


    call    ExitProcess

    ; never here
    hlt

    section .data
    divisor db 10 ; Divisor for division by 10
    
    numberA0 dq 2
    numberB0 dq 1

    
message0:
    db      '2 + 1  = ', 10 ; db is defined byte
message_end0:

    numberA1 dq 5
    numberB1 dq 2

    
message1:
    db      '5 + 2  = ', 10 ; db is defined byte
message_end1:

    remainder_array db 10 dup(0) ; Array to store remainders
    array_size equ 10
message:
    db      '          ', 10 ; db is defined byte
message_end:
    section .bss
woho:
    resq  1   ;quadword - reserve space for EIGHT bytes which is ONE quadword


