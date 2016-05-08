using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using Quandl.API;

namespace Quandl.UI
{
    public abstract class BaseQuandlProcessor: IQuandlProcessor
    {
        private const int INTERVAL = 2000;
        private readonly QuandlService _service;

        public BaseQuandlProcessor(QuandlService service) {
            _service = service;
        }

        public IEnumerable<Series> GetSeriesLists(IEnumerable<string> stockNames) {
            List<Series> seriesList = new List<Series>();

            foreach (var name in stockNames) {
                StockData sd = RetrieveStockData(name);
                List<StockValue> values = sd.GetValues();
                seriesList.Add(GetSeries(values, name));
                seriesList.Add(GetTrend(values, name));
            }
            return seriesList;
        }

        public abstract Task<IEnumerable<Series>> GetSeriesListsAsync(IEnumerable<string> stockNames);

        protected StockData RetrieveStockData(string name) {
            return _service.GetData(name);
        }

        protected Task<StockData> RetrieveStockDataAsync(string name) {
            return Task.Run(() => _service.GetData(name));
        }

        protected Series GetSeries(List<StockValue> stockValues, string name) {
            Series series = new Series(name) {ChartType = SeriesChartType.FastLine};

            int j = 0;
            for (int i = stockValues.Count - INTERVAL; i < stockValues.Count; i++) {
                series.Points.Add(new DataPoint(j++, stockValues[i].Close));
            }
            return series;
        }

        protected Series GetTrend(List<StockValue> stockValues, string name) {
            double k, d;
            Series series = new Series(name + " Trend") {ChartType = SeriesChartType.FastLine};

            var vals = stockValues.Select(x => x.Close).ToArray();
            LinearLeastSquaresFitting.Calculate(vals, out k, out d);

            int j = 0;
            for (int i = stockValues.Count - INTERVAL; i < stockValues.Count; i++) {
                series.Points.Add(new DataPoint(j++, k*i + d));
            }
            return series;
        }
    }
}