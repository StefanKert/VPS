using System.Drawing;

namespace MandelbrotGenerator
{
    public static class ColorSchema
    {
        public static Color GetColor(int iterations)
        {
            if (iterations == Settings.DefaultSettings.MaxIterations)
                return Color.Black;

            var red = (iterations%32)*3;
            if (red > 255)
                red = 255;

            var green = (iterations%16)*2;
            if (green > 255)
                green = 255;

            var blue = (iterations%128)*14;
            if (blue > 255)
                blue = 255;

            return Color.FromArgb(red, green, blue);
        }
    }
}