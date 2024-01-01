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
    ;CHANGE THIS
    ;Take the number in rcx, write out what it looks like as a string, make it so it can take ANY number
    ;I should not google the answer, no copy paste, sit and it doesnt work
    ; 0 in hex is 0x30

    mov rax, [number]
    mov rbx, [number2]
    add rax,rbx

    ;mov     rax, [number]
    mov     rcx, 10 ; Divisor for division by 10
    lea     rdi, [message + array_size - 1] ; Set destination buffer (end of buffer)

divide_loop:
    xor     rdx, rdx ; Clear remainder
    div     rcx ; Divide rax by 10
    add     dl, '0' ; Convert remainder to ASCII
    mov     [rdi], dl ; Store the digit
    ;mov     [rdi], 0x30+rdx ; Store the digit
    dec     rdi ; Move to the previous position in the buffer

    test    rax, rax ; Check if quotient is zero
    jnz     divide_loop ; If not, continue the loop


end_loop:

    sub     rsp, 32 ;¨This is to reserve space for shadow stack space, you always reserve space for four variables
    mov     ecx,-11
    call    GetStdHandle ;¨TODO GOOGLE GetStdHandle!!!

    add    rsp, 32 ;¨This unreserves the space

    sub    rsp, 32 
    sub    rsp, 8+8 ;¨The first 8 is for the FIFTH parameret, but you cant just move it 8 because it needs to be 16 byte aligned when we call a function! 
    mov    rcx, rax
    ;lea    rdx, message ;lea = load effective adress
    lea    rdx, message ;lea = load effective adress
    mov    r8, message_end - message
    ;mov    r8, message_end - message
    lea    r9, woho 
    mov    qword[rsp+4*8],0

    call   WriteFile

    add    rsp, 8+8
    add    rsp, 32   

    add     rsp, 8
    mov     rcx, rax

    ;TRYING TO DUPLICATE HERE

    sub     rsp, 8 ;It's so iternal stack is aligned to 16 bytes, if not doing this it would be misaligned with 8 bytes

    mov     rax, [number2]
    mov     rcx, 10 ; Divisor for division by 10
    lea     rdi, [message + array_size - 1] ; Set destination buffer (end of buffer)

divide_loop2:
    xor     rdx, rdx ; Clear remainder
    div     rcx ; Divide rax by 10
    add     dl, '0' ; Convert remainder to ASCII
    mov     [rdi], dl ; Store the digit

    dec     rdi ; Move to the previous position in the buffer

    test    rax, rax ; Check if quotient is zero
    jnz     divide_loop2 ; If not, continue the loop


end_loop2:

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
    number dq 124
    number2 dq 356
    remainder_array db 10 dup(0) ; Array to store remainders
    array_size equ 10
message:
    db      '          ', 10 ;¨db is defined byte
message_end:
    section .bss
woho:
    resq  1   ;quadword - reserve space for EIGHT bytes which is ONE quadword
