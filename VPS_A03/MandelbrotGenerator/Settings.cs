using System.ComponentModel;

namespace MandelbrotGenerator
{
    [DefaultPropertyAttribute("MinReal")]
    public class Settings
    {
        public static Settings DefaultSettings { get; } = new Settings();

        #region Initial Area

        [Category("Initial Area"),
         Description("Minimum real value of the area")]
        public double MinReal { get; set; }

        [Category("Initial Area"),
         Description("Minimum imaginary value of the area")]
        public double MinImg { get; set; }

        [Category("Initial Area"),
         Description("Maximum real value of the area")]
        public double MaxReal { get; set; }

        [Category("Initial Area"),
         Description("Maximum imaginary value of the area")]
        public double MaxImg { get; set; }

        #endregion

        #region Generator Settings

        private int _maxIterations;

        [Category("Generator Settings"),
         Description("Maximum number of iterations")]
        public int MaxIterations
        {
            get { return _maxIterations; }
            set
            {
                if (value > 0) _maxIterations = value;
            }
        }

        private double _zBorder;

        [Category("Generator Settings"),
         Description("Border value for z")]
        public double ZBorder
        {
            get { return _zBorder; }
            set
            {
                if (value > 0) _zBorder = value;
            }
        }

        #endregion

        #region Parallelization Settings

        private int _workers;

        [Category("Parallelization Settings")]
        [Description("Number of worker threads")]
        public int Workers
        {
            get { return _workers; }
            set
            {
                if (value > 0) _workers = value;
            }
        }

        #endregion

        public Settings()
        {
            MinReal = -1.4;
            MinImg = -0.1;
            MaxReal = -1.32;
            MaxImg = -0.02;
            _maxIterations = 10000;
            _zBorder = 4.0;
            _workers = 15;
        }
    }
}