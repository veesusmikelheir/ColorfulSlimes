using ColorfulSlimes.Components;
using SRML.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ColorfulSlimes.Main;

namespace ColorfulSlimes.Commands
{
    internal class GetColors : ConsoleCommand
    {
        public override string ID => "getcolors";

        public override string Usage => "getcolors <export>";

        public override string Description => "Gets all the colors (hex/rgb/float) of the paintable object you're currently looking at.";

        public override bool Execute(string[] args)
        {
            if (args.Length > 2)
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

            string[] hexColors = ColorToHexArray(painter.currTop, painter.currMid, painter.currBot);
            byte[][] rgbColors = ColorToRGBArray(painter.currTop, painter.currMid, painter.currBot);
            string[][] floatColors = ColorToFloatArray(painter.currTop, painter.currMid, painter.currBot);

            string all =
                $"""
                -- {hit.collider.name.Replace("(Clone)", "")} --

                --- Top
                ---- HEX : {hexColors[0]}
                ---- RGB : {rgbColors[0][0]}, {rgbColors[0][1]}, {rgbColors[0][2]}
                ---- FLOAT : {floatColors[0][0]}f, {floatColors[0][1]}f, {floatColors[0][2]}f

                --- Middle
                ---- HEX : {hexColors[1]}
                ---- RGB : {rgbColors[1][0]}, {rgbColors[1][1]}, {rgbColors[1][2]}
                ---- FLOAT : {floatColors[1][0]}f, {floatColors[1][1]}f, {floatColors[1][2]}f

                --- Bottom
                ---- HEX : {hexColors[2]}
                ---- RGB : {rgbColors[2][0]}, {rgbColors[2][1]}, {rgbColors[2][2]}
                ---- FLOAT : {floatColors[2][0]}f, {floatColors[2][1]}f, {floatColors[2][2]}f

                Remove the `f` from floats if you're not using it for, presumably, developing reasons.
                """;

            // Logs the colors
            ModConsole.Log("\n" + all);

            // Exports the colors
            if (!bool.TryParse(args[0], out bool result))
            {
                ModConsole.LogError($"Export value (true/false) is invalid!");
                return false;
            }

            if (result)
            {
                if (!Directory.Exists("ColorfulSlimes\\Export"))
                    Directory.CreateDirectory("ColorfulSlimes\\Export");

                DirectoryInfo directoryInfo = new DirectoryInfo($"{Environment.CurrentDirectory}\\ColorfulSlimes\\Export");
                FileInfo[] fileInfo = directoryInfo.GetFiles("*.txt");

                int count = 0;
                if (fileInfo.Length > 0)
                    for (int i = 0; i < fileInfo.Length; i++) // for or foreach, whatever, im liking for loops now lol
                        count++;

                File.WriteAllText($"{Environment.CurrentDirectory}\\ColorfulSlimes\\Export\\colorful_slimes_export_{count}.txt", all);
                ModConsole.Log($"{Environment.CurrentDirectory}\\ColorfulSlimes\\Export\\colorful_slimes_export_{count}.txt is the exported file path.");
            }

            return true;
        }

        public string[] ColorToHexArray(params Color[] colors)
        {
            List<string> hexes = new();
            for (int i = 0; i < colors.Length; i++)
                hexes.Add(ColorUtility.ToHtmlStringRGB(colors[i]));
            return hexes.ToArray();
        }

        public string[][] ColorToFloatArray(params Color[] colors)
        {
            List<string[]> floats = new();
            for (int i = 0; i < colors.Length; i++)
            {
                floats.Add(new string[]
                {
                    colors[i].r.ToString($"F{4}"),
                    colors[i].g.ToString($"F{4}"),
                    colors[i].b.ToString($"F{4}")
                });
            }
            return floats.ToArray();
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

        /*void PrintSection(Sections section, Painter painter)
        {
            switch (section)
            {
                case Sections.TOP:
                    {
                        string topHex = ColorUtility.ToHtmlStringRGB(painter.currTop);
                        ModConsole.Log("\n" +
                            $"-- {painter.name.Replace("(Clone)", "")} -- \n" +
                            $"--- Top : {topHex}\n"
                        );
                        break;
                    }
                case Sections.MIDDLE:
                    {
                        string midHex = ColorUtility.ToHtmlStringRGB(painter.currMid);
                        ModConsole.Log("\n" +
                            $"-- {painter.name.Replace("(Clone)", "")} -- \n" +
                            $"--- Middle : {midHex}\n"
                        );
                        break;
                    }
                case Sections.BOTTOM:
                    {
                        string botHex = ColorUtility.ToHtmlStringRGB(painter.currBot);
                        ModConsole.Log("\n" +
                            $"-- {painter.name.Replace("(Clone)", "")} -- \n" +
                            $"--- Bottom : {botHex}\n"
                        );
                        break;
                    }
            }
        }*/
    }
}
