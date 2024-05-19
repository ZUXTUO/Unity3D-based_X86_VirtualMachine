using x86CS;
using x86CS.Devices;
using x86CS.Configuration;
using UnityEngine;
using System.Threading;

public class UnityMain : MonoBehaviour
{
    public Machine machine;
    public bool CPU_Timer = false;

    /// <summary>
    /// 开机
    /// </summary>
    public void Load()
    {
        machine = new Machine();
        UnityManager.ins.MemorySize = 256;
        machine.Running = true;
        machine.Start();
        CPU_Timer = true;
    }

    /// <summary>
    /// 内存输出
    /// </summary>
    public void Out()
    {
        Memory.Load();
    }

    /// <summary>
    /// CPU时钟（伪装）
    /// </summary>
    public void Update()
    {
        if (CPU_Timer)
        {
            machine.CPU.Fetch();
            machine.RunCycle();
        }
    }
}