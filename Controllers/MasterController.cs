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

            UserModel users = new UserModel
            {
                id = string.IsNullOrEmpty(id) ? (int?)null : int.Parse(id),
                nama = nama,
                email = email
            };

            int rowsUpdated = _userRepo.Save(users);
            if (rowsUpdated > 0)
            {
                TempData["SuccessMessage"] = "Berhasil Menyimpan Data!";
                return RedirectToAction("Users");
            }
            else
            {
                return NotFound();
            }
            // return RedirectToAction("Users");
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

        // public IActionResult About()
        // {
        //     return View();
        // }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        // }
    }
}