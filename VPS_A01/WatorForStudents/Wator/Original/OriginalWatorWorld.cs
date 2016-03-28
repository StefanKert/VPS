using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly Random _random;
        private readonly int[] _randomMatrix;
        private readonly byte[] _rgbValues;

        private int Width { get; set; }
        private int Height { get; set; }
        public Animal[] Grid { get; private set; }

        private int InitialFishPopulation { get; set; }
        public int InitialFishEnergy { get; private set; }
        public int FishBreedTime { get; private set; }

        private int InitialSharkPopulation { get; set; }
        private int InitialSharkEnergy { get; set; }
        public int SharkBreedEnergy { get; private set; }

        public OriginalWatorWorld(Settings settings) {
            CopySettings(settings);
            _directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray();
            _rgbValues = new byte[Width*Height*4];
            _random = new Random();
            Grid = new Animal[Width * Height];

            for (var i = 0; i < Width*Height; i++) {
                var value = _random.Next(Width*Height);
                if (value < InitialFishPopulation) 
                    Grid[i] = new Fish(this, i, _random.Next(0, FishBreedTime));
                else if (value < InitialFishPopulation + InitialSharkPopulation) 
                    Grid[i] = new Shark(this, i, _random.Next(0, SharkBreedEnergy));
                else 
                    Grid[i] = null;
            }
            _randomMatrix = Enumerable.Range(0, Width*Height).ToArray();
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

        public void ExecuteStep() {
            ShuffleArray(_randomMatrix);

            for (var i = 0; i < Width*Height; i++) {
                var pos = _randomMatrix[i];

                if (Grid[pos] != null && !Grid[pos].Moved)
                    Grid[pos].ExecuteStep();
            }

            for (var i = 0; i < Width*Height; i++) {
                if (Grid[i] != null)
                    Grid[i].Commit();
            }
        }
 
        public Bitmap GenerateImage() {
            int counter = 0;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++) {
                    Color col;
                    if (Grid[x * y] == null)
                        col = Color.DarkBlue;
                    else
                        col = Grid[x * y].Color;

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

        private int GetPosition(Direction direction, int position) {
            var maxPosition = Width*Height;
            int pos;
            switch (direction) {
                case Direction.Up:
                    pos = position - Width;
                    if (pos < 0)
                        pos += maxPosition;
                    return pos;
                case Direction.Down:
                    pos = position + Width;
                    if (pos >= maxPosition) pos -= maxPosition;
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

        public int SelectNeighbor(Type type, int position) {
            ShuffleArray(_directions);
            foreach(var direction in _directions) {
                var point = GetPosition(direction, position);
                if (IsCellOfType(point, type))
                    return point;
            }
            return -1;
        }

        private bool IsCellOfType(int position, Type type) {
            if (type == null && Grid[position] == null) {
                return true;
            }
            if ((type != null && Grid[position] == null) ||
                (type == null && Grid[position] != null)) {
                return false;
            }
            return Grid[position].GetType() == type;
        }

        private void ShuffleArray<T>(T[] arr) {
            for (var i = arr.Length - 1; i > 0; i--) {
                var index = _random.Next(i);
                var tmp = arr[index];
                arr[index] = arr[i];
                arr[i] = tmp;
            }
        }
    }
}