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

        private Direction[] Directions; 
        private Random random;
        private int[,] randomMatrix;
        private byte[] rgbValues;


        public int Width { get; private set; }
        public int Height { get; private set; }
        public Animal[,] Grid { get; private set; }

        public int InitialFishPopulation { get; private set; }
        public int InitialFishEnergy { get; private set; }
        public int FishBreedTime { get; private set; }

        public int InitialSharkPopulation { get; private set; }
        public int InitialSharkEnergy { get; private set; }
        public int SharkBreedEnergy { get; private set; }

        public OriginalWatorWorld(Settings settings) {
            CopySettings(settings);
            Directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray();
            rgbValues = new byte[Width*Height*4];
            random = new Random();
            Grid = new Animal[Width, Height];

            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    int value = random.Next(Width*Height);
                    if (value < InitialFishPopulation) {
                        Grid[i, j] = new Fish(this, new Point(i, j), random.Next(0, FishBreedTime));
                    }
                    else if (value < InitialFishPopulation + InitialSharkPopulation) {
                        Grid[i, j] = new Shark(this, new Point(i, j), random.Next(0, SharkBreedEnergy));
                    }
                    else {
                        Grid[i, j] = null;
                    }
                }
            }

            randomMatrix = GenerateRandomMatrix(Width, Height);
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
            RandomizeMatrix(randomMatrix);

            for (var i = 0; i < Width; i++) {
                for (var j = 0; j < Height; j++) {
                    var col = randomMatrix[i, j]%Width;
                    var row = randomMatrix[i, j]/Width;

                    if (Grid[col, row] != null && !Grid[col, row].Moved)
                        Grid[col, row].ExecuteStep();
                }
            }

            for (var i = 0; i < Width; i++) {
                for (var j = 0; j < Height; j++) {
                    if (Grid[i, j] != null)
                        Grid[i, j].Commit();
                }
            }
        }
 
        public Bitmap GenerateImage() {
            int counter = 0;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++) {
                    Color col;
                    if (Grid[x, y] == null)
                        col = Color.DarkBlue;
                    else
                        col = Grid[x, y].Color;

                    rgbValues[counter++] = col.B; //  // b
                    rgbValues[counter++] = col.G; // // g
                    rgbValues[counter++] = col.R; //  // R
                    rgbValues[counter++] = col.A; //  // a
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
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, rgbValues.Length);
            }
            finally {
                // Unlock the bits.
                if (bmpData != null)
                    bitmap.UnlockBits(bmpData);
            }
            return bitmap;
        }

        private Point GetPosition(Direction direction, Point position) {
            int i = -1, j = -1;
            switch (direction) {
                case Direction.Up:
                    i = position.X;
                    j = (position.Y + Height - 1)%Height;
                    break;
                case Direction.Down:
                    i = position.X;
                    j = (position.Y + 1)%Height;
                    break;
                case Direction.Left:
                    i = (position.X + Width - 1)%Width;
                    j = position.Y;
                    break;
                case Direction.Right:
                    i = (position.X + 1)%Width;
                    j = position.Y;
                    break;
            }
            return new Point(i, j);
        }

        public Point SelectNeighbor(Type type, Point position) {
            ShuffleArray(Directions);
            foreach(var direction in Directions) {
                var point = GetPosition(direction, position);
                if (IsCellOfType(position, type))
                    return point;
            }
            return new Point(-1, -1);
        }

        private bool IsCellOfType(Point position, Type type) {
            if (type == null && Grid[position.X, position.Y] == null) {
                return true;
            }
            if ((type != null && Grid[position.X, position.Y] == null) ||
                (type == null && Grid[position.X, position.Y] != null)) {
                return false;
            }
            if (Grid[position.X, position.Y].GetType() == type)
                return true;

            return false;
        }

        private void ShuffleArray(Direction[] arr) {
            for (var i = arr.Length - 1; i > 0; i--) {
                var index = random.Next(i);
                var tmp = arr[index];
                arr[index] = arr[i];
                arr[i] = tmp;
            }
        }

        private int[,] GenerateRandomMatrix(int width, int height) {
            int[,] matrix = new int[width, height];
            int row = 0;
            int col = 0;
            for (int i = 0; i < matrix.Length; i++) {
                matrix[col, row] = i;
                col++;
                if (col >= width) {
                    col = 0;
                    row++;
                }
            }
            RandomizeMatrix(matrix);
            return matrix;
        }

        private void RandomizeMatrix(int[,] matrix) {
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);
            int temp, selectedRow, selectedCol;
            int row = 0;
            int col = 0;

            for (int i = 0; i < height*width; i++) {
                temp = matrix[col, row];
                selectedRow = random.Next(row, height);
                if (selectedRow == row)
                    selectedCol = random.Next(col, width);
                else
                    selectedCol = random.Next(width);

                matrix[col, row] = matrix[selectedCol, selectedRow];
                matrix[selectedCol, selectedRow] = temp;

                col++;
                if (col >= width) {
                    col = 0;
                    row++;
                }
            }
        }
    }
}