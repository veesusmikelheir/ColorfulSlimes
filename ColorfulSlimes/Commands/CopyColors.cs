using ColorfulSlimes.Components;
using SRML.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ColorfulSlimes.Main;

namespace ColorfulSlimes.Commands
{
    internal class CopyColors : ConsoleCommand
    {
        public override string ID => "copycolors";

        public override string Usage => "copycolors <name>";

        public override string Description => "Copies all the colors of the paintable object you're currently looking at.";

        public override bool Execute(string[] args)
        {
            if (args.Length > 1)
            {
                ModConsole.LogError("Incorrect number of arguments!");
                return false;
            }

            Painter painter;
            if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                painter = hit.collider.GetComponent<Painter>();
                if (!painter)
                {
                    ModConsole.LogError("Invalid object!");
                    return false;
                }
            }
            else
            {
                ModConsole.LogError("You aren't pointed at anything!");
                return false;
            }

            try
            {
                byte[][] rgbArray = ColorToRGBArray(painter.currTop, painter.currMid, painter.currBot);

                if (!Directory.Exists("ColorfulSlimes\\Clipboard"))
                    Directory.CreateDirectory("ColorfulSlimes\\Clipboard");

                File.AppendAllLines($"{Environment.CurrentDirectory}\\ColorfulSlimes\\Clipboard\\{args[0]}.txt",
                    new List<string>()
                    {
                        $"{rgbArray[0][0]}, {rgbArray[0][1]}, {rgbArray[0][2]};",
                        $"{rgbArray[1][0]}, {rgbArray[1][1]}, {rgbArray[1][2]};",
                        $"{rgbArray[2][0]}, {rgbArray[2][1]}, {rgbArray[2][2]}"
                    });
            }
            catch
            {
                ModConsole.Log("Something went wrong with the copying process..");
                return false;
            }

            return true;
        }

        public byte[][] ColorToRGBArray(params Color[] colors)
        {
            List<byte[]> rgbs = new();
            for (int i = 0; i < colors.Length; i++)
            {
                rgbs.Add(new byte[]
                {
                    (byte)Mathf.RoundToInt(colors[i].r * 255f),
                    (byte)Mathf.RoundToInt(colors[i].g * 255f),
                    (byte)Mathf.RoundToInt(colors[i].b * 255f)
                });
            }
            return rgbs.ToArray();
        }
    }
}
