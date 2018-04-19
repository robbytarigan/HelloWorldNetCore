using System.ServiceModel;

namespace Client
{
    [ServiceContract]
    public interface IPingService
    {
        [OperationContract]
        string Ping(string msg);
    }
}