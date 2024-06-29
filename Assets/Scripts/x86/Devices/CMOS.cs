using System;
using x86CS.Configuration;
using x86CS.ATADevice;
using System.Collections.Generic;

namespace x86CS.Devices
{
    public class CMOS : IDevice
    {
        private readonly int[] portsUsed = {0x70, 0x71};
        private byte currentReg;
        private byte statusA;
        private byte statusB;
        private byte statusC;
        private byte statusD;
        private ATA ataDevice;

        public int[] PortsUsed
        {
            get { return portsUsed; }
        }

        public CMOS(ATA ata)
        {
            statusA = 0x26; /* 默认的32.768分频器和默认的速率选择 */
            statusB = 0x02;  /* 无夏令时，12小时制，BCD（二进制编码十进制），所有标志位清零。 */
            statusC = 0x00;
            statusD = 0x80;

            ataDevice = ata;
        }

        public uint Read(ushort addr, int size)
        {
            // 获取当前时间
            DateTime currTime = DateTime.Now;
            // 初始化返回值
            ushort ret = 0;

            // 根据地址进行处理
            switch (addr)
            {
                case 0x70:
                    UnityEngine.Debug.Log("只写");

             ret = 0xff;
                    break;
                case 0x71:
                    switch (currentReg)
                    {
                        case 0x00:
                            UnityEngine.Debug.Log("返回当前时间的秒数");
                            return Util.ToBCD(currTime.Second); 
                        case 0x02:
                            UnityEngine.Debug.Log("返回当前时间的分钟数");
                            return Util.ToBCD(currTime.Minute); 
                        case 0x04:
                            UnityEngine.Debug.Log("返回当前时间的小时数");
                            return Util.ToBCD(currTime.Hour); 
                        case 0x06:
                            UnityEngine.Debug.Log("返回当前时间的星期几");
                            return Util.ToBCD((int)currTime.DayOfWeek); 
                        case 0x07:
                            UnityEngine.Debug.Log("返回当前时间的日期");
                            return Util.ToBCD(currTime.Day);
                        case 0x08:
                            UnityEngine.Debug.Log("返回当前时间的月份");
                            return Util.ToBCD(currTime.Month);
                        case 0x09:
                            UnityEngine.Debug.Log("返回当前时间的年份的后两位");
                            return Util.ToBCD(currTime.Year % 100);
                        case 0x0a:
                            UnityEngine.Debug.Log("返回状态A");
                            return statusA;
                        case 0x0b:
                            UnityEngine.Debug.Log("返回状态B");
                            return statusB;
                        case 0x0c:
                            UnityEngine.Debug.Log("返回状态C");
                            return statusC;
                        case 0x0d:
                            UnityEngine.Debug.Log("返回状态D");
                            return statusD;
                        case 0x0f:
                            return 0x00;
                        case 0x10:
                            //UnityEngine.Debug.Log("1.44M 软盘驱动器");
                            return 0x40;
                        case 0x12:
                            //UnityEngine.Debug.Log("检查硬盘数量(不超过两个)");
                            switch (ataDevice.HardDrives.Length)
                            {
                                case 1:
                                    UnityEngine.Debug.Log("如果只有一个硬盘，则返回0xf0");
                                    return 0xf0;
                                case 2:
                                    UnityEngine.Debug.Log("如果有两个硬盘，则返回0xff");
                                    return 0xff; 
                                default:
                                    UnityEngine.Debug.Log("虚拟硬盘数量不正确");
                                    return 0;
                            }
                        case 0x13:
                            UnityEngine.Debug.Log("键盘重复速率");
                            return 0;
                        case 0x14:
                            UnityEngine.Debug.Log("机器配置字节");
                            return 0x05;
                        case 0x15:
                            UnityEngine.Debug.Log("可用的640K内存的低字节");
                            return 0x71; 
                        case 0x16:
                            //UnityEngine.Debug.Log("上述的高字节");
                            return 0x02; 
                        case 0x17:
                            //UnityEngine.Debug.Log("1M -> 65M 内存的低字节");
                            if (UnityManager.ins.MemorySize > 64)
                            {
                                //UnityEngine.Debug.Log("运行内存大于64M");
                                return 0xff;
                            }
                            else
                            {
                                //UnityEngine.Debug.Log("运行内存小于64M");
                                return (byte)(((UnityManager.ins.MemorySize - 1) * 1024));
                            }
                        case 0x18:
                            //UnityEngine.Debug.Log("上述的高字节");
                            if (UnityManager.ins.MemorySize > 64)
                            {
                                //UnityEngine.Debug.Log("运行内存大于64M");
                                return 0xff;
                            }
                            else
                            {
                                //UnityEngine.Debug.Log("运行内存小于64M");
                                return (byte)(((UnityManager.ins.MemorySize - 1) * 1024) >> 8);
                            }
                        case 0x19:
                            if (ataDevice.HardDrives.Length == 0)
                            {
                                UnityEngine.Debug.Log("没有虚拟硬盘");
                                return 0;
                            }
                            return 47;
                        case 0x1a:
                            if (ataDevice.HardDrives.Length < 2)
                            {
                                UnityEngine.Debug.Log("虚拟硬盘数量小于2");
                                return 0;
                            }
                            return 47;
                        case 0x1b:
                            UnityEngine.Debug.Log("HDD1 - 柱面数低字节");
                            if (ataDevice.HardDrives.Length == 0)
                            {
                                UnityEngine.Debug.Log("没有虚拟硬盘");
                                return 0;
                            }
                            return (byte)ataDevice.HardDrives[0].Cylinders;
                        case 0x1c:
                            UnityEngine.Debug.Log("HDD1 - 柱面数高字节");
                            if (ataDevice.HardDrives.Length == 0)
                            {
                                UnityEngine.Debug.Log("没有虚拟硬盘");
                                return 0;
                            }
                            return (byte)(ataDevice.HardDrives[0].Cylinders >> 8);
                        case 0x1d:
                            UnityEngine.Debug.Log("HDD1 - 磁头数");
                            if (ataDevice.HardDrives.Length == 0)
                            {
                                UnityEngine.Debug.Log("没有虚拟硬盘");
                                return 0;
                            }
                            return ataDevice.HardDrives[0].Heads;
                        case 0x1e:
                        case 0x1f:
                            UnityEngine.Debug.Log("HDD 预补偿 - 未使用");
                            return 0;
                        case 0x20:
                            UnityEngine.Debug.Log("HDD1 驱动器控制字节");
                            return 0x08;
                        case 0x21:
                            UnityEngine.Debug.Log("HDD 着陆区 - 未使用");
                            return 0;
                        case 0x22:
                            return 0;
                        case 0x23:
                            UnityEngine.Debug.Log("HDD1 - 扇区数");
                            if (ataDevice.HardDrives.Length == 0)
                            {
                                UnityEngine.Debug.Log("没有虚拟硬盘");
                                return 0;
                            }
                            return ataDevice.HardDrives[0].Sectors;
                        case 0x30:
                            if (UnityManager.ins.MemorySize > 64)
                            {
                                //UnityEngine.Debug.Log("运行内存大于64M");
                                return 0xff;
                            }
                            else
                            {
                                //UnityEngine.Debug.Log("运行内存小于64M");
                                return (byte)(((UnityManager.ins.MemorySize - 1) * 1024));
                            }
                        case 0x31:
                            UnityEngine.Debug.Log("上述的高字节");
                            if (UnityManager.ins.MemorySize > 64)
                            {
                                //UnityEngine.Debug.Log("运行内存大于64M");
                                return 0xff;
                            }
                            else
                            {
                                //UnityEngine.Debug.Log("运行内存小于64M");
                                return (byte)(((UnityManager.ins.MemorySize - 1) * 1024) >> 8);
                            }
                        case 0x32:
                            UnityEngine.Debug.Log("返回当前时间的年份的前两位");
                            return Util.ToBCD(currTime.Year / 100); 
                        case 0x34:
                            UnityEngine.Debug.Log("16MB 到 4GB 内存的低字节");
                            return (byte)(((UnityManager.ins.MemorySize - 16) * 1024 * 1024) >> 16);
                        case 0x35:
                            //UnityEngine.Debug.Log("上述的高字节");
                            return (byte)((((UnityManager.ins.MemorySize - 16) * 1024 * 1024) >> 16) >> 8);
                        case 0x3d:
                            UnityEngine.Debug.Log("第一和第二启动设备");
                            return 0x21;
                        case 0x38:
                            UnityEngine.Debug.Log("第三启动设备");
                            return 0x00;
                        case 0x39:
                            UnityEngine.Debug.Log("HDD0 翻译模式，我们选择 LBA");
                            return 0x1;
                        case 0x5b:
                            return 0x00;
                        case 0x5c:
                            return 0x00;
                        case 0x5d:
                            return 0x00;
                        default:
                            //System.Diagnostics.Debugger.Break();
                            break;
                    }
                    break;
            }
            return ret;
        }

        public void Write(ushort addr, uint value, int size)
        {
            var tmp = (ushort)(value & 0x7f);

            switch (addr)
            {
                case 0x70:         
                    currentReg = (byte)tmp;
                    break;
                case 0x71:
                    switch (currentReg)
                    {
                        case 0x0a:
                            statusA = (byte)value;
                            break;
                        case 0x0b:
                            statusB = (byte)value;
                            break;
                        case 0x0f:
                            break;
                        default:
                            //System.Diagnostics.Debugger.Break();
                            break;
                    }
                    break;
            }
        }
    }
}
