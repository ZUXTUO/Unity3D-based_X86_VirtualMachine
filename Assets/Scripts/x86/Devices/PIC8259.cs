using System.Diagnostics;
using System.Threading;
using System;

namespace x86CS.Devices
{
    public class PIC8259 : IDevice
    {
        private readonly int[] portsUsed = {0x20, 0x21, 0xa0, 0xa1};
        private readonly PIController[] controllers;

        public event EventHandler<InterruptEventArgs> Interrupt;

        public PIC8259()
        {
            controllers = new PIController[2];
            controllers[0] = new PIController();
            controllers[1] = new PIController();
        }

        /*
        //测试功能，没有用
        public void OnInterrupt()
        {
            EventHandler<InterruptEventArgs> handler = Interrupt;
            if (handler != null)
            {
                byte irq = 0x00;
                byte source = 0x00;
                InterruptEventArgs eventArgs = new InterruptEventArgs(irq, source);
                handler(this, eventArgs);
            }
        }
        */

        public void OnInterrupt(InterruptEventArgs e)
        {
            EventHandler<InterruptEventArgs> handler = Interrupt;
            if (handler != null)
                handler(this, e);

            //Write(0x21, 1, 0);
            //Write(0x21, 0x20, 0);
            //Write(0x21, 0x38, 0);
            //Write(0x21, 0x00, 0);
        }

        public bool RunController()
        {
            int runningIRQ = LowestRunningInt();
            int pendingIRQ = LowestPending();
            //int pendingIRQ = 0;

            byte irq, vector;

            UnityEngine.Debug.LogWarning("runningIRQ: " + runningIRQ + "  pendingIRQ: " + pendingIRQ);

            if (pendingIRQ == -1)
                //controllers[0].Test_Change_requestRegister();
                //controllers[1].Test_Change_requestRegister();
                return false;

            if (runningIRQ < 0)
            {
                irq = (byte)pendingIRQ;
                if (irq < 8)
                    vector = (byte)(controllers[0].VectorBase + irq);
                else
                    vector = (byte)(controllers[1].VectorBase + irq);

                OnInterrupt(new InterruptEventArgs(irq, vector));
                return true;
            }
            return false;
        }

        public int[] PortsUsed
        {
            get { return portsUsed; }
        }

        public bool RequestInterrupt(byte irq)
        {
            PIController controller = irq < 8 ? controllers[0] : controllers[1];

            if (((controller.MaskRegister >> irq & 0x1)) != 0)
                return false;

            return controller.RequestInterrupt(irq);
        }

        private int LowestRunningInt()
        {
            int ret = controllers[0].LowestRunning();
            return ret == -1 ? controllers[1].LowestRunning() : ret;
        }

        private int LowestPending()
        {
            int ret = controllers[0].LowestPending();
            return ret == -1 ? controllers[1].LowestPending() : ret;
        }

        public bool InterruptService(out int irq, out int vector)
        {
            int runningIRQ = LowestRunningInt();
            int pendingIRQ = LowestPending();

            if (pendingIRQ == -1)
            {
                irq = 0;
                vector = 0;
                return false;
            }

            if (runningIRQ < 0)
            {
                irq = pendingIRQ;
                if (irq < 8)
                    vector = controllers[0].VectorBase + irq;
                else
                    vector = controllers[1].VectorBase + irq;

                return true;
            }

            irq = 0;
            vector = 0;
            return false;
        }

        public void AckInterrupt(byte irq)
        {
            if (irq < 8)
                controllers[0].AckInterrupt(irq);
            else
                controllers[1].AckInterrupt(irq);
        }

        public uint Read(ushort addr, int size)
        {
            PIController controller = null;

            switch (addr)
            {
                case 0x20:
                case 0x21:
                    controller = controllers[0];
                    break;
                case 0xa0:
                case 0xa1:
                    controller = controllers[1];
                    break;
            }
            UnityEngine.Debug.Assert(controller != null, "controller != null");

            if ((controller.CommandRegister & 0x3) == 0x3)
                return controller.InServiceRegister;

            return addr%10 == 0 ? controller.StatusRegister : controller.DataRegister;
        }

