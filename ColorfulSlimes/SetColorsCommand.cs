using SRML.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ColorfulSlimes.Main;

namespace ColorfulSlimes
{
    internal class SetColorsCommand : ConsoleCommand
    {
        public override string ID => "setcolors";

        public override string Usage => "setcolors \\<all/top/bottom/middle\\> \\<red\\> \\<green\\> \\<blue\\>";

        public override string Description => "sets the colors for a particular section on the slime you're currently looking at";

        public override bool Execute(string[] args)
        {
            if(args.Length != 4)
            {
                consoleInstance.LogError("Incorrect number of arguments!");
                return false;
            }

            Sections id;
            try
            {
                id = (Sections)Enum.Parse(typeof(Sections), args[0], true);
            }
            catch
            {
                consoleInstance.LogError("Invalid section");
                return false;
            }


            bool CheckColor(out int colorLocal, string colorstring, string colorName)
            {
                if(!int.TryParse(colorstring,out colorLocal))
                {
                    consoleInstance.LogError($"Color value {colorName} is invalid!");
                    return false;
                }

                if(colorLocal<0||colorLocal>255)
                {
                    consoleInstance.LogError("Color Values must be in the range 0 to 255!");
                }

                return true;
            }

            if (!CheckColor(out var red, args[1], "red")) return false;
            if (!CheckColor(out var green, args[2], "green")) return false;
            if (!CheckColor(out var blue, args[3], "blue")) return false;

            var color = new Color(red / 255f, green / 255f, blue / 255f);


            SlimePainter painter;
            if(Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out var hit))
            {
                painter = hit.collider.GetComponent<SlimePainter>();
                if (!painter  || painter.dataPiece == null)
                {
                    consoleInstance.LogError("Invalid object!");
                    return false;
                }

            }
            else{
                consoleInstance.LogError("You aren't pointed at anything!");
                return false;
            }

            void SetSection(Sections secId)
            {
                switch (secId)
                {
                    case Sections.TOP:
                        painter.dataPiece.SetValue("top", color);
                        break;
                    case Sections.MIDDLE:
                        painter.dataPiece.SetValue("middle", color);
                        break;
                    case Sections.BOTTOM:
                        painter.dataPiece.SetValue("bottom", color);
                        break;
                }
            }

            if (id == Sections.ALL)
            {
                SetSection(Sections.BOTTOM);
                SetSection(Sections.MIDDLE);
                SetSection(Sections.TOP);
            }
            else
            {
                SetSection(id);
            }


            painter.SetColors();

            return true; 
        }

        public enum Sections
        {
            ALL,
            TOP,
            BOTTOM,
            MIDDLE
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0) return new List<string>() { "all", "top", "bottom", "middle" };
            return base.GetAutoComplete(argIndex, argText);
        }
    }
}
