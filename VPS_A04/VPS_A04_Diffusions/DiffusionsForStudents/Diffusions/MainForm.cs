using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Diffusions
{
    public partial class MainForm: Form
    {
        private Area _currentArea;
        private readonly ImageGenerator _generator;
        private bool _running;
        private const int _TIP_SIZE = 50;
        private const double _DEFAULT_HEAT = 400.0;

        public MainForm() {
            InitializeComponent();
            ResetImage();
            //generator = new SyncImageGenerator();
            _generator = new ParallelImageGenerator();
            _generator.ImageGenerated += generator_ImageGenerated;
            _generator.CalculationFinished += generator_CalculationFinished;
        }

        private void InitArea() {
            _currentArea = new Area(pictureBox.Width, pictureBox.Height);

            for (int i = 0; i < pictureBox.Width - 1; i++) {
                for (int j = 0; j < pictureBox.Height - 1; j++) {
                    _currentArea.Matrix[i, j] = 0;
                }
            }
            Reheat(_currentArea.Matrix, 5, 5, pictureBox.Width, pictureBox.Height, 100, 150);
            Reheat(_currentArea.Matrix, 100, 100, pictureBox.Width, pictureBox.Height, 80, _DEFAULT_HEAT);
        }

        private void Reheat(double[,] matrix, int x, int y, int width, int height, int size, double val) {
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {

                    matrix[(x + i)%width, (y + j)%height] = val;
                }
            }
        }

        private void ResetImage() {
            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics graphics = Graphics.FromImage(pictureBox.Image);
            graphics.FillRectangle(Brushes.Azure, 0, 0, pictureBox.Width, pictureBox.Height);
            graphics.Dispose();
        }

        private void UpdateImage(Area area) {
            toolStripStatusLabel.Text = "Calculating ...";
            _generator.GenerateImage(area);
        }

        private void generator_CalculationFinished(object sender, EventArgs<Tuple<TimeSpan>> e)
        {
            if (InvokeRequired)
                Invoke(new EventHandler<EventArgs<Tuple<TimeSpan>>>(generator_CalculationFinished), sender, e);
            else {
                _running = false;
                startButton.Text = "Start";
                toolStripStatusLabel.Text = "Calculation Finished (Runtime: " + e.Value.Item1 + ")";
            }
        }

        private void generator_ImageGenerated(object sender, EventArgs<Tuple<Area, Bitmap, TimeSpan>> e) {
            if (InvokeRequired)
                Invoke(new EventHandler<EventArgs<Tuple<Area, Bitmap, TimeSpan>>>(generator_ImageGenerated), sender, e);
            else {
                if (_generator.StopRequested)
                    return;
                _currentArea = e.Value.Item1;
                if (e.Value.Item2 != null)
                    pictureBox.Image = e.Value.Item2;

                _running = true;
                startButton.Text = "Stop";
                toolStripStatusLabel.Text = "Calculating. Runtime: " + e.Value.Item3 + ")";
            }
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e) {
            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            var filename = saveFileDialog.FileName;
            ImageFormat format;
            if (filename.EndsWith("jpg"))
                format = ImageFormat.Jpeg;
            else if (filename.EndsWith("gif"))
                format = ImageFormat.Gif;
            else if (filename.EndsWith("png"))
                format = ImageFormat.Png;
            else
                format = ImageFormat.Bmp;
            pictureBox.Image.Save(filename, format);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var dialog = new SettingsDialog()) {
                dialog.ShowDialog();
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e) {
            int x = e.X;
            int y = e.Y;

            if (e.Button == MouseButtons.Left) {
                _generator.Signal.Reset();
                Reheat(_currentArea.Matrix, x, y, _currentArea.Width, _currentArea.Height, _TIP_SIZE, _DEFAULT_HEAT);
                _generator.Signal.Set();
            }
        }

        private void startButton_Click(object sender, EventArgs e) {
            if (_running) {
                _generator.Stop();
            }
            else {
                InitArea();
                UpdateImage(_currentArea);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            _generator.ImageGenerated -= generator_ImageGenerated;
            if (_running)
                Invoke(new Action(_generator.Stop));
        }
    }
}