        public void Write(ushort addr, uint value, int size)
        {
            PIController controller = null;

            switch (addr)
            {
                case 0x20:
                case 0x21:
                    controller = controllers[0];
                    break;
                case 0xa0:
                case 0xa1:
                    controller = controllers[1];
                    break;
            }

            UnityEngine.Debug.Assert(controller != null, "controller != null");
            if (!controller.Init)
                controller.ProcessICW((byte) value);
            else if (addr%0x10 == 0)
            {
                if (value == 0x20)
                    controller.EOI();
                controller.CommandRegister = (byte) value;
            }
            else
                controller.MaskRegister = (byte) value;
        }
    }

    internal class PIController
    {
        private byte requestRegister; // 请求寄存器，用于记录中断请求的状态
        private byte currentICW; // 当前初始化命令字(ICW)的计数器
        private bool expectICW4; // 是否期望收到初始化命令字4(ICW4)
        private byte linkIRQ; // 链接的中断请求号

        public byte InServiceRegister { get; private set; } // 服务寄存器，用于记录正在处理的中断
        public byte VectorBase { get; private set; } // 向量基地址，用于确定中断向量的起始位置
        public byte MaskRegister { get; set; } // 屏蔽寄存器，用于控制中断的屏蔽和解除屏蔽
        public bool Init { get; set; } // 初始化标志，表示控制器是否已初始化
        public byte CommandRegister { get; set; } // 命令寄存器，用于发送控制命令
        public byte StatusRegister { get; private set; } // 状态寄存器，用于记录控制器的状态
        public byte DataRegister { get; private set; } // 数据寄存器，用于读取或写入数据

        public PIController()
        {
            MaskRegister = 0xff; // 初始化屏蔽寄存器为全1，表示未屏蔽任何中断
            currentICW = 0; // 初始化当前初始化命令字计数器为0
            Init = false; // 初始化标志为false，表示控制器未初始化
            StatusRegister = 0; // 初始化状态寄存器为0
            DataRegister = 0; // 初始化数据寄存器为0
            InServiceRegister = 0; // 初始化服务寄存器为0
        }

        public bool RequestInterrupt(byte irq)
        {
            // 检查中断是否已在服务中
            if (((InServiceRegister >> irq) & 0x1) != 0)
                return false;

            // 检查中断是否已被请求
            if (((requestRegister >> irq) & 0x1) != 0)
                return false;

            // 将中断请求置位
            requestRegister |= (byte)(1 << irq);

            return true;
        }

        public int LowestRunning()
        {
            // 查找最低优先级的正在服务的中断
            for (int i = 0; i < 8; i++)
            {
                if (((InServiceRegister >> i) & 0x1) == 0x1)
                    return i;
            }
            return -1;
        }

        public int LowestPending()
        {
            // 查找最低优先级的未处理的中断
            for (int i = 0; i < 8; i++)
            {
                if (((requestRegister >> i) & 0x1) == 0x1)
                    return i;
            }
            return -1;
        }

        public void AckInterrupt(byte irq)
        {
            // 清除中断请求位并将中断加入服务中
            requestRegister &= (byte)~(1 << irq);
            InServiceRegister |= (byte)(1 << irq);
        }

        public void EOI()
        {
            // 结束中断服务，将服务寄存器清零
            InServiceRegister = 0;
        }

        public void ProcessICW(byte icw)
        {
            switch (currentICW)
            {
                case 0:
                    expectICW4 = (icw & 0x1) != 0; // 检查是否期望收到ICW4
                    break;
                case 1:
                    VectorBase = icw; // 设置向量基地址
                    break;
                case 2:
                    linkIRQ = (byte)(icw & 0x7); // 设置链接的中断请求号
                    if (!expectICW4)
                        Init = true; // 如果不期望收到ICW4，则初始化完成
                    break;
                case 3:
                    Init = true; // 初始化完成
                    break;
            }
            if (Init)
            {
                currentICW = 0;
            }
            else
            {
                currentICW++;
            }
        }

        //测试功能
        public void Test_Change_requestRegister()
        {
            requestRegister = 1;
        }
    }
}
