using System;
using System.Drawing;
using System.Threading;

namespace MandelbrotGenerator.Generators
{
    public abstract class AbstractImageGenerator
    {
        protected CancellationTokenSource Source;

        protected AbstractImageGenerator() : this(new CancellationTokenSource()) { }
        private AbstractImageGenerator(CancellationTokenSource source)
        {
            Source = source;
        }

        protected virtual Bitmap GenerateBitmap(Area area, CancellationToken cancelToken)
        {
            Bitmap bitmap = new Bitmap(area.Width, area.Height);
            for (int column = 0; column < area.Width; column++)
            {
                for (int row = 0; row < area.Height; row++)
                {
                    if (cancelToken.IsCancellationRequested)
                        return null;
                    bitmap.SetPixel(column, row, ImageGeneratorLogic.GetColorForData(area, column, row, cancelToken));
                }
            }
            return bitmap;
        }



        public abstract void GenerateImage(Area area);

        public event EventHandler<EventArgs<Tuple<Area, Bitmap, TimeSpan>>> ImageGenerated;

        protected void OnImageGenerated(Area area, Bitmap bitmap, TimeSpan timeSpan)
        {
            if (area != null && bitmap != null)
                ImageGenerated?.Invoke(this, new EventArgs<Tuple<Area, Bitmap, TimeSpan>>(new Tuple<Area, Bitmap, TimeSpan>(area, bitmap, timeSpan)));
        }
    }
}