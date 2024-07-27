bits 16
cpu 186
org 0x7c00

mov ax, 0B800h ; 文本模式缓冲区地址
mov es, ax

xor bx, bx     ; bx = 0，用于偏移计算

reset_cursor:
mov ah, 02h    ; 设置光标位置的BIOS功能号
mov bh, 0      ; 页号，通常为0
mov dh, 0      ; 行号
mov dl, 0      ; 列号
int 10h        ; 调用BIOS中断10h，设置光标位置

randomize_screen:
; 生成随机颜色和字符
mov ah, 0h     ; 随机数生成功能号
int 1Ah        ; 调用BIOS中断1Ah，获取系统时钟计数作为随机数种子
xor dx, dx     ; dx = 0
mov cx, 2000   ; 填充整个屏幕的像素数 (80 * 25 * 2)
mov di, 0      ; es:di 指向缓冲区

fill_screen:
; 生成随机颜色
mov ax, dx     ; 使用dx作为随机数种子
imul ax, ax, 1103515245
add ax, 12345
mov dx, ax     ; 更新dx为新的随机数

; 随机颜色范围为0-15，其中高4位是背景色，低4位是前景色
mov ah, dl
and ah, 0Fh    ; 仅保留低4位

; 随机字符范围为32-126 (可打印ASCII字符)
mov al, dl
and al, 7Fh    ; 仅保留7位

mov word [es:di], ax ; 将字符和颜色写入显存
add di, 2       ; 每个字符占用两个字节

loop fill_screen ; 循环直到填充完整个屏幕

jmp $           ; 无限循环，保持显示屏上的随机字符

times 510-($-$$) db 0  ; 填充剩余的空间，使总大小为512字节
dw 0xAA55       ; 主引导记录标志
