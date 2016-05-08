using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using Quandl.API;

namespace Quandl.UI
{
    public class TaskContinueWithQuandlProcessor : BaseQuandlProcessor
    {
        public TaskContinueWithQuandlProcessor(QuandlService service) : base(service) { }

        public override Task<IEnumerable<Series>> GetSeriesListsAsync(IEnumerable<string> stockNames) {
            var seriesList = new ConcurrentBag<Series>();
            return Task.Run(() => {
                Task.WaitAll(CreateTasksFromStockNames(stockNames, seriesList));
                return seriesList.AsEnumerable();
            });
        }

        private Task[] CreateTasksFromStockNames(IEnumerable<string> stockNames, ConcurrentBag<Series> seriesList) {
            return stockNames.Select(name => {
                return RetrieveStockDataAsync(name).ContinueWith(x => {
                    var values = x.Result.GetValues();
                    var innerTasks = new List<Task> {
                        Task.Run(() => seriesList.Add(GetSeries(values, name))),
                        Task.Run(() => seriesList.Add(GetTrend(values, name)))
                    };
                    Task.WaitAll(innerTasks.ToArray());
                });
            }).ToArray();
        }
    }
}