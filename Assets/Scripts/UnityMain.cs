using x86CS;
using x86CS.Devices;
using x86CS.Configuration;
using UnityEngine;
using System.Threading;

public class UnityMain : MonoBehaviour
{
    public static UnityMain ins;

    public Machine machine;
    public Thread thread;
    public bool CPU_Run = false;

    public void Awake()
    {
        ins = this;
    }

    /// <summary>
    /// 开机
    /// </summary>
    public void Load()
    {
        machine = new Machine();
        UnityManager.ins.MemorySize = 256;
        machine.Running = true;
        machine.Start();
        CPU_Run = true;
        thread = new Thread(() =>
        {
            while (CPU_Run)
            {
                machine.RunCycle();
            }
        });
        thread.Start();
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void OnApplicationQuit()
    {
        CPU_Run = false;
        thread.Join();
    }

    /// <summary>
    /// 内存输出
    /// </summary>
    public void Out()
    {
        Memory.Load();
    }
}