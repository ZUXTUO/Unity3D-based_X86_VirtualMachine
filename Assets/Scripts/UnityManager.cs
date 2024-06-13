using x86CS;
using x86CS.Devices;
using x86CS.Configuration;
using UnityEngine;
using System.IO;

public class UnityManager : MonoBehaviour
{
    public static UnityManager ins;
    public void Awake()
    {
        ins = this;
        DontDestroyOnLoad(gameObject);

        DiskLoad();//载入虚拟硬盘信息
    }

    [Header("镜像名")]
    public string ImageName;
    [Header("镜像位置")]
    public string ImagePath;
    [Header("BIOS名")]
    public string BiosName;
    [Header("VGABIOS名")]
    public string VgaBiosName;
    [Header("运存大小")]
    public int MemorySize = 256;

    /// <summary>
    /// 载入虚拟硬盘信息
    /// </summary>
    public void DiskLoad()
    {
        ImagePath = Directory.GetCurrentDirectory() + "/IMG/" + ImageName;
        Img_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + ImageName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        Bios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + BiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        VgaBios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + VgaBiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
    }

    public FileStream Img_stream;//镜像文件-流
    public FileStream Bios_stream;//BIOS文件-流
    public FileStream VgaBios_stream;//VGABIOS文件-流
}