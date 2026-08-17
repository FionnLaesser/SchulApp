using CoreWCF;
using SchulAppSOAP.Models;

namespace SchulAppSOAP.Contracts
{
    [ServiceContract]
    public interface ISchuelerService
    {
        [OperationContract] //Sagt das es zu SOAP gehört
        Task<List<SchuelerModel>> GetSchueler();

        [OperationContract]
        Task<SchuelerModel?> GetSchuelerById(int id);

        [OperationContract]
        Task<bool> DeleteSchueler(int id);
    }
}