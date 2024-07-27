nasm -f bin boot.asm -o boot.bin
cat boot.bin > floppy.img
dd if=/dev/zero bs=1 count=$((512*2880-$(stat -c%s floppy.img))) >> floppy.img
