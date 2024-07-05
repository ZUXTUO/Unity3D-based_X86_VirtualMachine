using x86CS;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UnityMain : MonoBehaviour
{
    public static UnityMain ins;

    public Machine machine;
    [Header("检测CPU是否在运行")]
    public bool CPU_Run = false;
    [Header("模拟的CPU频率")]
    public int HZ = 500;
    [Header("VGA显示器")]
    public VGAController VGA;
    [Header("是否需要实时显示屏幕信息")]
    public bool NeedLoadVGA = true;

    public void Awake()
    {
        ins = this;
    }

    /// <summary>
    /// 开机
    /// </summary>
    public void Load()
    {
        if (!CPU_Run)
        {
            machine = new Machine();
            UnityManager.ins.MemorySize = 256;
            machine.Running = true;
            machine.Start();
            CPU_Run = true;
            StartCoroutine(RunCpuCycle());
            StartCoroutine(RunVGA());
        }
        else
        {
            Debug.Log("关机");
            OnApplicationQuit();
        }
    }

    private System.Threading.Thread thread;
    private bool isThreadRunning = false;
    /// <summary>
    /// CPU运行+VGA输出
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunCpuCycle()
    {
        // 检查线程是否正在运行，避免重复启动
        if (isThreadRunning)
        {
            yield break;
        }

        isThreadRunning = true;

        // 创建新的线程来执行功能
        thread = new System.Threading.Thread(ThreadFunction);
        thread.Start();
    }
    /// <summary>
    /// 线程执行
    /// </summary>
    private void ThreadFunction()
    {
        while (CPU_Run)
        {
            machine.RunCycle();
        }
    }
    /// <summary>
    /// VGA信息输出
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunVGA()
    {
        while (CPU_Run)
        {
            if (NeedLoadVGA)
            {
                VGA.LoadVGA();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 关闭
    /// </summary>
    private void OnApplicationQuit()
    {
        CPU_Run = false;
        StopCoroutine(RunCpuCycle());
        StopCoroutine(RunVGA());

        machine.Running = false;
        machine.Stop();

        isThreadRunning = false;
        thread.Abort();
        thread = null;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}