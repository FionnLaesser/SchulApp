using CoreWCF;
using SchulAppSOAP.Models;

namespace SchulAppSOAP.Contracts
{
    [ServiceContract]
    public interface ISchuelerService
    {
        [OperationContract]
        Task<List<SchuelerModel>> GetSchueler();

        [OperationContract]
        Task<SchuelerModel?> GetSchuelerById(int id);

        [OperationContract]
        Task<bool> AddSchueler(SchuelerModel schueler);

        [OperationContract]
        Task<bool> UpdateSchueler(SchuelerModel schueler);

        [OperationContract]
        Task<bool> DeleteSchueler(int id);

        [OperationContract]
        Task<bool> DeleteSchuelerMitAudit(
            int id,
            string? auditUserName,
            string? auditUserRole);
    }
}
