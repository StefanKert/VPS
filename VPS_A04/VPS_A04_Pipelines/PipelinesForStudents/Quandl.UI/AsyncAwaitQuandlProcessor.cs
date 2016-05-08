using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using Quandl.API;

namespace Quandl.UI
{
    public class AsyncAwaitQuandlProcessor : BaseQuandlProcessor
    {
        public AsyncAwaitQuandlProcessor(QuandlService service) : base(service) { }

        public override async Task<IEnumerable<Series>> GetSeriesListsAsync(IEnumerable<string> stockNames) {
            var seriesList = new ConcurrentBag<Series>();
            await Task.WhenAll(CreateTasksFromStockNames(stockNames, seriesList));
            return seriesList;
        }


        private Task[] CreateTasksFromStockNames(IEnumerable<string> stockNames, ConcurrentBag<Series> seriesList) {
            return stockNames.Select(name => Task.Run(async () => {
                var values = (await RetrieveStockDataAsync(name)).GetValues();
                var innerTasks = new List<Task> {
                    Task.Run(() => seriesList.Add(GetSeries(values, name))),
                    Task.Run(() => seriesList.Add(GetTrend(values, name)))
                };
                await Task.WhenAll(innerTasks.Cast<Task>().ToArray());
            })).ToArray();
        }
    }
}