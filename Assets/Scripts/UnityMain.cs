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
    [Header("���CPU�Ƿ�������")]
    public bool CPU_Run = false;
    [Header("ģ���CPUƵ��")]
    public int HZ = 500;

    public void Awake()
    {
        ins = this;
    }

    /// <summary>
    /// ����
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
    /// �ر�
    /// </summary>
    public void OnApplicationQuit()
    {
        CPU_Run = false;
        StopCoroutine(RunCpuCycle());
    }

    /// <summary>
    /// �ڴ����
    /// </summary>
    public void Out()
    {
        Memory.Load();
    }
}