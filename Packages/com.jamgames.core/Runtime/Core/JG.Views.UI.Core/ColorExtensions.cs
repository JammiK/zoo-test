using System.Collections.Generic;
using UnityEngine;

namespace Core.JG.Extensions
{
    /// <summary>
    /// http://wiki.unity3d.com/index.php?title=HexConverter
    /// </summary>
    public class ColorExtensions
    {
        static readonly Dictionary<string, Color> colourCache_ = new Dictionary<string, Color>();

        public static string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
            return hex;
        }

        public static Color HexToColor(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return Color.white;
            }

            Color resultingColor;
            if (colourCache_.TryGetValue(hex, out resultingColor))
            {
                return resultingColor;
            }

            string str = hex[0] == '#' ? hex.Substring(1) : hex;

            float r = byte.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            float g = byte.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            float b = byte.Parse(str.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

            float a = str.Length == 8
                ? byte.Parse(str.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
                : 256f;

            resultingColor = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
            colourCache_.Add(hex, resultingColor);

            return resultingColor;
        }
    }
}
