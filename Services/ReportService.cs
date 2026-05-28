using Microsoft.EntityFrameworkCore;
using Museum.Models;

namespace Museum.Services
{
    public class ReportService
    {
        private readonly MuseumDbContext _context;

        public ReportService(MuseumDbContext context) => _context = context;

        public async Task<List<ExhibitionAttendanceDto>> GetExhibitionAttendanceReport(DateOnly start, DateOnly end)
        {
            var query = from t in _context.Tickets
                        where t.VisitDate >= start && t.VisitDate <= end
                              && t.TicketStatusFkNavigation.Title == "Использован"
                        group t by t.ExhibitionFkNavigation.Title into g
                        select new ExhibitionAttendanceDto
                        {
                            Exhibition = g.Key,
                            VisitorsCount = g.Count(),
                            Revenue = g.Sum(x => x.SalePrice)
                        };
            return await query.ToListAsync();
        }
    }

    public class ExhibitionAttendanceDto
    {
        public string Exhibition { get; set; }
        public int VisitorsCount { get; set; }
        public double Revenue { get; set; }
    }
}