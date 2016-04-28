using System;
using System.Diagnostics;
using System.Threading;

namespace MandelbrotGenerator.Generators
{
    public class AsyncImageGenerator : AbstractImageGenerator
    {
        private Thread _calculationThread;

        public override void GenerateImage(Area area)
        {
            if (_calculationThread != null && _calculationThread.IsAlive)
            {
                Source.Cancel();
                Source.Token.WaitHandle.WaitOne();
                Source = new CancellationTokenSource();
            }
            _calculationThread = new Thread(() => BuildBitmap(area));
            _calculationThread.Start();
        }

        protected virtual void BuildBitmap(Area area)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var bm = GenerateBitmap(area, Source.Token);
            stopwatch.Stop();
            OnImageGenerated(area, bm, stopwatch.Elapsed);
        }
    }
}