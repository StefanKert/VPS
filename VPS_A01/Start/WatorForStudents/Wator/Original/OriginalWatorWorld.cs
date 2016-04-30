using System;
using System.Drawing;
using System.Threading.Tasks;

namespace VSS.Wator.Original
{
    // object-oriented implementation of the wator world simulation
    public class OriginalWatorWorld: IWatorWorld
    {
        // random number generator
        private Random random { get; set; }

        [ThreadStatic]
        private static Random localRandom;

        // A matrix of ints that determines the order of execution of each cell of the world.
        // this matrix is shuffled in each time step.
        // Cells of the world must be executed in a random order,
        // otherwise the animal in the first cell is always allowed to move first.
        private int[,] randomMatrix;

        // for visualization
        private byte[] rgbValues;

        #region Properties
        // width (number of cells) of the world
        public int Width { get; private set; }
        // height (number of cells) of the world
        public int Height { get; private set; }
        // the cells of the world (2D-array of animal (fish or shark), empty cells have the value null)
        public Animal[,] Grid { get; private set; }

        // simulation parameters
        public int InitialFishPopulation { get; private set; }
        public int InitialFishEnergy { get; private set; }
        public int FishBreedTime { get; private set; }

        public int InitialSharkPopulation { get; private set; }
        public int InitialSharkEnergy { get; private set; }
        public int SharkBreedEnergy { get; private set; }
        #endregion

        // create and init a new wator world with the given settings
        public OriginalWatorWorld(Settings settings) {
            // copy settings 
            Width = settings.Width;
            Height = settings.Height;
            InitialFishPopulation = settings.InitialFishPopulation;
            InitialFishEnergy = settings.InitialFishEnergy;
            FishBreedTime = settings.FishBreedTime;
            InitialSharkPopulation = settings.InitialSharkPopulation;
            InitialSharkEnergy = settings.InitialSharkEnergy;
            SharkBreedEnergy = settings.SharkBreedEnergy;

            rgbValues = new byte[Width*Height*4];

            random = new Random();
            Grid = new Animal[Width, Height];

            // initialize the population by placing the required number of shark and fish
            // randomly on the grid
            // randomMatrix contains all values from 0 .. Width*Height in a random ordering
            // so we can simply place a fish onto a cell if the value in the same cell of the
            // randomMatrix is smaller then the number of fish 
            // subsequently we can place a shark if the number in randomMatrix is smaller than
            // the number of fish and shark
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

            // populate the random matrix that determines the order of execution for the cells
            randomMatrix = GenerateRandomMatrix(Width, Height);
        }

