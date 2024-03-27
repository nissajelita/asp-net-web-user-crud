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

        public MasterController(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public static string GenerateSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt();
        }

        // Hash the password using BCrypt with the provided salt
        public static string HashPassword(string password, string salt)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, salt);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userRepo.GetUsers();
            return View("Users/Index", users);
        }
        public IActionResult Testing()
        {
            return View("Testing/test");
        }


        public async Task<IActionResult> EditUsers(int iD)
        {
            var users = await _userRepo.EditUsers(iD);
            if (users == null)
            {
                return NotFound();
            }
            return View("Users/Edit", users);
        }

        [HttpPost]
        public async Task<IActionResult> Save(UserModel obj)
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

            if (string.IsNullOrEmpty(id))
            {
                var user = await _userRepo.GetUsersByUname(username);
                if (user != null)
                {
                    TempData["Message"] = "Username sudah ada!";
                }
            }

            obj.id_user = !string.IsNullOrEmpty(id) ? int.Parse(id) : 0;
            obj.nm_user = nama;
            obj.email = email;
            obj.username = username;
            obj.password = passwordHash;


            try
            {
                await _userRepo.Save(obj);
            }
            catch (System.Exception e)
            {

                TempData["Message"] = e.Message;
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsers(int iD)
        {
            int rowsUpdated = await _userRepo.Delete(iD);

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
            return View("About");
        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        // }
    }
}