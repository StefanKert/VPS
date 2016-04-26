using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MandelbrotGenerator
{
    public partial class MainForm : Form
    {
        private Area _currentArea;
        private Point _mouseDownPoint;
        private readonly IImageGenerator _generator;

        public MainForm()
        {
            InitializeComponent();

            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            var graphics = Graphics.FromImage(pictureBox.Image);
            graphics.FillRectangle(Brushes.Azure, 0, 0, pictureBox.Width, pictureBox.Height);
            graphics.Dispose();

            string path = Application.StartupPath;

            _currentArea = new Area
            {
                MinReal = Settings.DefaultSettings.MinReal,
                MinImg = Settings.DefaultSettings.MinImg,
                MaxReal = Settings.DefaultSettings.MaxReal,
                MaxImg = Settings.DefaultSettings.MaxImg,
                Width = pictureBox.Width,
                Height = pictureBox.Height
            };

            _generator = new AsyncImageGenerator();
            _generator.ImageGenerated += GeneratorOnImageGenerated;
        }

        private void GeneratorOnImageGenerated(object sender, EventArgs<Tuple<Area, Bitmap, TimeSpan>> args)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler<EventArgs<Tuple<Area, Bitmap, TimeSpan>>>(GeneratorOnImageGenerated), sender,
                    args);
            }
            else
            {
                _currentArea = args.Value.Item1;
                pictureBox.Image = args.Value.Item2;
                toolStripStatusLabel.Text = "Done (Runtime: " + args.Value.Item3 + ")";
            }
        }

        private void UpdateImage(Area area)
        {
            toolStripStatusLabel.Text = "Calculating ...";

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            _generator.GenerateImage(area);
            //stopwatch.Stop();

            //currentArea = area;
            //pictureBox.Image = bitmap;
            //toolStripStatusLabel.Text = "Done (Runtime: " + stopwatch.Elapsed.ToString() + ")";
        }

        #region Menu events

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
                return;

            var filename = saveFileDialog.FileName;
            ImageFormat format;
            if (filename.EndsWith("jpb")) format = ImageFormat.Jpeg;
            else if (filename.EndsWith("gif")) format = ImageFormat.Gif;
            else if (filename.EndsWith("png")) format = ImageFormat.Png;
            else format = ImageFormat.Bmp;

            pictureBox.Image.Save(filename, format);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SettingsDialog dialog = new SettingsDialog())
                dialog.ShowDialog();
        }

        #endregion

        #region Mouse events

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _mouseDownPoint = e.Location;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var x = Math.Min(e.X, _mouseDownPoint.X);
            var y = Math.Min(e.Y, _mouseDownPoint.Y);
            var width = Math.Abs(e.X - _mouseDownPoint.X);
            var height = Math.Abs(e.Y - _mouseDownPoint.Y);

            pictureBox.Refresh();
            var graphics = pictureBox.CreateGraphics();
            graphics.DrawRectangle(Pens.Yellow, x, y, width, height);
            graphics.Dispose();
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            var area = new Area
            {
                Width = pictureBox.Width,
                Height = pictureBox.Height
            };

            switch (e.Button)
            {
                case MouseButtons.Left:
                    area.MinReal = _currentArea.MinReal + _currentArea.PixelWidth*Math.Min(e.X, _mouseDownPoint.X);
                    area.MinImg = _currentArea.MinImg + _currentArea.PixelHeight*Math.Min(e.Y, _mouseDownPoint.Y);
                    area.MaxReal = _currentArea.MinReal + _currentArea.PixelWidth*Math.Max(e.X, _mouseDownPoint.X);
                    area.MaxImg = _currentArea.MinImg + _currentArea.PixelHeight*Math.Max(e.Y, _mouseDownPoint.Y);
                    break;
                case MouseButtons.Right:
                    area.MinReal = Settings.DefaultSettings.MinReal;
                    area.MinImg = Settings.DefaultSettings.MinImg;
                    area.MaxReal = Settings.DefaultSettings.MaxReal;
                    area.MaxImg = Settings.DefaultSettings.MaxImg;
                    break;
            }

            UpdateImage(area);
        }

        #endregion
    }
}