using Domain.Interfaces;

namespace Domain.Interfaces
{
    public interface IReportPDFService 
    {
        byte[] GeneratePdf(string reportName, string dsName, object data);
    }
}
