using System.Drawing;
using System.Threading;

namespace MandelbrotGenerator.Generators
{
    public static class ImageGeneratorLogic
    {
        private static int MaxIterations => Settings.DefaultSettings.MaxIterations;
        private static double Border => Settings.DefaultSettings.ZBorder * Settings.DefaultSettings.ZBorder;

        public static Color GetColorForData(Area area, int column, int row, CancellationToken cancelToken)
        {
            var cReal = area.MinReal + column * area.PixelWidth;
            var cImg = area.MinImg + row * area.PixelHeight;
            var zReal = 0.0;
            var zImg = 0.0;

            var k = 0;
            while ((zReal * zReal + zImg * zImg < Border) && (k < MaxIterations))
            {
                if (cancelToken.IsCancellationRequested)
                    return Color.Empty;
                var zNewReal = zReal * zReal - zImg * zImg + cReal;
                var zNewImg = 2 * zReal * zImg + cImg;

                zReal = zNewReal;
                zImg = zNewImg;

                k++;
            }
            return ColorSchema.GetColor(k);
        }
    }
}