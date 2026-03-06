; ml64 /c Example.asm && link Example.obj /subsystem:console /entry:Main kernel32.lib
; In case u want to compile it urself n test it ^

extern GetStdHandle:proc
extern WriteConsoleA:proc
extern ExitProcess:proc

.data

Message         db "Hello there :D", 13, 10, 0
MessageLength   equ $ - Message - 1
Written         dd 0

.code

Main proc
    sub rsp, 38h

    ; GetStdHandle(STD_OUTPUT_HANDLE)
    mov ecx, -11
    call GetStdHandle
    mov rbx, rax

    ; WriteConsoleA(Handle, &Message, Length, &Written, NULL)
    mov rcx, rbx
    lea rdx, [Message]
    mov r8d, MessageLength
    lea r9, [Written]
    mov qword ptr [rsp + 20h], 0
    call WriteConsoleA

    ; ExitProcess(0)
    xor ecx, ecx
    call ExitProcess

Main endp

end
