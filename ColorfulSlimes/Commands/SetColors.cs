using ColorfulSlimes.Components;
using SRML.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static ColorfulSlimes.Main;

namespace ColorfulSlimes.Commands
{
    internal class SetColors : ConsoleCommand
    {
        public enum Sections
        {
            ALL,
            TOP,
            MIDDLE,
            BOTTOM
        }

        public override string ID => "setcolors";

        public override string Usage => "setcolors <all/top/bottom/middle> <red> <green> <blue>";

        public override string Description => "Sets the color for a particular section on the paintable object you're currently looking at.";

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0)
                return new List<string>() { "all", "top", "bottom", "middle" };
            return base.GetAutoComplete(argIndex, argText);
        }

        public override bool Execute(string[] args)
        {
            if (args.Length != 4)
            {
                ModConsole.LogError("Incorrect number of arguments!");
                return false;
            }

            Sections id;
            try
            {
                id = (Sections)Enum.Parse(typeof(Sections), args[0], true);
            }
            catch
            {
                ModConsole.LogError("Invalid section");
                return false;
            }

            if (!IsColorValid(out var red, args[1], "red"))
                return false;

            if (!IsColorValid(out var green, args[2], "green"))
                return false;

            if (!IsColorValid(out var blue, args[3], "blue"))
                return false;

            Color color = new Color(red / 255f, green / 255f, blue / 255f);
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

            if (id == Sections.ALL)
            {
                SetSection(Sections.TOP, painter, color);
                SetSection(Sections.MIDDLE, painter, color);
                SetSection(Sections.BOTTOM, painter, color);
            }
            else
                SetSection(id, painter, color);

            return true;
        }

        bool IsColorValid(out int colorLocal, string colorstring, string colorName)
        {
            if (!int.TryParse(colorstring, out colorLocal))
            {
                ModConsole.LogError($"Color value {colorName} is invalid!");
                return false;
            }

            if (colorLocal < 0 || colorLocal > 255)
                ModConsole.LogError("Color values must be in the range 0 to 255!");

            return true;
        }

        void SetSection(Sections section, Painter painter, Color color)
        {
            switch (section)
            {
                case Sections.TOP:
                    {
                        painter.Paint(color, painter.currMid, painter.currBot);
                        break;
                    }
                case Sections.MIDDLE:
                    {
                        painter.Paint(painter.currTop, color, painter.currBot);
                        break;
                    }
                case Sections.BOTTOM:
                    {
                        painter.Paint(painter.currTop, painter.currMid, color);
                        break;
                    }
            }
        }
    }
}
