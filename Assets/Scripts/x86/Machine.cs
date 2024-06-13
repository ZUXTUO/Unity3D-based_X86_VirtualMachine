using System;
using System.Collections.Generic;
using System.IO;
using x86CS.Devices;
using x86CS.Configuration;
using x86CS.ATADevice;

namespace x86CS
{
    public delegate void InteruptHandler();

    public class Machine
    {
        private readonly Dictionary<uint, uint> breakpoints = new Dictionary<uint, uint>();
        private readonly Dictionary<uint, uint> tempBreakpoints = new Dictionary<uint, uint>();
        private readonly IDevice[] devices;
        private readonly PIC8259 picDevice;
        public readonly VGA vgaDevice;
        private readonly DMAController dmaController;
        private readonly ATA ataDevice;

        private Dictionary<ushort, IOEntry> ioPorts;
        public KeyboardDevice keyboard;
        private bool isStepping;

        public Floppy FloppyDrive { get; private set; }
        public HardDisk HardDiskDrive { get; private set; }
        public CPU.CPU CPU { get; private set; }

        public bool Running;

        private bool AppStillIdle
        {
            get
            {
                Message msg;
                return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        public Machine()
        {
            picDevice = new PIC8259();
            vgaDevice = new VGA();
            FloppyDrive = new Floppy();
            dmaController = new DMAController();
            keyboard = new KeyboardDevice();
            ataDevice = new ATA();

            devices = new IDevice[]
                          {
                              FloppyDrive, new CMOS(ataDevice), new Misc(), new PIT8253(), picDevice, keyboard, dmaController, vgaDevice, ataDevice
                          };

            CPU = new CPU.CPU();

            picDevice.Interrupt += PicDeviceInterrupt;

            SetupSystem();

            CPU.IORead += CPUIORead;
            CPU.IOWrite += CPUIOWrite;
        }

        void ApplicationIdle(object sender, System.EventArgs e)
        {
            while (AppStillIdle)
            {
                RunCycle();
            }
        }

        void PicDeviceInterrupt(object sender, InterruptEventArgs e)
        {
            if (CPU.IF)
            {
                uint currentAddr = (uint)(CPU.GetSelectorBase(x86Disasm.SegmentRegister.CS) + CPU.EIP);
                picDevice.AckInterrupt(e.IRQ);
                CPU.ExecuteInterrupt(e.Vector);
                if (isStepping)
                {
                    tempBreakpoints.Add(currentAddr, currentAddr);
                    Running = true;
                }
            }
        }

        void DMARaised(object sender, ByteArrayEventArgs e)
        {
            var device = sender as INeedsDMA;

            if (device == null)
                return;

            dmaController.DoTransfer(device.DMAChannel, e.ByteArray);
        }

        void IRQRaised(object sender, EventArgs e)
        {
            var device = sender as INeedsIRQ;

            if (device == null)
                return;

            picDevice.RequestInterrupt((byte)device.IRQNumber);
        }

        private void SetupIOEntry(ushort port, ReadCallback read, WriteCallback write)
        {
            var entry = new IOEntry {Read = read, Write = write};

            ioPorts.Add(port, entry);
        }

        private uint CPUIORead(ushort addr, int size)
        {
            IOEntry entry;

            var ret = (ushort) (!ioPorts.TryGetValue(addr, out entry) ? 0xffff : entry.Read(addr, size));

            return ret;
        }

        private void CPUIOWrite(ushort addr, uint value, int size)
        {
            IOEntry entry;

            if (ioPorts.TryGetValue(addr, out entry))
                entry.Write(addr, value, size);
        }

        private void LoadBIOS()
        {
            var buffer = new byte[UnityManager.ins.Bios_stream.Length];

            uint startAddr = (uint)(0xfffff - buffer.Length) + 1;

            UnityManager.ins.Bios_stream.Read(buffer, 0, buffer.Length);
            Memory.BlockWrite(startAddr, buffer, buffer.Length);

            UnityManager.ins.Bios_stream.Close();
            UnityManager.ins.Bios_stream.Dispose();
        }

        private void LoadVGABios()
        {
            var buffer = new byte[UnityManager.ins.VgaBios_stream.Length];

            UnityManager.ins.VgaBios_stream.Read(buffer, 0, buffer.Length);
            Memory.BlockWrite(0xc0000, buffer, buffer.Length);

            UnityManager.ins.VgaBios_stream.Close();
            UnityManager.ins.VgaBios_stream.Dispose();
        }

        private void SetupSystem()
        {
            ioPorts = new Dictionary<ushort, IOEntry>();
            keyboard = new KeyboardDevice();

            LoadBIOS();
            LoadVGABios();

            foreach(IDevice device in devices)
            {
                INeedsIRQ irqDevice = device as INeedsIRQ;
                INeedsDMA dmaDevice = device as INeedsDMA;

                if(irqDevice != null)
                    irqDevice.IRQ += IRQRaised;

                if(dmaDevice != null)
                    dmaDevice.DMA += DMARaised;

                foreach(int port in device.PortsUsed)
                    SetupIOEntry((ushort)port, device.Read, device.Write);
            }

            CPU.CS = 0xf000;
            CPU.IP = 0xfff0;
        }

        public void Restart()
        {
            Running = false;
            CPU.Reset();
            SetupSystem();
        }

        public void SetBreakpoint(uint addr)
        {
            if (breakpoints.ContainsKey(addr))
                return;

            breakpoints.Add(addr, addr);
        }

        public void ClearBreakpoint(uint addr)
        {
            if (!breakpoints.ContainsKey(addr))
                return;

            breakpoints.Remove(addr);
        }

        public bool CheckBreakpoint()
        {
            uint cpuAddr = CPU.CurrentAddr;
            
            return breakpoints.ContainsKey(cpuAddr) || tempBreakpoints.ContainsKey(cpuAddr);
        }

        public void Start()
        {
            int addr = (int)((CPU.CS << 4) + CPU.IP);

            CPU.Fetch(true);
        }

        public void Stop()
        {
            foreach (IDevice device in devices)
            {
                IShutdown shutdown = device as IShutdown;

                if (shutdown != null)
                    shutdown.Shutdown();
            }
        }

        public void ClearTempBreakpoints()
        {
            tempBreakpoints.Clear();
        }

        public void StepOver()
        {
            uint currentAddr = (uint)(CPU.GetSelectorBase(x86Disasm.SegmentRegister.CS) + CPU.EIP + CPU.OpLen);

            tempBreakpoints.Add(currentAddr, currentAddr);
            Running = true;
        }

        public void RunCycle()
        {
            RunCycle(false, false);
        }

        public void RunCycle(bool logging, bool stepping)
        {
            isStepping = stepping;
            CPU.Cycle(logging);
            CPU.Fetch(logging);
            picDevice.RunController();
            //keyboard.Cycle();
        }
    }

    public struct IOEntry
    {
        public ReadCallback Read;
        public WriteCallback Write;
    }
}
