using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace Quandl.UI
{
    public interface IQuandlProcessor
    {
        IEnumerable<Series> GetSeriesLists(IEnumerable<string> stockNames);
        Task<IEnumerable<Series>> GetSeriesListsAsync(IEnumerable<string> stockNames);
    }
}