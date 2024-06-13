using UnityEngine;
using UnityEngine.UI;
using x86CS;
using x86CS.Devices;
public class VGAController : MonoBehaviour
{
    private Texture2D vgaTexture_1, vgaTexture_2;
    public RawImage image_1, image_2;

    public int memory_1, memory_2;

    public Event e;

    private void Start()
    {
        vgaTexture_1 = new Texture2D(960, 544);
        vgaTexture_2 = new Texture2D(960, 544);
    }

    public static Color Convert(System.Drawing.Color drawingColor)
    {
        return new Color(
            drawingColor.R / 255f,
            drawingColor.G / 255f,
            drawingColor.B / 255f,
            drawingColor.A / 255f
        );
    }

    public void Update()
    {
        if (UnityMain.ins.CPU_Run)
        {
            byte[] fontBuffer = new byte[0x2000];
            byte[] displayBuffer = new byte[0xfa0];

            // 从内存中读取数据
            Memory.BlockRead(0xa0000, fontBuffer, fontBuffer.Length);
            Memory.BlockRead(0xb8000, displayBuffer, displayBuffer.Length);

            Color[] pixelColors_1 = new Color[960 * 544];
            Color[] pixelColors_2 = new Color[960 * 544];

            int pixelIndex = 0;

            for (var i = 0; i < displayBuffer.Length; i += 2)
            {
                int currChar = displayBuffer[i];
                int fontOffset = currChar * 32;
                byte attribute = displayBuffer[i + 1];
                int y = i / 160 * 16;

                System.Drawing.Color foreColour = UnityMain.ins.machine.vgaDevice.GetColour(attribute & 0xf);
                System.Drawing.Color backColour = UnityMain.ins.machine.vgaDevice.GetColour((attribute >> 4) & 0xf);

                for (var f = fontOffset; f < fontOffset + 16; f++)
                {
                    int x = ((i % 160) / 2) * 8;

                    for (var j = 7; j >= 0; j--)
                    {
                        if (((fontBuffer[f] >> j) & 0x1) != 0)
                            vgaTexture_1.SetPixel(x, y, Convert(foreColour));
                        else
                            vgaTexture_2.SetPixel(x, y, Convert(backColour));
                        x++;
                    }
                    y++;
                }
            }

            vgaTexture_1.Apply();
            vgaTexture_2.Apply();
            image_1.texture = vgaTexture_1;
            image_2.texture = vgaTexture_2;

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                UnityMain.ins.machine.keyboard.KeyPress((uint)'0');
                Debug.Log("down 0");
            }
            else if (Input.GetKeyUp(KeyCode.Keypad0))
            {
                UnityMain.ins.machine.keyboard.KeyUp((uint)'0');
                Debug.Log("up 0");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                UnityMain.ins.machine.keyboard.KeyPress((uint)'1');
                Debug.Log("down 1");
            }
            else if (Input.GetKeyUp(KeyCode.Keypad1))
            {
                UnityMain.ins.machine.keyboard.KeyUp((uint)'1');
                Debug.Log("up 1");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                UnityMain.ins.machine.keyboard.KeyPress((uint)'2');
                Debug.Log("down 2");
            }
            else if (Input.GetKeyUp(KeyCode.Keypad2))
            {
                UnityMain.ins.machine.keyboard.KeyUp((uint)'2');
                Debug.Log("up 2");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                UnityMain.ins.machine.keyboard.KeyPress((uint)'3');
                Debug.Log("down 3");
            }
            else if (Input.GetKeyUp(KeyCode.Keypad3))
            {
                UnityMain.ins.machine.keyboard.KeyUp((uint)'3');
                Debug.Log("up 3");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                UnityMain.ins.machine.keyboard.KeyPress((uint)'4');
                Debug.Log("down 4");
            }
            else if (Input.GetKeyUp(KeyCode.Keypad4))
            {
                UnityMain.ins.machine.keyboard.KeyUp((uint)'4');
                Debug.Log("up 4");
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                UnityMain.ins.machine.keyboard.KeyPress((uint)'5');
                Debug.Log("down 5");
            }
            else if (Input.GetKeyUp(KeyCode.Keypad5))
            {
                UnityMain.ins.machine.keyboard.KeyUp((uint)'5');
                Debug.Log("up 5");
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                UnityMain.ins.machine.keyboard.KeyPress((uint)'\r');
                Debug.Log("down enter");
            }
            else if (Input.GetKeyUp(KeyCode.Return))
            {
                UnityMain.ins.machine.keyboard.KeyUp((uint)'\r');
                Debug.Log("up enter");
            }
        }
    }
}