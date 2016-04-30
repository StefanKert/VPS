using System;
using System.Drawing;

namespace Diffusions
{
    public abstract class ImageGenerator: IImageGenerator
    {
        protected bool stopRequested = false;

        public bool StopRequested
        {
            get { return stopRequested; }
        }

        protected bool finished = false;

        public bool Finished
        {
            get { return finished; }
        }

        public abstract void GenerateImage(Area area); //TODO

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
            var handler = ImageGenerated;
            if (handler != null)
                handler(this, new EventArgs<Tuple<Area, Bitmap, TimeSpan>>(new Tuple<Area, Bitmap, TimeSpan>(area, bitmap, timespan)));
        }

        public virtual void Stop() {
            stopRequested = true;
            //TODO
        }
    }
}