        // execute one time step of the simulation. Each cell of the world must be executed once
        // Animal move around on the grid. To make sure each animal is executed only once we
        // use the moved flag.
        public void ExecuteStep() {
            var redRows = new Task[Height/4];
            var blackRows = new Task[Height/4];

            for (int i = 0; i < Height/4; i++) {
                redRows[i] = new Task(CreateExecuteAction(i*4 + 0, i*4 + 2, new Random(random.Next())));
                blackRows[i] = new Task(CreateExecuteAction(i*4 + 2, i*4 + 4, new Random(random.Next())));
            }

            var redPhase = new Task(() => {
                foreach (var task in redRows) {
                    task.Start();
                }
                Task.WaitAll(redRows);
            });
            var blackPhase = new Task(() => {
                foreach (var task in blackRows) {
                    task.Start();
                }
                Task.WaitAll(blackRows);
            });

            var remainingPhase = new Task(() => { CreateExecuteAction(Height - Height%4, Height, new Random(random.Next())).Invoke(); });

            ExecuteInRandomOrder(redPhase, blackPhase, remainingPhase);

            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    if (Grid[i, j] != null)
                        Grid[i, j].Commit();
                }
            }
        }

        private int[][] permutations = {new[] {0, 1, 2}, new [] {0, 2, 1}, new [] {1, 0, 2}, new [] {1, 2, 0}, new [] {2, 0, 1}, new [] {2, 1, 0},};

        private void ExecuteInRandomOrder(params Task[] tasks) {
            var selectedPermutation = permutations[random.Next(permutations.Length)];
            foreach (int idx in selectedPermutation) {
                tasks[idx].RunSynchronously();
            }
        }

        private Action CreateExecuteAction(int startRow, int endRow, Random random) {
            return () => {
                localRandom = random;
                RandomizeMatrix(randomMatrix, startRow, endRow);

                int row, col;
                for (int i = 0; i < Width; i++)
                {
                    for (int j = startRow; j < endRow; j++)
                    {
                        col = randomMatrix[i, j] % Width;
                        row = randomMatrix[i, j] / Width;
                        if (Grid[col, row] != null && !Grid[col, row].Moved)
                            Grid[col, row].ExecuteStep();
                    }
                }
            };
        }

        // generates a bitmap for the current state of the wator world
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

        // find all neighbouring cells of the given position that contain an animal of the given type
        public Point[] GetNeighbors(Type type, Point position) {
            Point[] neighbors = new Point[4];
            int neighborIndex;
            int i, j;

            // counter for the number of cells of the correct type
            neighborIndex = 0;
            // look up
            i = position.X;
            j = (position.Y + Height - 1)%Height;
            // if we look for empty cells (null) we don't have to check the type using instanceOf
            if ((type == null) && (Grid[i, j] == null)) {
                neighbors[neighborIndex] = new Point(i, j);
                neighborIndex++;
            }
            else if ((type != null) && (type.IsInstanceOfType(Grid[i, j]))) {
                // using instanceOf to check if the type of the animal on grid cell (i/j) is either a shark of a fish
                // animals that moved in this iteration onto the given cell are not considered
                // because the simulation runs in discrete time steps
                if ((Grid[i, j] != null) && (!Grid[i, j].Moved)) {
                    neighbors[neighborIndex] = new Point(i, j);
                    neighborIndex++;
                }
            }
            // look right
            i = (position.X + 1)%Width;
            j = position.Y;
            if ((type == null) && (Grid[i, j] == null)) {
                neighbors[neighborIndex] = new Point(i, j);
                neighborIndex++;
            }
            else if ((type != null) && (type.IsInstanceOfType(Grid[i, j]))) {
                if ((Grid[i, j] != null) && (!Grid[i, j].Moved)) {
                    neighbors[neighborIndex] = new Point(i, j);
                    neighborIndex++;
                }
            }
            // look down
            i = position.X;
            j = (position.Y + 1)%Height;
            if ((type == null) && (Grid[i, j] == null)) {
                neighbors[neighborIndex] = new Point(i, j);
                neighborIndex++;
            }
            else if ((type != null) && (type.IsInstanceOfType(Grid[i, j]))) {
                if ((Grid[i, j] != null) && (!Grid[i, j].Moved)) {
                    neighbors[neighborIndex] = new Point(i, j);
                    neighborIndex++;
                }
            }
            // look left
            i = (position.X + Width - 1)%Width;
            j = position.Y;
            if ((type == null) && (Grid[i, j] == null)) {
                neighbors[neighborIndex] = new Point(i, j);
                neighborIndex++;
            }
            else if ((type != null) && (type.IsInstanceOfType(Grid[i, j]))) {
                if ((Grid[i, j] != null) && (!Grid[i, j].Moved)) {
                    neighbors[neighborIndex] = new Point(i, j);
                    neighborIndex++;
                }
            }

            // create a result array of the correct length containing only
            // the discovered cells of the correct type
            Point[] result = new Point[neighborIndex];
            Array.Copy(neighbors, result, neighborIndex);
            return result;
        }

        // select a random neighbouring cell that contains an animal (or null) of the given type
        public Point SelectNeighbor(Type type, Point position) {
            // first determine _all_ neighbours of the given type
            Point[] neighbors = GetNeighbors(type, position);
            if (neighbors.Length > 1) {
                // if more than one cell has been found => return a randomly selected cell
                return neighbors[localRandom.Next(neighbors.Length)];
            }
            else if (neighbors.Length == 1) {
                // if only a single cell contains an animal of the given type we can save the call to random
                return neighbors[0];
            }
            else {
                // return a point with negative coordinates to indicate
                // that no neighbouring cell has found
                // return value must be checked by the caller
                return new Point(-1, -1);
            }
        }

        // create a 2D array containing all numbers in the range 0 .. width * height
        // the numbers are shuffled to create a random ordering
        private int[,] GenerateRandomMatrix(int width, int height) {
            int[,] matrix = new int[width, height];

            // initialize
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
            return matrix;
        }

        // shuffle the values of the 2D array in a random fashion
        private void RandomizeMatrix(int[,] matrix, int startRow, int endRow) {
            int width = matrix.GetLength(0);

            int row = startRow;
            int col = 0;

            for (int i = 0; i < (endRow - startRow) * width; i++) {
                var temp = matrix[col, row];
                var selectedRow = localRandom.Next(row, endRow);
                var selectedCol = selectedRow == row ? localRandom.Next(col, width) : localRandom.Next(width);

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