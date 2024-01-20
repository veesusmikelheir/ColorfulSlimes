using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Utility
{
    public static List<Color> GenerateColors(int maxColors = 16777216)
    {
        List<Color> allColors = new List<Color>();
        HashSet<Color> usedColors = new HashSet<Color>();

        while (allColors.Count < maxColors)
        {
            float r = UnityEngine.Random.value;
            float g = UnityEngine.Random.value;
            float b = UnityEngine.Random.value;

            Color color = new Color(r, g, b);

            if (!usedColors.Contains(color))
            {
                allColors.Add(color);
                usedColors.Add(color);
            }
        }

        return allColors;
    }

    public static void ColorX8Shader(this Material material, Color[] X8Colors)
    {
        if (X8Colors.Length < 8)
            throw new NullReferenceException("Please have at least 8 colors in your X8 Colors Array.");
        if (X8Colors.Length > 8)
            throw new NullReferenceException("Please don't have more than 8 colors in your X8 Colors Array.");

        material.SetColor("_Color00", X8Colors[0]);
        material.SetColor("_Color11", X8Colors[1]);
        material.SetColor("_Color21", X8Colors[2]);
        material.SetColor("_Color31", X8Colors[3]);
        material.SetColor("_Color41", X8Colors[4]);
        material.SetColor("_Color51", X8Colors[5]);
        material.SetColor("_Color61", X8Colors[6]);
        material.SetColor("_Color71", X8Colors[7]);
    }
}