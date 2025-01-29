using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;

namespace MusicMatch.Services
{
    public class ReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserReport>> GetAllReportsAsync()
        {
            return await _context.UserReports
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportedBy)
                .OrderByDescending(r => r.ReportedAt)
                .ToListAsync();
        }

        public async Task<UserReport> GetReportByIdAsync(int reportId)
        {
            return await _context.UserReports
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportedBy)
                .FirstOrDefaultAsync(r => r.Id == reportId);
        }

        public async Task AddReportAsync(UserReport report)
        {
            _context.UserReports.Add(report);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReportAsync(int reportId)
        {
            var report = await _context.UserReports.FindAsync(reportId);
            if (report != null)
            {
                _context.UserReports.Remove(report);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkReportAsResolved(int reportId)
        {
            var report = await _context.UserReports.FindAsync(reportId);
            if (report != null)
            {
                
                report.IsResolved = true;
                _context.Update(report);
                await _context.SaveChangesAsync();
            }
        }
    }
}
