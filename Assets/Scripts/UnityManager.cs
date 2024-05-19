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

    public string ImageName;//镜像名
    public string BiosName;//BIOS名
    public string VgaBiosName;//VGABIOS名
    public DiskElement[] Disks;//虚拟硬盘
    public int MemorySize = 256;//运存大小
    public x86CS.Configuration.DriveType Type;//驱动属性

    /// <summary>
    /// 载入虚拟硬盘信息
    /// </summary>
    public void DiskLoad()
    {
        DiskElement diskElement = new DiskElement();
        diskElement.Type = Type;
        diskElement.Image = Directory.GetCurrentDirectory() + "/IMG/" + ImageName;

        Disks = new DiskElement[1];
        Disks[0] = diskElement;

        if (Type == x86CS.Configuration.DriveType.HardDisk)
        {
            Img_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + ImageName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        else if (Type == x86CS.Configuration.DriveType.CDROM)
        {
            Iso_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + ImageName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }
        Bios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + BiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        VgaBios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + VgaBiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
    }

    public FileStream Img_stream;//镜像文件-流
    public FileStream Iso_stream;//镜像文件-流
    public FileStream Bios_stream;//BIOS文件-流
    public FileStream VgaBios_stream;//VGABIOS文件-流
}