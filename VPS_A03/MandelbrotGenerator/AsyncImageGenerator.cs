using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace MandelbrotGenerator
{
  public class AsyncImageGenerator : IImageGenerator
  {
    public void GenerateImage(Area area) {
      var t = new Thread(() => {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var bm = SyncImageGenerator.GenerateBitmap(area);
        stopwatch.Stop();

        OnImageGenerated(area, bm, stopwatch.Elapsed);
      });
      t.Start();
    }

    public event EventHandler<EventArgs<Tuple<Area, Bitmap, TimeSpan>>> ImageGenerated;

    private void OnImageGenerated(Area area, Bitmap bitmap, TimeSpan timeSpan) {
      ImageGenerated?.Invoke(this,
        new EventArgs<Tuple<Area, Bitmap, TimeSpan>>(new Tuple<Area, Bitmap, TimeSpan>(area, bitmap, timeSpan)));
    }
  }
}