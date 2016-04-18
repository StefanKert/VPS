using System;
using System.Diagnostics;
using System.Drawing;

namespace MandelbrotGenerator {
  public class SyncImageGenerator : IImageGenerator {
    public static Bitmap GenerateBitmap(Area area) {
      int maxIterations;
      double zBorder;
      double cReal, cImg, zReal, zImg, zNewReal, zNewImg;

      maxIterations = Settings.DefaultSettings.MaxIterations;
      zBorder = Settings.DefaultSettings.ZBorder * Settings.DefaultSettings.ZBorder;

      Bitmap bitmap = new Bitmap(area.Width, area.Height);

      for (int i = 0; i < area.Width; i++) {
        for (int j = 0; j < area.Height; j++) {
          cReal = area.MinReal + i*area.PixelWidth;
          cImg = area.MinImg + j*area.PixelHeight;
          zReal = 0;
          zImg = 0;

          int k = 0;
          while ((zReal*zReal + zImg*zImg < zBorder) && (k < maxIterations)) {
            zNewReal = zReal*zReal - zImg*zImg + cReal;
            zNewImg = 2*zReal*zImg + cImg;

            zReal = zNewReal;
            zImg = zNewImg;

            k++;
          }
          bitmap.SetPixel(i, j, ColorSchema.GetColor(k));
        }
      }


      return bitmap;
    }

    public void GenerateImage(Area area) {
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      var bm = GenerateBitmap(area);
      stopwatch.Stop();

      OnImageGenerated(area, bm, stopwatch.Elapsed);
    }

    public event EventHandler<EventArgs<Tuple<Area, Bitmap, TimeSpan>>> ImageGenerated;

    private void OnImageGenerated(Area area, Bitmap bitmap, TimeSpan timeSpan) {
      ImageGenerated?.Invoke(this,
        new EventArgs<Tuple<Area, Bitmap, TimeSpan>>(new Tuple<Area, Bitmap, TimeSpan>(area, bitmap, timeSpan)));
    }
  }
}
