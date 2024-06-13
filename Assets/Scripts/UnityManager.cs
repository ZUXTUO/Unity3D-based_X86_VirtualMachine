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
    [Header("����λ��(������)")]
    public string ImagePath;
    [Header("BIOS��")]
    public string BiosName;
    [Header("VGABIOS��")]
    public string VgaBiosName;
    [Header("�˴��С")]
    public int MemorySize = 256;
    [Header("������")]
    public x86CS.Configuration.DriveType Type;
    [Header("����Ӳ�����ƣ�ѡ�")]
    public string VHD_Name;
    [Header("����Ӳ�̵�ַ(������)")]
    public string VHD_Path;
    [Header("����������ƣ�ѡ�")]
    public string ISO_Name;
    [Header("������̵�ַ(������)")]
    public string ISO_Path;

    /// <summary>
    /// ��������Ӳ����Ϣ
    /// </summary>
    public void DiskLoad()
    {
#if UNITY_EDITOR
        ImagePath = Directory.GetCurrentDirectory() + "/IMG/" + ImageName;
        Img_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + ImageName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        Bios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + BiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        VgaBios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + VgaBiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        if (Type == x86CS.Configuration.DriveType.HardDisk)
        {
            if (VHD_Name != null)
            {
                VHD_Path = Directory.GetCurrentDirectory() + "/IMG/" + VHD_Name;
            }
        }
        else if (Type == x86CS.Configuration.DriveType.CDROM)
        {
            if (ISO_Name != null)
            {
                ISO_Path = Directory.GetCurrentDirectory() + "/IMG/" + ISO_Name;
            }
        }
#elif UNITY_PSP2
        ImagePath = "/ux0:/IMG/" + ImageName;
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

    public FileStream Img_stream;//�����ļ�-��
    public FileStream Bios_stream;//BIOS�ļ�-��
    public FileStream VgaBios_stream;//VGABIOS�ļ�-��
}