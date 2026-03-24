using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IReportPDFService
    {
        byte[] GeneratePdf(string reportName, string dsName, object data);
    }
}
