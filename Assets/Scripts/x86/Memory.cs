﻿using System;
using System.Runtime.InteropServices;
using System.IO;
using x86CS.Configuration;

namespace x86CS
{
    public class Memory
    {
        private static byte[] memory;

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
            UnityEngine.Debug.Log(String.Format("Block write {0:X} length {1:X} ends {2:X}", addr, length, addr + length));
            int int_addr = (int)addr;
            UnityEngine.Debug.Log("addr: " + int_addr);

            // 确保目标内存区域足够大，以便容纳要复制的数据
            if (memory.Length - int_addr < length)
            {
                UnityEngine.Debug.Log("内存大小："+ memory.Length);
                UnityEngine.Debug.Log("没有足够空间，尝试重新分配");
                memory = new byte[UnityManager.ins.MemorySize * 1024 * 1024];
            }
            else
            {
                UnityEngine.Debug.Log("内存足够");
            }

            Buffer.BlockCopy(buffer, 0, memory, int_addr, length);
        }

        public static int BlockRead(uint addr, byte[] buffer, int length)
        {
            Buffer.BlockCopy(memory, (int)addr, buffer, 0, length);

            UnityEngine.Debug.Log(String.Format("Block read {0:X} length {1:X} ends {2:X}", addr, length, addr + length));

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

            UnityEngine.Debug.Log(String.Format("Read {0} address {1:X} value {2:X}{3}", size, addr, ret, passedMem ? " (OverRead)" : ""));

            return ret;
        }

        public static void Write(uint addr, uint value, int size)
        {
            if (addr > MemoryArray.Length)
            {
                UnityEngine.Debug.Log(String.Format("Write {0} address {1:X} value {2:X} (OverWrite, ignored)", size, addr, value));
                return;
            }

            UnityEngine.Debug.Log(String.Format("Write {0} address {1:X} value {2:X}", size, addr, value));

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
