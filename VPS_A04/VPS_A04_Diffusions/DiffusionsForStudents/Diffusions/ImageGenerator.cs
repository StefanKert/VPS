using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Diffusions
{
    public abstract class ImageGenerator: IImageGenerator
    {
        private CancellationTokenSource _source;
        private bool _stopRequested;
 
        public bool StopRequested => _stopRequested;

        public void GenerateImage(Area area) {
            _source = new CancellationTokenSource();
            _stopRequested = false;
            Task.Run(() => {
                var swOverall = new Stopwatch();
                swOverall.Start();
                for (var i = 0; i < Settings.DefaultSettings.MaxIterations; i++) {
                    if (_source.Token.IsCancellationRequested)
                        return;
                    var sw = new Stopwatch();
                    sw.Start();
                    var bm = GenerateBitmap(area);
                    OnImageGenerated(area, bm, sw.Elapsed);
                    sw.Stop();
                }
                swOverall.Stop();
                OnCalculationFinished(swOverall.Elapsed);
            }, _source.Token);
        }

        public abstract Bitmap GenerateBitmap(Area area);

        public virtual void ColorBitmap(double[,] array, int width, int height, Bitmap bm) {
            var maxColorIndex = ColorSchema.Colors.Count - 1;

            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    var colorIndex = (int)array[i, j]%maxColorIndex;
                    bm.SetPixel(i, j, ColorSchema.Colors[colorIndex]);
                }
            }
        }

        public event EventHandler<EventArgs<Tuple<TimeSpan>>> CalculationFinished;
        public event EventHandler<EventArgs<Tuple<Area, Bitmap, TimeSpan>>> ImageGenerated;

        protected void OnImageGenerated(Area area, Bitmap bitmap, TimeSpan timespan) {
            ImageGenerated?.Invoke(this, new EventArgs<Tuple<Area, Bitmap, TimeSpan>>(new Tuple<Area, Bitmap, TimeSpan>(area, bitmap, timespan)));
        }
        protected void OnCalculationFinished(TimeSpan timespan)
        {
            CalculationFinished?.Invoke(this, new EventArgs<Tuple<TimeSpan>>(new Tuple<TimeSpan>(timespan)));
        }

        public virtual void Stop() {
            _stopRequested = true;
            _source.Cancel();
            OnCalculationFinished(new TimeSpan());
        }
    }
}