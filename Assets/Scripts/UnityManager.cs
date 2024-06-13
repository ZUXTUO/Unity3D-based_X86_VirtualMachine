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
    [Header("镜像位置(不用填)")]
    public string ImagePath;
    [Header("BIOS名")]
    public string BiosName;
    [Header("VGABIOS名")]
    public string VgaBiosName;
    [Header("运存大小")]
    public int MemorySize = 256;
    [Header("镜像名")]
    public x86CS.Configuration.DriveType Type;
    [Header("虚拟硬盘名称（选填）")]
    public string VHD_Name;
    [Header("虚拟硬盘地址(不用填)")]
    public string VHD_Path;
    [Header("虚拟光盘名称（选填）")]
    public string ISO_Name;
    [Header("虚拟光盘地址(不用填)")]
    public string ISO_Path;

    /// <summary>
    /// 载入虚拟硬盘信息
    /// </summary>
    public void DiskLoad()
    {
        ImagePath = Directory.GetCurrentDirectory() + "/IMG/" + ImageName;
        Img_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + ImageName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        Bios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + BiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        VgaBios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + VgaBiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        //if (Type == x86CS.Configuration.DriveType.HardDisk || Type == x86CS.Configuration.DriveType.ALL)
        if (Type == x86CS.Configuration.DriveType.HardDisk)
        {
            if (VHD_Name != null)
            {
                VHD_Path = Directory.GetCurrentDirectory() + "/IMG/" + VHD_Name;
            }
        }
        else if (Type == x86CS.Configuration.DriveType.CDROM)
        //if (Type == x86CS.Configuration.DriveType.CDROM || Type == x86CS.Configuration.DriveType.ALL)
        {
            if (ISO_Name != null)
            {
                ISO_Path = Directory.GetCurrentDirectory() + "/IMG/" + ISO_Name;
            }
        }
    }

    public FileStream Img_stream;//镜像文件-流
    public FileStream Bios_stream;//BIOS文件-流
    public FileStream VgaBios_stream;//VGABIOS文件-流
}