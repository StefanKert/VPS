using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Quandl.API;
#pragma warning disable 4014

namespace Quandl.UI
{
    public partial class QuandlViewer: Form
    {
        private readonly IQuandlProcessor _processor;
        private readonly string[] names = {"NASDAQ_MSFT", "NASDAQ_AAPL", "NASDAQ_GOOG"};
     
        public QuandlViewer() {
            InitializeComponent();
            var service = new QuandlService();
            _processor = new AsyncAwaitQuandlProcessor(service); 
        }

        private async void displayButton_Click(object sender, EventArgs e) {
            displayButton.Enabled = false;
            OnDataRetrieved(await _processor.GetSeriesListsAsync(names));
        }

        private void OnDataRetrieved(IEnumerable<Series> series) {
            DisplayData(series);
            SaveImage("chart");
            displayButton.Enabled = true;
        }

        private void DisplayData(IEnumerable<Series> seriesList) {
            chart.Series.Clear();
            foreach (var series in seriesList) {
                chart.Series.Add(series);
            }
        }

        private void SaveImage(string fileName) {
            chart.SaveImage(fileName + ".jpg", ChartImageFormat.Jpeg);
        }
    }
}