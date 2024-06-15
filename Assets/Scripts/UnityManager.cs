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

    [Header("虚拟软盘名")]
    public string FloppyName;
    [Header("虚拟硬盘名称")]
    public string VHD_Name;
    [Header("虚拟光盘名称")]
    public string ISO_Name;
    [Header("BIOS名")]
    public string BiosName;
    [Header("VGABIOS名")]
    public string VgaBiosName;
    [Header("运存大小")]
    public int MemorySize = 256;
    [Header("镜像名")]
    public x86CS.Configuration.DriveType Type;

    [HideInInspector]
    public string FloppyPath;
    [HideInInspector]
    public string VHD_Path;
    [HideInInspector]
    public string ISO_Path;

    /// <summary>
    /// 载入虚拟硬盘信息
    /// </summary>
    public void DiskLoad()
    {
#if UNITY_EDITOR
        Bios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + BiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        VgaBios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + VgaBiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        if (Type == x86CS.Configuration.DriveType.Floppy)
        {
            if (FloppyName != null)
            {
                FloppyPath = Directory.GetCurrentDirectory() + "/IMG/" + FloppyName;
                Img_stream = new FileStream(FloppyPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                UnityEngine.Debug.Log("Floppy: "+ FloppyPath);
            }
        }
        else if (Type == x86CS.Configuration.DriveType.HardDisk)
        {
            if (VHD_Name != null)
            {
                UnityEngine.Debug.Log("虚拟硬盘初步载入完成");
                VHD_Path = Directory.GetCurrentDirectory() + "/IMG/" + VHD_Name;
                Img_stream = new FileStream(VHD_Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                UnityEngine.Debug.Log("HardDisk: " + VHD_Path);
            }
        }
        else if (Type == x86CS.Configuration.DriveType.CDROM)
        {
            if (ISO_Name != null)
            {
                ISO_Path = Directory.GetCurrentDirectory() + "/IMG/" + ISO_Name;
                Img_stream = new FileStream(ISO_Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                UnityEngine.Debug.Log("ISO: " + ISO_Path);
            }
        }
#elif UNITY_PSP2
        FloppyPath = "/ux0:/IMG/" + FloppyName;
        Img_stream = new FileStream("/ux0:/IMG/" + ImageName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        Bios_stream = new FileStream("/ux0:/IMG/" + BiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        VgaBios_stream = new FileStream("/ux0:/IMG/" + VgaBiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        if (Type == x86CS.Configuration.DriveType.HardDisk)
        {
            if (VHD_Name != null)
            {
                VHD_Path = "/ux0:/IMG/" + VHD_Name;
            }
        }
        else if (Type == x86CS.Configuration.DriveType.CDROM)
        {
            if (ISO_Name != null)
            {
                ISO_Path = "/ux0:/IMG/" + ISO_Name;
            }
        }
#endif
    }

    public FileStream Img_stream;//镜像文件-流
    public FileStream Bios_stream;//BIOS文件-流
    public FileStream VgaBios_stream;//VGABIOS文件-流
}