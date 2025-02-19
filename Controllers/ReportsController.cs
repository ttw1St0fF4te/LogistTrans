namespace LogistTrans.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using LogistTrans.Context;
using LogistTrans.Models;
using System.Text;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

[Authorize(Roles = "admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reportData = new ReportData
            {
                TotalProductsSold = await _context.OrderItems.SumAsync(oi => oi.Quantity),
                TotalClients = await _context.Clients.CountAsync(),
                ActiveClients = await _context.Orders.Select(o => o.ClientId).Distinct().CountAsync(),
                TotalDrivers = await _context.Employees.CountAsync(e => e.Login.Role.RoleName == "driver"),
                DriverWorkload = await _context.Routes
                    .GroupBy(r => r.EmployeeId)
                    .Select(g => new DriverWorkload
                    {
                        DriverId = g.Key,
                        RouteCount = g.Count()
                    })
                    .ToListAsync()
            };

            return View(reportData);
        }

        public async Task<IActionResult> ExportCsv()
        {
            var reportData = await GetReportData();
            var csv = GenerateCsv(reportData);
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", "Report.csv");
        }

        public async Task<IActionResult> ExportPdf()
        {
            var reportData = await GetReportData();
            var pdf = GeneratePdf(reportData);
            return File(pdf, "application/pdf", "Report.pdf");
        }

        private async Task<ReportData> GetReportData()
        {
            return new ReportData
            {
                TotalProductsSold = await _context.OrderItems.SumAsync(oi => oi.Quantity),
                TotalClients = await _context.Clients.CountAsync(),
                ActiveClients = await _context.Orders.Select(o => o.ClientId).Distinct().CountAsync(),
                TotalDrivers = await _context.Employees.CountAsync(e => e.Login.Role.RoleName == "driver"),
                DriverWorkload = await _context.Routes
                    .GroupBy(r => r.EmployeeId)
                    .Select(g => new DriverWorkload
                    {
                        DriverId = g.Key,
                        RouteCount = g.Count()
                    })
                    .ToListAsync()
            };
        }

        private string GenerateCsv(ReportData reportData)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Statistic,Value");
            csv.AppendLine($"Total Products Sold,{reportData.TotalProductsSold}");
            csv.AppendLine($"Total Clients,{reportData.TotalClients}");
            csv.AppendLine($"Active Clients,{reportData.ActiveClients}");
            csv.AppendLine($"Total Drivers,{reportData.TotalDrivers}");
            foreach (var workload in reportData.DriverWorkload)
            {
                csv.AppendLine($"Driver {workload.DriverId} Routes,{workload.RouteCount}");
            }
            return csv.ToString();
        }

        private byte[] GeneratePdf(ReportData reportData)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header()
                        .AlignCenter()
                        .Text("Report")
                        .FontSize(20);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(col =>
                        {
                            col.Item().Text($"Total Products Sold: {reportData.TotalProductsSold}");
                            col.Item().Text($"Total Clients: {reportData.TotalClients}");
                            col.Item().Text($"Active Clients: {reportData.ActiveClients}");
                            col.Item().Text($"Total Drivers: {reportData.TotalDrivers}");
                            col.Item().Text("Driver Workload:");
                            foreach (var workload in reportData.DriverWorkload)
                            {
                                col.Item().Text($"Driver {workload.DriverId}: {workload.RouteCount} routes");
                            }
                        });
                });
            }).GeneratePdf();

            return pdf;
        }
    }

    public class ReportData
    {
        public int TotalProductsSold { get; set; }
        public int TotalClients { get; set; }
        public int ActiveClients { get; set; }
        public int TotalDrivers { get; set; }
        public List<DriverWorkload> DriverWorkload { get; set; }
    }

    public class DriverWorkload
    {
        public int DriverId { get; set; }
        public int RouteCount { get; set; }
    }