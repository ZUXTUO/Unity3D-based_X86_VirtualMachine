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
    [Header("����λ��")]
    public string ImagePath;
    [Header("BIOS��")]
    public string BiosName;
    [Header("VGABIOS��")]
    public string VgaBiosName;
    [Header("�˴��С")]
    public int MemorySize = 256;

    /// <summary>
    /// ��������Ӳ����Ϣ
    /// </summary>
    public void DiskLoad()
    {
        ImagePath = Directory.GetCurrentDirectory() + "/IMG/" + ImageName;
        Img_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + ImageName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        Bios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + BiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        VgaBios_stream = new FileStream(Directory.GetCurrentDirectory() + "/IMG/" + VgaBiosName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
    }

    public FileStream Img_stream;//�����ļ�-��
    public FileStream Bios_stream;//BIOS�ļ�-��
    public FileStream VgaBios_stream;//VGABIOS�ļ�-��
}