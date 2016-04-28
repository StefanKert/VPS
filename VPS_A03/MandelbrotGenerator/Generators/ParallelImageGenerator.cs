using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Threading;

namespace MandelbrotGenerator.Generators
{
    public class ParallelImageGenerator : AsyncImageGenerator
    {
        private readonly object _bitmapLock = new object();
        private readonly object _currentRowLock = new object();
        private int _currentRow;
        private Bitmap _bitmap;

        protected  override void BuildBitmap(Area area)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _currentRow = 0;
            _bitmap = new Bitmap(area.Width, area.Height);
            var threads = new Thread[Settings.DefaultSettings.Workers];

            for (int i = 0; i < threads.Length; i++) {
                Thread t = new Thread(() => BuildBitmap(area, Source.Token));
                threads[i] = t;
                t.Start();
            }

            foreach (var thread in threads) {
                thread.Join();
            }
            stopwatch.Stop();
            if (Source.Token.IsCancellationRequested)
                _bitmap = null;
            OnImageGenerated(area, _bitmap, stopwatch.Elapsed);
        }


        private void BuildBitmap(Area area, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var row = GetCurrentRow();
                if (row >= area.Height)
                    return;

                for (int column = 0; column < area.Width; column++) {
                    var color = ImageGeneratorLogic.GetColorForData(area, column, row, cancellationToken);
                    lock (_bitmapLock) {
                        _bitmap.SetPixel(column, row, color);
                    }
                }
            }
        }

        private int GetCurrentRow() {
            int row;
            lock (_currentRowLock) {
                row = _currentRow;
                _currentRow++;
            }
            return row;
        }
    }
}