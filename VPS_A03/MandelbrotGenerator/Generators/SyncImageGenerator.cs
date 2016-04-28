using System.Diagnostics;

namespace MandelbrotGenerator.Generators
{
    public class SyncImageGenerator : AbstractImageGenerator
    {
        public override void GenerateImage(Area area)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var bm = GenerateBitmap(area, Source.Token);
            stopwatch.Stop();
            OnImageGenerated(area, bm, stopwatch.Elapsed);
        }
    }
}