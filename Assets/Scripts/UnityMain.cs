using x86CS;
using x86CS.Devices;
using x86CS.Configuration;
using UnityEngine;

public class UnityMain : MonoBehaviour
{
    public Machine machine;
    public void Load()
    {
        machine = new Machine();
        UnityManager.ins.MemorySize = 256;
        machine.Start();
    }
}