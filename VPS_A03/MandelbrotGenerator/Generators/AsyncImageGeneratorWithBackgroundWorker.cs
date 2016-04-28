using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace MandelbrotGenerator.Generators
{
    public class AsyncImageGeneratorWithBackgroundWorker : AbstractImageGenerator
    {
        private BackgroundWorker _bw;

        public override void GenerateImage(Area area)
        {
            if (_bw != null && _bw.IsBusy)
            {
                Source.Cancel();
                Source.Token.WaitHandle.WaitOne();
                Source = new CancellationTokenSource();
                _bw.CancelAsync();
            }

            _bw = new BackgroundWorker { WorkerSupportsCancellation = true };
            _bw.DoWork += (sender, args) => BuildBitmap(area, args);
            _bw.RunWorkerCompleted += OnBuildBitmapCompleted;
            _bw.RunWorkerAsync();
        }

        private void BuildBitmap(Area area, DoWorkEventArgs args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var bm = GenerateBitmap(area, Source.Token);
            stopwatch.Stop();
            args.Result = new Tuple<Area, Bitmap, TimeSpan>(area, bm, stopwatch.Elapsed);
        }

        private void OnBuildBitmapCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var bw = sender as BackgroundWorker;
            if (bw != null)
                bw.RunWorkerCompleted -= OnBuildBitmapCompleted;
            var res = e.Result as Tuple<Area, Bitmap, TimeSpan>;
            OnImageGenerated(res?.Item1, res?.Item2, res?.Item3 ?? new TimeSpan());
        }
    }
}