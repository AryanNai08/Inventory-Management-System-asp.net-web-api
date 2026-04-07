
using AspNetCore.Reporting;
using Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.ThirdPartyServices
{
    public class ReportPDFService : IReportPDFService
    {
        private readonly IWebHostEnvironment _env;

        public ReportPDFService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public byte[] GeneratePdf(string reportName, string dsName, object data)
        {
            string reportPath = Path.Combine(
                _env.ContentRootPath,
                "Reports",
                $"{reportName}.rdlc"
            );

            if (!File.Exists(reportPath))
                throw new Exception("RDLC not found");

            LocalReport report = new LocalReport(reportPath);

            report.AddDataSource(dsName, data);

            var result = report.Execute(RenderType.Pdf, 1);

            return result.MainStream;
        }
    }
}
