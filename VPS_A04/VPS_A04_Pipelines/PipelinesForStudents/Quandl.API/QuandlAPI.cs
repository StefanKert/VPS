
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Quandl.API
{
    [ServiceContract]
    public interface QuandlAPI
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/datasets/GOOG/{identifier}.json?api_key=VzoQm5AD-Xv2xivgQKw7")]
        StockData GetData(string identifier);
    }
}