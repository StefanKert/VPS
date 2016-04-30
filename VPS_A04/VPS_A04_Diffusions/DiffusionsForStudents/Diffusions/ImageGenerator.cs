using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace Diffusions
{
    public abstract class ImageGenerator: IImageGenerator
    {
        protected bool stopRequested = false;

        public bool StopRequested => stopRequested;

        protected bool finished = false;
   
        public bool Finished => finished;

        public void GenerateImage(Area area) {
            Task.Run(() => {
                for (var i = 0; i < Settings.DefaultSettings.MaxIterations; i++) {
                    var sw = new Stopwatch();
                    sw.Start();
                    var bm = GenerateBitmap(area);
                    sw.Stop();
                    OnImageGenerated(area, bm, sw.Elapsed);
                }
            });
        }

        public abstract Bitmap GenerateBitmap(Area area);

        public virtual void ColorBitmap(double[,] array, int width, int height, Bitmap bm) {
            int maxColorIndex = ColorSchema.Colors.Count - 1;

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    int colorIndex = (int)array[i, j]%maxColorIndex;
                    bm.SetPixel(i, j, ColorSchema.Colors[colorIndex]);
                }
            }
        }

        public event EventHandler<EventArgs<Tuple<Area, Bitmap, TimeSpan>>> ImageGenerated;

        protected void OnImageGenerated(Area area, Bitmap bitmap, TimeSpan timespan) {
            ImageGenerated?.Invoke(this, new EventArgs<Tuple<Area, Bitmap, TimeSpan>>(new Tuple<Area, Bitmap, TimeSpan>(area, bitmap, timespan)));
        }

        public virtual void Stop() {
            stopRequested = true;
            //TODO
        }
    }
}