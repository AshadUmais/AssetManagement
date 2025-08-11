using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssetMgmt.Data;
using AssetMgmt.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AssetMgmt.Controllers
{
    [Authorize]
    public class AssetsController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public AssetsController(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<IActionResult> Index()
        {
            var list = await _ctx.Assets.Include(a => a.Location).Include(a => a.Custodian).ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            ViewBag.Locations = _ctx.Locations.ToList();
            ViewBag.Users = _ctx.UserMasters.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Asset asset)
        {
            ModelState.Remove(nameof(asset.Location));
            ModelState.Remove(nameof(asset.Custodian));
            asset.Location = await _ctx.Locations.FirstOrDefaultAsync(l => l.LocationID == asset.LocationID);

            if (asset.Location == null)
            {
                ModelState.AddModelError("LocationID", "Selected Location does not exist.");
            }
            asset.Custodian = await _ctx.UserMasters.FirstOrDefaultAsync(u => u.UserID == asset.CustodianID);

            if (asset.Custodian == null)
            {
                ModelState.AddModelError("CustodianID", "Selected Custodian does not exist.");
            }
            if (!ModelState.IsValid) {
                ViewBag.Locations = _ctx.Locations.ToList();
                ViewBag.Users = _ctx.UserMasters.ToList();
                return View(asset);
            }

            _ctx.Assets.Add(asset);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var asset = _ctx.Assets.Include(a => a.Location).FirstOrDefault(a => a.AssetID == id);
            if (asset == null) return NotFound();

            ViewBag.Locations = _ctx.Locations.ToList();

            var building = asset.Location?.Building ?? "";
            ViewBag.Users = _ctx.UserMasters.Where(u => u.AssignedBuilding == building).ToList();

            return View(asset);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Asset asset)
        {
            ModelState.Remove(nameof(asset.Location));
            ModelState.Remove(nameof(asset.Custodian));
            asset.Location = await _ctx.Locations.FirstOrDefaultAsync(l => l.LocationID == asset.LocationID);

            if (asset.Location == null)
            {
                ModelState.AddModelError("LocationID", "Selected Location does not exist.");
            }
            asset.Custodian = await _ctx.UserMasters.FirstOrDefaultAsync(u => u.UserID == asset.CustodianID);

            if (asset.Custodian == null)
            {
                ModelState.AddModelError("CustodianID", "Selected Custodian does not exist.");
            }

            if (!ModelState.IsValid) {
                ViewBag.Locations = _ctx.Locations.ToList();
                ViewBag.Users = _ctx.UserMasters.ToList();
                return View(asset);
            }
            _ctx.Assets.Update(asset);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Transfer(int id)
        {
            var asset = await _ctx.Assets.Include(a => a.Location).FirstOrDefaultAsync(a => a.AssetID == id);
            if (asset == null) return NotFound();
            ViewBag.Locations = _ctx.Locations.ToList();
            ViewBag.Users = _ctx.UserMasters.ToList();
            return View(new AssetTransferLog { AssetID = id, FromLocationID = asset.LocationID, TransferDate = System.DateTime.UtcNow });
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(AssetTransferLog model)
        {
            ModelState.Remove(nameof(model.ToLocation));
            ModelState.Remove(nameof(model.FromLocation));
            ModelState.Remove(nameof(model.Asset));
            ModelState.Remove(nameof(model.TransferredByUser));
            ModelState.Remove(nameof(model.TransferredToUser));
            model.ToLocation = _ctx.Locations.FirstOrDefault(l => l.LocationID == model.ToLocationID);
            model.FromLocation = _ctx.Locations.FirstOrDefault(l => l.LocationID == model.FromLocationID);
            model.Asset = _ctx.Assets.FirstOrDefault(l => l.AssetID == model.AssetID);
            model.TransferredByUser = _ctx.UserMasters.FirstOrDefault(l => l.UserID == model.TransferredBy);
            model.TransferredToUser = _ctx.UserMasters.FirstOrDefault(l => l.UserID == model.TransferredTo);

            if (!ModelState.IsValid)
            {
                ViewBag.Locations = _ctx.Locations.ToList();
                return View(model);
            }

            var asset = await _ctx.Assets.FindAsync(model.AssetID);
            if (asset == null) return NotFound();

            asset.LocationID = model.ToLocationID;
            asset.Status = "Transferred";
            asset.CustodianID = model.TransferredTo;

            model.TransferDate = System.DateTime.UtcNow;
            _ctx.AssetTransferLogs.Add(model);

            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> History(int id)
        {
            var logs = await _ctx.AssetTransferLogs
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .Include(t => t.TransferredByUser)
                .Include(t => t.TransferredToUser)
                .Where(t => t.AssetID == id)
                .OrderByDescending(t => t.TransferDate)
                .ToListAsync();
            ViewBag.Asset = await _ctx.Assets.FindAsync(id);
            return View(logs);
        }
        [HttpGet]
        public IActionResult GetCustodiansByLocation(int locationId)
        {
            var building = _ctx.Locations
                .Where(l => l.LocationID == locationId)
                .Select(l => l.Building)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(building))
                return Json(new List<object>());

            var custodians = _ctx.UserMasters
                .Where(u => u.AssignedBuilding == building)
                .Select(u => new { u.UserID, u.FullName })
                .ToList();

            return Json(custodians);
        }
    }
}
