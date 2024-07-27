using UnityEngine;
using System.IO;

public class UnityManager : MonoBehaviour
{
    public static UnityManager ins;
    public void Start()
    {
        ins = this;

        DiskLoad();//载入虚拟硬盘信息
    }

    [Header("镜像名(软盘/硬盘/光盘)")]
    public string ImgName;
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
            if (ImgName != null)
            {
                FloppyPath = Directory.GetCurrentDirectory() + "/IMG/" + ImgName;
                Img_stream = new FileStream(FloppyPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                if (UnityMain.ins.NeedLog)
                {
                    UnityEngine.Debug.Log("Floppy: "+ FloppyPath);
                }
            }
        }
        else if (Type == x86CS.Configuration.DriveType.HardDisk)
        {
            if (ImgName != null)
            {
                if (UnityMain.ins.NeedLog)
                {
                    UnityEngine.Debug.Log("虚拟硬盘初步载入完成");
                }
                VHD_Path = Directory.GetCurrentDirectory() + "/IMG/" + ImgName;
                Img_stream = new FileStream(VHD_Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                if (UnityMain.ins.NeedLog)
                {
                    UnityEngine.Debug.Log("HardDisk: " + VHD_Path);
                }
            }
        }
        else if (Type == x86CS.Configuration.DriveType.CDROM)
        {
            if (ImgName != null)
            {
                ISO_Path = Directory.GetCurrentDirectory() + "/IMG/" + ImgName;
                Img_stream = new FileStream(ISO_Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                if (UnityMain.ins.NeedLog)
                {
                    UnityEngine.Debug.Log("ISO: " + ISO_Path);
                }
            }
        }
#elif UNITY_PSP2
        Bios_stream = new FileStream("/ux0:/IMG/" + BiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        VgaBios_stream = new FileStream("/ux0:/IMG/" + VgaBiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        if (Type == x86CS.Configuration.DriveType.Floppy)
        {
            if (ImgName != null)
            {
                FloppyPath = "/ux0:/IMG/" + ImgName;
                Img_stream = new FileStream(FloppyPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                if (UnityMain.ins.NeedLog)
                {
                    UnityEngine.Debug.Log("Floppy: " + FloppyPath);
                }
            }
        }
        else if (Type == x86CS.Configuration.DriveType.HardDisk)
        {
            if (ImgName != null)
            {
                if (UnityMain.ins.NeedLog)
                {
                    UnityEngine.Debug.Log("虚拟硬盘初步载入完成");
                }
                VHD_Path = "/ux0:/IMG/" + ImgName;
                Img_stream = new FileStream(VHD_Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                if (UnityMain.ins.NeedLog)
                {
                    UnityEngine.Debug.Log("HardDisk: " + VHD_Path);
                }
            }
        }
        else if (Type == x86CS.Configuration.DriveType.CDROM)
        {
            if (ImgName != null)
            {
                ISO_Path = "/ux0:/IMG/" + ImgName;
                Img_stream = new FileStream(ISO_Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                if (UnityMain.ins.NeedLog)
                {
                    UnityEngine.Debug.Log("ISO: " + ISO_Path);
                }
            }
        }
#endif
    }

    public FileStream Img_stream;//镜像文件-流
    public FileStream Bios_stream;//BIOS文件-流
    public FileStream VgaBios_stream;//VGABIOS文件-流
}