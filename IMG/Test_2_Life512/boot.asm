bits 16
cpu 186
org 0x7c00

%define INITIAL_SP              0x7c00
%define SEGMENT_WORLD_STATE_A   0x1000
%define SEGMENT_WORLD_STATE_B   0x2000
%define COLOR_DEAD_CELL         0x281f1f
%define COLOR_LIVE_CELL         0x17d7d7
%define STEPS_PER_SECOND        20
%define LIVE_CELL_TO_LIVE_CELL  0b00001100
%define DEAD_CELL_TO_LIVE_CELL  0b00001000
%define WORLD_WIDTH             256 ; 地图宽度
%define WORLD_HEIGHT            256 ; 地图高度
%define SEGMENT_VGA_RAM         0xA000
%define CLOCK_FREQUENCY_HZ      1193180
%define MAX_FREQUENCY_DIVISOR   65535
%define MAKEWORD(HI, LO) (((HI) & 0xff) << 8 | ((LO) & 0xff))

%macro LOAD_STANDARD_SS 0
    xor     ax, ax
    mov     ss, ax
%endmacro

;=================================================================================
;=================================== 主代码在此 ====================================
;=================================================================================

%macro INITIALIZE_VIDEO 0
    mov     ax, 0x0012  ; 设置视频模式为640x400 16色
    int     0x10
    
    ; 设置调色板
    mov     dx, 0x3C8  ; 调色板索引寄存器端口
    mov     al, 0      ; 开始索引
    out     dx, al
    inc     dx         ; 调色板数据寄存器端口

    ; 设置死细胞颜色
    mov     al, 0x3f & (COLOR_DEAD_CELL >> 18)
    out     dx, al
    mov     al, 0x3f & (COLOR_DEAD_CELL >> 10)
    out     dx, al
    mov     al, 0x3f & (COLOR_DEAD_CELL >> 2)
    out     dx, al

    ; 设置活细胞颜色
    mov     al, 0x3f & (COLOR_LIVE_CELL >> 18)
    out     dx, al
    mov     al, 0x3f & (COLOR_LIVE_CELL >> 10)
    out     dx, al
    mov     al, 0x3f & (COLOR_LIVE_CELL >> 2)
    out     dx, al
%endmacro

%macro INITIALIZE_WORLD 0
    mov     al, 0x80
    out     0x70, al
    in      al, 0x71
    mov     dl, al
    mov     al, 0x82
    out     0x70, al
    in      al, 0x71
    mov     dh, al
    mov     ax, SEGMENT_WORLD_STATE_A
    mov     es, ax
    xor     di, di
    xor     cx, cx
%%Loop:
    mov     bl, cl
    mov     ax, dx
    mov     cl, 7
    shl     ax, cl
    xor     dx, ax
    mov     ax, dx
    mov     cl, 9
    shr     ax, cl
    xor     dx, ax
    mov     ax, dx
    dec     cx
    shl     ax, cl
    xor     dx, ax
    mov     al, dl
    and     al, 1
    stosb
    mov     cl, bl
    loop    %%Loop
%endmacro

%macro INITIALIZE_TIMER 0
    mov     al, 0b00_11_010_0
    out     0x43, al
    mov     ax, MAX_FREQUENCY_DIVISOR / STEPS_PER_SECOND
    out     0x40, al
    mov     al, ah
    out     0x40, al
%endmacro

%macro TICK_AND_RENDER 0
    mov     ax, SEGMENT_VGA_RAM
    mov     ss, ax
    xor     cx, cx
    xor     di, di
%%LoopCells:
    mov     bx, cx
    mov     ah, [bx]
    mov     al, 0
    add     al, [bx - 1]
    add     al, [bx + 1]
    dec     bh
    add     al, [bx - 1]
    add     al, [bx]
    add     al, [bx + 1]
    add     bh, 2
    add     al, [bx - 1]
    add     al, [bx]
    add     al, [bx + 1]
    mov     bx, cx
    mov     cl, al
    mov     al, DEAD_CELL_TO_LIVE_CELL
    test    ah, ah
    jz      %%CellCurrentlyDead
    mov     al, LIVE_CELL_TO_LIVE_CELL
%%CellCurrentlyDead:
    shr     al, cl
    and     al, 1
    stosb
    mov     ch, ah
    mov     al, bh
    mov     ah, 0
    add     ax, 0 ;72 ;(VIDEO_HEIGHT - WORLD_HEIGHT) / 2
    mov     dx, 80 ;80 ;VIDEO_WIDTH / 8
    mul     dx
    mov     si, ax
    mov     al, bl
    mov     ah, 0
    add     ax, 0 ;192 ;(VIDEO_WIDTH - WORLD_WIDTH) / 2
    mov     cl, 3
    shr     ax, cl
    add     si, ax
    mov     al, 0x80
    mov     cl, bl
    and     cl, 7
    shr     al, cl
    mov     ah, [ss:si]
    test    ch, ch
    jnz     %%SetCellLive
    not     al
    and     ah, al
    jmp     %%WritePixel
%%SetCellLive:
    or      ah, al
%%WritePixel:
    mov     [ss:si], ah
    mov     cx, bx
    inc     cx
    jnz     %%LoopCells
    LOAD_STANDARD_SS
%endmacro

Start:
    cli
    cld
    LOAD_STANDARD_SS
    mov     sp, INITIAL_SP
    INITIALIZE_VIDEO
    INITIALIZE_TIMER
    INITIALIZE_WORLD
    mov     cx, SEGMENT_WORLD_STATE_A
    mov     dx, SEGMENT_WORLD_STATE_B
.MainLoop:
    mov     ds, cx
    mov     es, dx
    TICK_AND_RENDER
    mov     cx, es
    mov     dx, ds
    mov     al, CLOCK_FREQUENCY_HZ / MAX_FREQUENCY_DIVISOR
.TimerDivider:
    sti
    hlt
    cli
    dec     ax
    jnz     .TimerDivider
    jmp     .MainLoop

    times (510 - ($ - $$)) db 0
    dw 0xaa55  ; 引导扇区签名