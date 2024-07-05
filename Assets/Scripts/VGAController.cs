using UnityEngine;
using UnityEngine.UI;
using x86CS;
/// <summary>
/// 画面测试用
/// </summary>
public class VGAController : MonoBehaviour
{
    public Texture2D vgaTexture_1, vgaTexture_2;
    public RawImage image_1, image_2;
    public bool vga_font, vga_back;

    public int width = 960, height = 544;

    Color[] clearPixels;

    byte[] fontBuffer = new byte[0x2000];
    byte[] displayBuffer = new byte[0xfa0];

    Color[] pixels1 = new Color[0];
    Color[] pixels2 = new Color[0];
    Color foreColour;
    Color backColour;

    //第一帧检测--测试用
    private bool FirstFrame = true;

    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        InitializeTextures();
    }
    /// <summary>
    /// 图像重置
    /// </summary>
    private void InitializeTextures()
    {
        vgaTexture_1 = new Texture2D(width, height);
        vgaTexture_2 = new Texture2D(width, height);

        clearPixels = new Color[width * height];
        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = new Color(0f, 0f, 0f, 0f);
        }
    }
    /// <summary>
    /// 屏幕颜色清除
    /// </summary>
    public void Clear()
    {
        try
        {
            if (vga_font)
            {
                vgaTexture_1.SetPixels(clearPixels);
            }
            if (vga_back)
            {
                vgaTexture_2.SetPixels(clearPixels);
            }
        }
        catch { }
    }
    /// <summary>
    /// 改变VGA更新
    /// </summary>
    public void ChangeUpdateLoop()
    {
        if (UnityMain.ins.NeedLoadVGA)
        {
            UnityMain.ins.NeedLoadVGA = false;
        }
        else
        {
            UnityMain.ins.NeedLoadVGA = true;
        }
    }
    /// <summary>
    /// 载入VGA画面
    /// </summary>
    public void LoadVGA()
    {
        if (UnityMain.ins.CPU_Run)
        {
            if (FirstFrame)
            {
                FirstFrame = false;
                Clear();
            }

            if (vga_font)
            {
                //前
                Memory.BlockRead(0xa0000, fontBuffer, fontBuffer.Length);
            }
            if (vga_back)
            {
                //后
                Memory.BlockRead(0xb8000, displayBuffer, displayBuffer.Length);
            }

            pixels1 = vgaTexture_1.GetPixels();
            pixels2 = vgaTexture_2.GetPixels();

            for (var i = 0; i < displayBuffer.Length; i += 2)
            {
                int currChar = displayBuffer[i];
                int fontOffset = currChar * 32;
                byte attribute = displayBuffer[i + 1];
                int y = (i / 160) * 16;

                foreColour = UnityMain.ins.machine.vgaDevice.GetColour(attribute & 0xf);
                backColour = UnityMain.ins.machine.vgaDevice.GetColour((attribute >> 4) & 0xf);

                for (var f = fontOffset; f < fontOffset + 16; f++)
                {
                    int x = ((i % 160) / 2) * 8;

                    for (var j = 7; j >= 0; j--)
                    {
                        if (((fontBuffer[f] >> j) & 0x1) != 0)
                        {
                            pixels1[y * width + x] = foreColour;
                            pixels2[y * width + x] = Color.clear;
                        }
                        else
                        {
                            pixels1[y * width + x] = Color.clear;
                            pixels2[y * width + x] = backColour;
                        }
                        x++;
                    }
                    y++;
                }
            }

            if (vga_font)
            {
                vgaTexture_1.SetPixels(pixels1);
                vgaTexture_1.Apply();
                image_1.texture = vgaTexture_1;
            }
            if (vga_back)
            {
                vgaTexture_2.SetPixels(pixels2);
                vgaTexture_2.Apply();
                image_2.texture = vgaTexture_2;
            }
        }
    }
    /// <summary>
    /// 前
    /// </summary>
    public void Font()
    {
        vga_font = true;
        vga_back = false;
        image_1.gameObject.SetActive(true);
        image_2.gameObject.SetActive(false);
    }
    /// <summary>
    /// 后
    /// </summary>
    public void Back()
    {
        vga_font = false;
        vga_back = true;
        image_1.gameObject.SetActive(false);
        image_2.gameObject.SetActive(true);
    }
    /// <summary>
    /// 全
    /// </summary>
    public void All()
    {
        vga_font = true;
        vga_back = true;
        image_1.gameObject.SetActive(true);
        image_2.gameObject.SetActive(true);
    }
}