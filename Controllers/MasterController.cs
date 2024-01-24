// using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using todolist.Models;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Tls;
using todolist.Repository;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;



namespace todolist.Controllers
{
    public class MasterController : Controller
    {
        private readonly UserRepo _userRepo;

        public MasterController(IConfiguration configuration)
        {
            string conn_ = configuration.GetConnectionString("conn_");
            _userRepo = new UserRepo(conn_);
        }

        // public MasterController(DbContext dbContext)
        // {
        //     string conn_ = dbContext.GetConnection();
        //     _userRepo = new UserRepo(conn_);
        // }

        // public static string GenerateSalt()
        // {
        //     byte[] saltBytes = new byte[16];
        //     using (var rng = RandomNumberGenerator.Create())
        //     {
        //         rng.GetBytes(saltBytes);
        //     }
        //     return Convert.ToBase64String(saltBytes);
        // }

        // // Hash the password using a salt
        // public static string HashPassword(string password, string salt)
        // {
        //     using (var sha256 = SHA256.Create())
        //     {
        //         var saltedPassword = Encoding.UTF8.GetBytes(password + salt);
        //         var hashedPassword = sha256.ComputeHash(saltedPassword);
        //         return Convert.ToBase64String(hashedPassword);
        //     }
        // }
        public static string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt();
        }

        // Hash the password using BCrypt with the provided salt
        public static string HashPassword(string password, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        public IActionResult Users()
        {
            List<UserModel> users = _userRepo.GetUsers();
            return View("Users/Index", users);
        }

        public IActionResult EditUsers(int iD)
        {
            UserModel users = _userRepo.EditUsers(iD);
            if (users == null)
            {
                return NotFound(); // Or handle when the user is not found
            }
            return View("Users/Edit", users);
        }

        [HttpPost]
        public IActionResult Save()
        {
            var id = Request.Form["id"];
            var nama = Request.Form["nama"];
            var email = Request.Form["email"];
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            string passwordHash = "";

            if (!string.IsNullOrEmpty(password))
            {
                string salt = GenerateSalt();
                passwordHash = HashPassword(password, salt);

            }

            if (string.IsNullOrEmpty(id) && _userRepo.GetUsersByUname(username).username != null)
            {
                TempData["Message"] = "Username sudah ada!";
            }
            else
            {

                UserModel users = new UserModel
                {
                    id = string.IsNullOrEmpty(id) ? (int?)null : int.Parse(id),
                    nama = nama,
                    email = email,
                    username = username,
                    password = passwordHash
                };

                int results = _userRepo.Save(users);
                if (results > 0)
                {
                    TempData["Message"] = "Berhasil Menyimpan Data!";
                }
                else
                {
                    TempData["Message"] = "Gagal Menyimpan Data!";
                }

            }
            return RedirectToAction("Users");
        }

        [HttpPost]
        public IActionResult DeleteUsers(int iD)
        {
            int rowsUpdated = _userRepo.Delete(iD);

            if (rowsUpdated > 0)
            {
                TempData["SuccessMessage"] = "Berhasil Menghapus Data!";
                return RedirectToAction("Users");
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult About()
        {
            return View();
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        // }
    }
}