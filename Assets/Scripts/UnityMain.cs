using x86CS;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UnityMain : MonoBehaviour
{
    public static UnityMain ins;

    public Machine machine;
    [Header("检测CPU是否在运行")]
    public bool CPU_Run = false;
    //[Header("模拟的CPU频率")]
    //public int HZ = 500;
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
    /// 逐帧刷新
    /// </summary>
    private void Update()
    {
        if (CPU_Run)
        {
            // 检查其他按键的输入
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // 将按键的字符转换为扫描码，并发送给键盘设备
                        uint scanCode = GetScanCode((char)keyCode);

                        // 模拟键盘按下动作
                        machine.keyboard.KeyPress(scanCode);

                        Debug.Log("按下：" + scanCode);
                    }
                    else if (Input.GetKeyUp(keyCode))
                    {
                        // 将按键的字符转换为扫描码，并发送给键盘设备
                        uint scanCode = GetScanCode((char)keyCode);

                        // 模拟键盘松开动作
                        machine.keyboard.KeyUp(scanCode);

                        Debug.Log("抬起：" + scanCode);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 根据字符获取对应的扫描码
    /// </summary>
    /// <param name="keyChar"></param>
    /// <returns></returns>
    private uint GetScanCode(char keyChar)
    {
        Dictionary<char, uint> scanCodeMap = new Dictionary<char, uint>
    {
        {' ', 0x39}, {'!', 0x02}, {'"', 0x28}, {'#', 0x04}, {'$', 0x05}, {'%', 0x06}, {'&', 0x08}, {'\'', 0x28},
        {'(', 0x0A}, {')', 0x0B}, {'*', 0x09}, {'+', 0x0D}, {',', 0x33}, {'-', 0x0C}, {'.', 0x34}, {'/', 0x35},
        {'0', 0x0B}, {'1', 0x02}, {'2', 0x03}, {'3', 0x04}, {'4', 0x05}, {'5', 0x06}, {'6', 0x07}, {'7', 0x08},
        {'8', 0x09}, {'9', 0x0A}, {':', 0x27}, {';', 0x27}, {'<', 0x33}, {'=', 0x0D}, {'>', 0x34}, {'?', 0x35},
        {'@', 0x03}, {'A', 0x1E}, {'B', 0x30}, {'C', 0x2E}, {'D', 0x20}, {'E', 0x12}, {'F', 0x21}, {'G', 0x22},
        {'H', 0x23}, {'I', 0x17}, {'J', 0x24}, {'K', 0x25}, {'L', 0x26}, {'M', 0x32}, {'N', 0x31}, {'O', 0x18},
        {'P', 0x19}, {'Q', 0x10}, {'R', 0x13}, {'S', 0x1F}, {'T', 0x14}, {'U', 0x16}, {'V', 0x2F}, {'W', 0x11},
        {'X', 0x2D}, {'Y', 0x15}, {'Z', 0x2C}, {'[', 0x1A}, {'\\', 0x2B}, {']', 0x1B}, {'^', 0x07}, {'_', 0x0C},
        {'`', 0x29}, {'a', 0x1E}, {'b', 0x30}, {'c', 0x2E}, {'d', 0x20}, {'e', 0x12}, {'f', 0x21}, {'g', 0x22},
        {'h', 0x23}, {'i', 0x17}, {'j', 0x24}, {'k', 0x25}, {'l', 0x26}, {'m', 0x32}, {'n', 0x31}, {'o', 0x18},
        {'p', 0x19}, {'q', 0x10}, {'r', 0x13}, {'s', 0x1F}, {'t', 0x14}, {'u', 0x16}, {'v', 0x2F}, {'w', 0x11},
        {'x', 0x2D}, {'y', 0x15}, {'z', 0x2C}, {'{', 0x1A}, {'|', 0x2B}, {'}', 0x1B}, {'~', 0x29}
    };

        if (scanCodeMap.ContainsKey(keyChar))
        {
            return scanCodeMap[keyChar];
        }

        return 0;
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