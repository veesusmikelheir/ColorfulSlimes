using ColorfulSlimes.Components;
using SRML.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ColorfulSlimes.Main;

namespace ColorfulSlimes.Commands
{
    internal class PasteColors : ConsoleCommand
    {
        public override string ID => "pastecolors";

        public override string Usage => "pastecolors <name>";

        public override string Description => "Pastes the selected colors from the clipboard onto the paintable object you're currently looking at.";

        public static List<string> _cachedFilenames = new();

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0)
            {
                if (Directory.Exists("ColorfulSlimes\\Clipboard"))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo($"{Environment.CurrentDirectory}\\ColorfulSlimes\\Clipboard");
                    FileInfo[] fileInfo = directoryInfo.GetFiles("*.txt");

                    if (fileInfo.Length > 0)
                        for (int i = 0; i < fileInfo.Length; i++)
                            if (!_cachedFilenames.Contains(fileInfo[i].Name.Replace(".txt", "")))
                                _cachedFilenames.Add(fileInfo[i].Name.Replace(".txt", ""));
                }
                return _cachedFilenames;
            }
            return base.GetAutoComplete(argIndex, argText);
        }

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

            DirectoryInfo directoryInfo = new DirectoryInfo($"{Environment.CurrentDirectory}\\ColorfulSlimes\\Clipboard");
            FileInfo[] fileInfo = directoryInfo.GetFiles("*.txt");

            if (Directory.Exists("ColorfulSlimes\\Clipboard"))
            {
                if (fileInfo.Length < 1)
                {
                    ModConsole.Log("There is nothing in the clipboard! Consider copying colors first.");
                    return false;
                }
            }
            else
            {
                ModConsole.Log("The clipboard directory does not exist! Consider copying colors first.");
                return false;
            }

            List<string> clipboard = new(_cachedFilenames);
            if (fileInfo.Length > 0)
                for (int i = 0; i < fileInfo.Length; i++)
                    if (!clipboard.Contains(fileInfo[i].Name.Replace(".txt", "")))
                        clipboard.Add(fileInfo[i].Name.Replace(".txt", ""));

            string selected = clipboard.FirstOrDefault(x => x == args[0]);
            if (selected != args[0])
            {
                ModConsole.LogError("Selected name from clipboard not found!");
                return false;
            }

            string[] splitSelected = File.ReadAllText($"{Environment.CurrentDirectory}\\ColorfulSlimes\\Clipboard\\{selected}.txt").Split("; ");
            byte[][] splitBytes = StringToRGBArray(splitSelected[0], splitSelected[1], splitSelected[2]);

            painter.Paint(
                new Color32(splitBytes[0][0], splitBytes[0][1], splitBytes[0][2], 255),
                new Color32(splitBytes[1][0], splitBytes[1][1], splitBytes[1][2], 255),
                new Color32(splitBytes[2][0], splitBytes[2][1], splitBytes[2][2], 255)
            );

            return true;
        }

        // thanks cs
        public static byte[] StringToByteArray(string toParse)
        {
            byte[] byteArray = Array.ConvertAll(toParse.Split(", "), byte.Parse);
            return byteArray;
        }

        public byte[][] StringToRGBArray(params string[] strings)
        {
            List<byte[]> rgbs = new();
            for (int i = 0; i < strings.Length; i++)
                rgbs.Add(Array.ConvertAll(strings[i].Split(", "), byte.Parse));
            return rgbs.ToArray();
        }
    }
}
