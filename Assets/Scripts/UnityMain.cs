using x86CS;
using x86CS.Devices;
using x86CS.Configuration;
using UnityEngine;
using System.Threading;
using System.Collections;

public class UnityMain : MonoBehaviour
{
    public static UnityMain ins;

    public Machine machine;
    [Header("检测CPU是否在运行")]
    public bool CPU_Run = false;
    [Header("模拟的CPU频率")]
    public int HZ = 500;

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
        StartCoroutine(RunCpuCycle());
    }

    private IEnumerator RunCpuCycle()
    {
        while (CPU_Run)
        {
            for(int a = 0; a < HZ; a++)
            {
                machine.RunCycle();
            }
            yield return null;
        }
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void OnApplicationQuit()
    {
        CPU_Run = false;
        StopCoroutine(RunCpuCycle());
    }

    /// <summary>
    /// 内存输出
    /// </summary>
    public void Out()
    {
        Memory.Load();
    }
}