using System;
using System.IO;
using x86CS.Configuration;

namespace x86CS
{
    public class Memory
    {
        private static readonly byte[] memory;

        public static bool A20 { get; set; }
        public static byte[] MemoryArray { get { return memory; } }

        static Memory()
        {
            memory = new byte[UnityManager.ins.MemorySize * 1024 * 1024];
        }

        public static void SegBlockWrite(ushort segment, ushort offset, byte[] buffer, int length)
        {
            var virtualPtr = (uint)((segment << 4) + offset);

            Memory.BlockWrite(virtualPtr, buffer, length);
        }

        public static void BlockWrite(uint addr, byte[] buffer, int length)
        {
            Buffer.BlockCopy(buffer, 0, memory, (int)addr, length);
        }

        public static int BlockRead(uint addr, byte[] buffer, int length)
        {
            Buffer.BlockCopy(memory, (int)addr, buffer, 0, length);

            return buffer.Length;
        }

        public static uint Read(uint addr, int size)
        {
            uint ret;
            bool passedMem = false;

            if (addr > MemoryArray.Length)
                passedMem = true;

            switch (size)
            {
                case 8:
                    if (passedMem)
                        ret = 0xff;
                    else
                        ret = memory[addr];
                    break;
                case 16:
                    if (passedMem)
                        ret = 0xffff;
                    else
                        ret = (ushort)(memory[addr] | memory[addr + 1] << 8);
                    break;
                default:
                    if (passedMem)
                        ret = 0xffffffff;
                    else
                        ret = (uint)(memory[addr] | memory[addr + 1] << 8 | memory[addr + 2] << 16 | memory[addr + 3] << 24);
                    break;
            }

            return ret;
        }

        public static void Write(uint addr, uint value, int size)
        {
            if (addr > MemoryArray.Length)
            {
                return;
            }

            switch (size)
            {
                case 8:
                    memory[addr] = (byte)value;
                    break;
                case 16:
                    memory[addr] = (byte)value;
                    memory[addr + 1] = (byte)((ushort)value).GetHigh();
                    break;
                default:
                    memory[addr] = (byte)value;
                    memory[addr + 1] = (byte)(value >> 8);
                    memory[addr + 2] = (byte)(value >> 16);
                    memory[addr + 3] = (byte)(value >> 24);
                    break;
            }
        }
    }
}
