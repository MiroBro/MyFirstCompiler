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
    ;¨CALCULATIONS HERE
    ;¨CHANGE THIS
    ;¨Take the number in rcx, write out what it looks like as a string, make it so it can take ANY number
    ;¨I should not google the answer, no copy paste, sit and it doesnt work
    ;¨

    mov    byte[message], 0x34 ;move a 4 (written in hex) inte the first byte in message
    mov    byte[message +1], 0x32 ;move a 2 into the second byte in message

    lea    rcx, message+2 ;lea loaded in the adress of message byte place 3
    mov    rdx, 4 ;save the remaining amount of space in the message to loop through

loop:
    mov    byte[rcx], 0x20
    inc    rcx
    dec    rdx
    jnz    loop
;ÜNTIL THIS

    sub     rsp, 32 ;¨This is to reserve space for shadow stack space, you always reserve space for four variables
    mov     ecx,-11
    call    GetStdHandle ;¨TODO GOOGLE GetStdHandle!!!
    ; hStdOut = GetstdHandle( STD_OUTPUT_HANDLE)
    ; rax = handle (for standard out)  

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
    ; WriteFile( hstdOut, message, length(message), &bytes, 0);
    ;mov    rcx, rbx
    ;mov    rdx, ldamsg





    ;lea     rax, [ebp-4]
    ;push    rax
    ;push    (message_end - message)
    ;push    message
    ;push    rbx
    ;call    WriteFile

    ; ExitProcess(0)
    ;push    0
    add     rsp, 8
    mov     rcx, rax
    call    ExitProcess

    ; never here
    hlt



    section .data
message:
    db      'lalaaa', 10 ;¨db is defined byte
message_end:
    section .bss
woho:
    resq  1   ;quadword - reserve space for EIGHT bytes which is ONE quadword
