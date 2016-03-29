using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SharpNeatLib.Maths;

namespace VSS.Wator.Original
{
    public class OriginalWatorWorld: IWatorWorld
    {
        private enum Direction
        {
            Up,
            Down, 
            Right,
            Left
        }

        private readonly Direction[] _directions; 
        private readonly FastRandom _random;
        private readonly int[] _randomMatrix;
        private readonly byte[] _rgbValues;
        private readonly int _maxPosition;

        private int Width { get; set; }
        private int Height { get; set; }
        public Animal[] Grid { get; private set; }

        private int InitialFishPopulation { get; set; }
        public int InitialFishEnergy { get; private set; }
        public int FishBreedTime { get; private set; }

        private int InitialSharkPopulation { get; set; }
        private int InitialSharkEnergy { get; set; }
        public int SharkBreedEnergy { get; private set; }
        public int Iteration { get; set; }

        public OriginalWatorWorld(Settings settings) {
            CopySettings(settings);
            _maxPosition = Width * Height;
            _directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray();
            _rgbValues = new byte[_maxPosition * 4];
            _random = new FastRandom();
            Grid = new Animal[_maxPosition];
            Iteration = 0;

            for (var i = 0; i < _maxPosition; i++) {
                var value = _random.Next(_maxPosition);
                if (value < InitialFishPopulation) 
                    Grid[i] = new Fish(this, i, _random.Next(0, FishBreedTime));
                else if (value < InitialFishPopulation + InitialSharkPopulation) 
                    Grid[i] = new Shark(this, i, _random.Next(0, SharkBreedEnergy));
                else 
                    Grid[i] = null;
            }
            _randomMatrix = Enumerable.Range(0, _maxPosition).ToArray();
        }

        private void CopySettings(Settings settings) {
            Width = settings.Width;
            Height = settings.Height;
            InitialFishPopulation = settings.InitialFishPopulation;
            InitialFishEnergy = settings.InitialFishEnergy;
            FishBreedTime = settings.FishBreedTime;
            InitialSharkPopulation = settings.InitialSharkPopulation;
            InitialSharkEnergy = settings.InitialSharkEnergy;
            SharkBreedEnergy = settings.SharkBreedEnergy;
        }

        public Bitmap GenerateImage() {
            int counter = 0;

            for (int i = 0; i < _maxPosition; i++) {
                var col = Grid[i] == null ? Color.DarkBlue : Grid[i].Color;
                _rgbValues[counter++] = col.B; //  // b
                _rgbValues[counter++] = col.G; // // g
                _rgbValues[counter++] = col.R; //  // R
                _rgbValues[counter++] = col.A; //  // a
            }
           
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, Width, Height);
            var bitmap = new Bitmap(Width, Height);
            System.Drawing.Imaging.BitmapData bmpData = null;
            try {
                bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(_rgbValues, 0, ptr, _rgbValues.Length);
            }
            finally {
                // Unlock the bits.
                if (bmpData != null)
                    bitmap.UnlockBits(bmpData);
            }
            return bitmap;
        }

        public void ExecuteStep() {
            ShuffleArray(_randomMatrix);
            Iteration++;
            for (var i = 0; i < _maxPosition; i++) {
                var animal = Grid[_randomMatrix[i]];
                if (animal != null && !animal.Moved)
                    animal.ExecuteStep();
            }
        }

        private int GetPosition(Direction direction, int position) {
            int pos;
            switch (direction) {
                case Direction.Up:
                    pos = position - Width;
                    if (pos < 0) pos += _maxPosition;
                    return pos;
                case Direction.Down:
                    pos = position + Width;
                    if (pos >= _maxPosition) pos -= _maxPosition;
                    return pos;
                case Direction.Left:
                    pos = position - 1;
                    if ((pos + 1) % Width == 0) pos += Width;
                    return pos;
                case Direction.Right:
                    pos = position + 1;
                    if (pos % Width == 0) pos -= Width;
                    return pos;
                default:
                    throw new ArgumentException("Directiontype not supported.", nameof(direction));
            }
        }

        public int SelectNeighborOfType(Type expectedType, int position, out int freeField)
        {
            ShuffleArray(_directions);
            freeField = -1;
            foreach (var direction in _directions)
            {
                var point = GetPosition(direction, position);
                var animal = Grid[point];
                if (animal == null)
                {
                    freeField = point;
                    continue;
                }
                if (animal.Moved)
                    continue;
                if (animal.GetType() == expectedType)
                    return point;
            }
            return -1;
        }

        public int SelectNeighborOfType<T>(int position, out int freeField) {
            ShuffleArray(_directions);
            freeField = -1;
            foreach (var direction in _directions) {
                var point = GetPosition(direction, position);
                var animal = Grid[point];
                if (animal == null) {
                    freeField = point;
                    continue;
                }
                if (animal.Moved)
                    continue;
                if (animal is T)
                    return point;
            }
            return -1;
        }

        public int SelectFreeNeighbor(int position) {
            ShuffleArray(_directions);
            foreach (var direction in _directions) {
                var point = GetPosition(direction, position);
                if (Grid[point] == null)
                    return point;
            }
            return -1;
        }

        private void ShuffleArray<T>(T[] arr) {
            for (var i = arr.Length - 1; i > 0; i--) {
                var index = _random.Next(i);
                if (index == i)
                    continue;
                var tmp = arr[index];
                arr[index] = arr[i];
                arr[i] = tmp;
            }
        }
    }
}