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

        DiskLoad();//��������Ӳ����Ϣ
    }

    [Header("������")]
    public string ImageName;
    [Header("BIOS��")]
    public string BiosName;
    [Header("VGABIOS��")]
    public string VgaBiosName;
    [Header("����Ӳ��")]
    public DiskElement[] Disks;
    [Header("�˴��С")]
    public int MemorySize = 256;
    [Header("��������")]
    public x86CS.Configuration.DriveType Type;
    [Header("������־���")]
    public bool LogOutput = true;
    [Header("CPU��־���")]
    public bool CPU_LogOutput = true;

    /// <summary>
    /// ��������Ӳ����Ϣ
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

    public FileStream Img_stream;//�����ļ�-��
    public FileStream Iso_stream;//�����ļ�-��
    public FileStream Bios_stream;//BIOS�ļ�-��
    public FileStream VgaBios_stream;//VGABIOS�ļ�-��
}