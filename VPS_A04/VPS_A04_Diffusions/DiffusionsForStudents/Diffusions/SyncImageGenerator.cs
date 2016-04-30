using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Diffusions
{
    public class SyncImageGenerator : ImageGenerator
    {
        public override Bitmap GenerateBitmap(Area area) {
            var matrix = area.Matrix;
            var height = area.Height;
            var width = area.Width;
            var newMatrix = new double[width, height];
            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    var jp = (j + height - 1) % height;
                    var jm = (j + 1) % height;
                    var ip = (i + 1) % width;
                    var im = (i + width - 1) % width;

                    newMatrix[i, j] = (matrix[i, jp] + matrix[i, jm] + matrix[ip, j] + matrix[im, j] + matrix[ip, jp] + matrix[ip, jm] + matrix[im, jp] + matrix[im, jm]) / 8.0;
                }
            }
            Bitmap bm = new Bitmap(width, height);
            ColorBitmap(newMatrix, width, height, bm);
            area.Matrix = newMatrix;
            return bm;
        }
    }
}
