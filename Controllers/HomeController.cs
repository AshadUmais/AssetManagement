using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetMgmt.Data;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;

namespace AssetMgmt.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public HomeController(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<IActionResult> Index()
        {
            var totalAssets = await _ctx.Assets.CountAsync();
            var totalLocations = await _ctx.Locations.CountAsync();
            var totalTransfersThisMonth = await _ctx.AssetTransferLogs
                .CountAsync(t => t.TransferDate.Month == System.DateTime.UtcNow.Month
                              && t.TransferDate.Year == System.DateTime.UtcNow.Year);

            var latestTransfers = await _ctx.AssetTransferLogs
                .Include(t => t.Asset)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .Include(t => t.TransferredByUser)
                .OrderByDescending(t => t.TransferDate)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalAssets = totalAssets;
            ViewBag.TotalLocations = totalLocations;
            ViewBag.TotalTransfersThisMonth = totalTransfersThisMonth;
            return View(latestTransfers);
        }
    }
}
