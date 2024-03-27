// using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using todolist.Models;
using todolist.Repository;
using todolist.Helpers;
// using System.Text;
// using BCrypt.Net;
// using Microsoft.AspNetCore.Http;


namespace todolist.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserRepo _userRepo;

        private DataContext _context;

        public AuthController(DataContext context, UserRepo userRepo)
        {
            // string conn_ = configuration.GetConnectionString("conn_");
            // _userRepo = new UserRepo(conn_);
            _userRepo = userRepo;
            _context = context;
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
        public async Task<IActionResult> Login()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];


            var users = await _userRepo.GetUsersByUname(username);

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
                    // UserLogin data = new UserLogin
                    // {
                    //     id_user = users.id_user,
                    //     login_time = DateTime.Now
                    // };
                    // _userRepo.SaveSession(data);
                    // HttpContext.Session.SetString("Username", users.username); ;
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