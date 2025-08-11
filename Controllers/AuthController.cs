using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssetMgmt.Data;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AssetMgmt.Models; // Assuming UserMaster is here
using System;

namespace AssetMgmt.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _ctx;
        private readonly PasswordHasher<UserMaster> _passwordHasher;

        public AuthController(IConfiguration config, ApplicationDbContext ctx)
        {
            _config = config;
            _ctx = ctx;
            _passwordHasher = new PasswordHasher<UserMaster>();
        }
       
        public IActionResult UsersList()
        {
            var users = _ctx.UserMasters.ToList();
            return View(users);
        }
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _ctx.UserMasters.FirstOrDefault(u => u.Email == email); 
          
            if (user == null)
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiryMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            Response.Cookies.Append("AuthToken", new JwtSecurityTokenHandler().WriteToken(token),
                new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = true });

            return RedirectToAction("Index", "Home");
        }

        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login");
        }


        public IActionResult CreateUser()
        {
            var locations = _ctx.Locations.ToList();
            ViewBag.Locations = locations;
            return View();
        }

        [HttpPost]
        public IActionResult CreateUser(string fullName, string email, string password, string designation, int locationId)
        {
            if (_ctx.UserMasters.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email already exists";
                return View();
            }

            var user = new UserMaster
            {
                FullName = fullName,
                Email = email
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            user.Designation = designation; 
            var location = _ctx.Locations.FirstOrDefault(u => u.LocationID == locationId) ?? throw new Exception("Location not found");
            user.AssignedBuilding = location.Building;
            user.AssignedFloor = location.Floor;
            user.AssignedRoom = location.Room;

            _ctx.UserMasters.Add(user);
            _ctx.SaveChanges();

            return RedirectToAction("Login");
        }

       
        public IActionResult EditUser(int id)
        {
            var user = _ctx.UserMasters.FirstOrDefault(u => u.UserID == id);
            if (user == null) return NotFound();
            var locations = _ctx.Locations.ToList();
            ViewBag.Locations = locations;
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(int id, string fullName, string email, string password, int locationId, string designation)
        {
            var user = _ctx.UserMasters.FirstOrDefault(u => u.UserID == id);
            if (user == null) return NotFound();
            
            user.FullName = fullName;
            user.Email = email;

            if (!string.IsNullOrWhiteSpace(password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, password);
            }
            user.Designation = designation;
            var location = _ctx.Locations.FirstOrDefault(u => u.LocationID == locationId) ?? throw new Exception("Location not found");
            user.AssignedBuilding = location.Building;
            user.AssignedFloor = location.Floor;
            user.AssignedRoom = location.Room;
            _ctx.UserMasters.Update(user);
            _ctx.SaveChanges();

            return RedirectToAction("Login");
        }
    }
}
