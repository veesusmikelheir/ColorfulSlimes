using ColorfulSlimes.Components;
using SRML.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            #region COLORS
            string topHex = "#" + ColorUtility.ToHtmlStringRGB(painter.currTop);
            string midHex = "#" + ColorUtility.ToHtmlStringRGB(painter.currMid);
            string botHex = "#" + ColorUtility.ToHtmlStringRGB(painter.currBot);

            int[] topRGB = new int[3]
            {
                Mathf.RoundToInt(painter.currTop.r * 255f),
                Mathf.RoundToInt(painter.currTop.g * 255f),
                Mathf.RoundToInt(painter.currTop.b * 255f),
            };

            int[] midRGB = new int[3]
            {
                Mathf.RoundToInt(painter.currMid.r * 255f),
                Mathf.RoundToInt(painter.currMid.g * 255f),
                Mathf.RoundToInt(painter.currMid.b * 255f),
            };

            int[] botRGB = new int[3]
            {
                Mathf.RoundToInt(painter.currBot.r * 255f),
                Mathf.RoundToInt(painter.currBot.g * 255f),
                Mathf.RoundToInt(painter.currBot.b * 255f),
            };

            string[] topFloat = new string[3]
            {
                painter.currTop.r.ToString($"F{4}"),
                painter.currTop.g.ToString($"F{4}"),
                painter.currTop.b.ToString($"F{4}"),
            };

            string[] midFloat = new string[3]
            {
                painter.currMid.r.ToString($"F{4}"),
                painter.currMid.g.ToString($"F{4}"),
                painter.currMid.b.ToString($"F{4}"),
            };

            string[] botFloat = new string[3]
            {
                painter.currBot.r.ToString($"F{4}"),
                painter.currBot.g.ToString($"F{4}"),
                painter.currBot.b.ToString($"F{4}"),
            };
            #endregion

            string all =
                $"""
                -- {hit.collider.name.Replace("(Clone)", "")} --
                --- Top
                ---- HEX : {topHex}
                ---- RGB : {topRGB[0]}, {topRGB[1]}, {topRGB[2]}
                ---- FLOAT : {topFloat[0]}, {topFloat[1]}, {topFloat[2]}
                --- Middle
                ---- HEX : {midHex}
                ---- RGB : {midRGB[0]}, {midRGB[1]}, {midRGB[2]}
                ---- FLOAT : {midFloat[0]}f, {midFloat[1]}f, {midFloat[2]}f
                --- Bottom
                ---- HEX : {botHex}
                ---- RGB : {botRGB[0]}, {botRGB[1]}, {botRGB[2]}
                ---- FLOAT : {botFloat[0]}f, {botFloat[1]}f, {botFloat[2]}f
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
