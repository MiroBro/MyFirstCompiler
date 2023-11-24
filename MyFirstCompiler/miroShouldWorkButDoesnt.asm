; program 3.1
; sample Assembly program - MASM (64-bit)

extern ExitProcess:PROC
public mainCRTStartup

.data
;sum DWORD 0

.code
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
  ;mov ebx, 2
  ;add ebx, ebx
  ;mov sum, eax

  ;xor rcx, rcx            ; Set Error Code to 0 (RCX is 1st parameter)
  call ExitProcess
mainCRTStartup ENDP
END