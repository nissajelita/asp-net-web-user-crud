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
    public class AuthController : Controller
    {
        private readonly UserRepo _userRepo;

        public AuthController(IConfiguration configuration)
        {
            string conn_ = configuration.GetConnectionString("conn_");
            _userRepo = new UserRepo(conn_);
        }

        public static string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt();
        }

        public static string HashPassword(string password, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // string hashedEnteredPassword = BCrypt.Net.BCrypt.HashPassword(enteredPassword, salt);

            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }

        public IActionResult Auth()
        {
            return View();
        }
        public IActionResult Login()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            // UserModel users = new UserModel;

            UserModel users = _userRepo.GetUsersByUname(username);

            if (users.username == null)
            {
                TempData["Message"] = "Username tidak ditemukan!";
                return RedirectToAction("Auth");
            }
            else
            {
                var _password = users.password ?? "";
                bool checkPassword = VerifyPassword(password, _password);

                if (checkPassword)
                {
                    UserLogin data = new UserLogin
                    {
                        id_user = users.id,
                        login_time = DateTime.Now
                    };
                    _userRepo.SaveSession(data);
                    TempData["Message"] = "Login Berhasil!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "Password salah!";
                    return RedirectToAction("Auth");
                }
            }
        }
    }
}