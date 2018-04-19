using System.ServiceModel;

namespace Service
{
    [ServiceContract]
    public interface IPingService
    {
        [OperationContract]
        string Ping(string msg);
    }
}