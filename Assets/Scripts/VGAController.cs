using UnityEngine;
using UnityEngine.UI;
using x86CS;
using x86CS.Devices;
public class VGAController : MonoBehaviour
{
    protected VGA vgaDevice;
    public int Height, Width;
    private Texture2D vgaTexture;
    public RawImage image;
    //private UnityEngine.Color ucolor;

    public int memory_1, memory_2;

    private void Start()
    {
        vgaTexture = new Texture2D(Width, Height);
        vgaDevice = new VGA();
    }

    public void Update()
    {
        var fontBuffer = new byte[0x2000];
        var displayBuffer = new byte[0xfa0];
        Color[] data = new Color[Width * Height];

        memory_1 = Memory.BlockRead(0xa0000, fontBuffer, fontBuffer.Length);
        memory_2 = Memory.BlockRead(0xb8000, displayBuffer, displayBuffer.Length);

        for (var i = 0; i < displayBuffer.Length; i += 2)
        {
            int currChar = displayBuffer[i];
            int fontOffset = currChar * 32;
            byte attribute = displayBuffer[i + 1];
            int y = i / 160 * 16;

            System.Drawing.Color fore, back;

            fore = vgaDevice.GetColour(attribute & 0xf);
            back = vgaDevice.GetColour((attribute >> 4) & 0xf);
            Color foreColour = new Color(fore.R, fore.G, fore.B);
            Color backColour = new Color(back.R, back.G, back.B);

            for (var f = fontOffset; f < fontOffset + 16; f++)
            {
                int x = ((i % 160) / 2) * 8;

                for (var j = 7; j >= 0; j--)
                {
                    if (((fontBuffer[f] >> j) & 0x1) != 0)
                        data[y * Width + x] = foreColour;
                    else
                        data[y * Width + x] = backColour;
                    x++;
                }
                y++;
            }
        }

        vgaTexture.SetPixels(data);
        vgaTexture.Apply();
        image.texture = vgaTexture;

        /*
        var fontBuffer = new byte[0x2000];
        var displayBuffer = new byte[0xfa0];
        Color[] data = new Color[Width * Height];

        Memory.BlockRead(0xa0000, fontBuffer, fontBuffer.Length);
        Memory.BlockRead(0xb8000, displayBuffer, displayBuffer.Length);

        for (var i = 0; i < displayBuffer.Length; i += 2)
        {
            int currChar = displayBuffer[i];
            int fontOffset = currChar * 32;
            byte attribute = displayBuffer[i + 1];
            int y = i / 160 * 16;

            System.Drawing.Color foreColour = vgaDevice.GetColour(attribute & 0xf);
            System.Drawing.Color backColour = vgaDevice.GetColour((attribute >> 4) & 0xf);

            for (var f = fontOffset; f < fontOffset + 16; f++)
            {
                int x = ((i % 160) / 2) * 8;

                for (var j = 7; j >= 0; j--)
                {
                    if (((fontBuffer[f] >> j) & 0x1) != 0)
                    {
                        ucolor = new UnityEngine.Color(foreColour.R, foreColour.G, foreColour.B, 1);
                        vgaTexture.SetPixel(x, y, ucolor);
                    }
                    else
                    {
                        ucolor = new UnityEngine.Color(backColour.R, backColour.G, backColour.B, 1);
                        vgaTexture.SetPixel(x, y, ucolor);
                    }
                    x++;
                }
                y++;
            }
        }
        vgaTexture.Apply();
        image.texture = vgaTexture;
        */
    }
}