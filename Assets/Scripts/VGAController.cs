using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using x86CS;

/// <summary>
/// 画面测试用
/// </summary>
public class VGAController : MonoBehaviour
{
    public int width = 960, height = 544;
    Color[] clearPixels;
    byte[] fontBuffer = new byte[0x2000];
    byte[] displayBuffer = new byte[0xfa0];
    Color foreColour;
    Color backColour;
    private bool FirstFrame = true;
    private UInt32[] wrapTexBuffer;
    private IntPtr wrapTexBufferPointer;
    private Texture2D wrapTex;
    private int TexBufferSize;
    public RawImage NextRawImage;

    private void Start()
    {
        InitializeTextures();
    }

    private void InitializeTextures()
    {
        wrapTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        wrapTexBuffer = new UInt32[width * height];
        GCHandle handle = GCHandle.Alloc(wrapTexBuffer, GCHandleType.Pinned);
        wrapTexBufferPointer = handle.AddrOfPinnedObject();
        TexBufferSize = wrapTexBuffer.Length * 4;

        clearPixels = new Color[width * height];
        for (int i = 0; i < clearPixels.Length; i++)
        {
            clearPixels[i] = new Color(0f, 0f, 0f, 0f);
        }
    }

    public void Clear()
    {
        Array.Clear(wrapTexBuffer, 0, wrapTexBuffer.Length);
        wrapTex.LoadRawTextureData(wrapTexBufferPointer, TexBufferSize);
        wrapTex.Apply();
    }

    /// <summary>
    /// 载入VGA画面-方法1
    /// </summary>
    public void LoadVGA()
    {
        if (!UnityMain.ins.CPU_Run) return;

        if (FirstFrame)
        {
            FirstFrame = false;
            Clear();
        }

        Memory.BlockRead(0xa0000, fontBuffer, fontBuffer.Length);
        Memory.BlockRead(0xb8000, displayBuffer, displayBuffer.Length);

        for (var i = 0; i < displayBuffer.Length; i += 2)
        {
            int currChar = displayBuffer[i];
            int fontOffset = currChar * 32;
            byte attribute = displayBuffer[i + 1];
            int y = (i / 160) * 16;

            foreColour = UnityMain.ins.machine.vgaDevice.GetColour(attribute & 0xf);
            backColour = UnityMain.ins.machine.vgaDevice.GetColour((attribute >> 4) & 0xf);

            uint foreColorUint = (uint)(foreColour.a * 255) << 24 | (uint)(foreColour.b * 255) << 16 | (uint)(foreColour.g * 255) << 8 | (uint)(foreColour.r * 255);
            uint backColorUint = (uint)(backColour.a * 255) << 24 | (uint)(backColour.b * 255) << 16 | (uint)(backColour.g * 255) << 8 | (uint)(backColour.r * 255);

            for (var f = fontOffset; f < fontOffset + 16; f++)
            {
                int x = ((i % 160) / 2) * 8;

                for (var j = 7; j >= 0; j--)
                {
                    wrapTexBuffer[y * width + x] = ((fontBuffer[f] >> j) & 0x1) != 0 ? foreColorUint : backColorUint;
                    x++;
                }
                y++;
            }
        }

        wrapTex.LoadRawTextureData(wrapTexBufferPointer, TexBufferSize);
        wrapTex.Apply();

        NextRawImage.gameObject.SetActive(true);

        NextRawImage.texture = wrapTex;
    }
}