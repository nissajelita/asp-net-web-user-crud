// using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using todolist.Models;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Tls;



namespace todolist.Controllers
{
    public class MasterController : Controller
    {
        private readonly IConfiguration _configuration;

        public MasterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Users()
        {
            string conn_ = _configuration.GetConnectionString("conn_");

            List<UserModel> users = new List<UserModel>();

            using (MySqlConnection connection = new MySqlConnection(conn_))
            {
                connection.Open();
                string query = "SELECT * FROM mst_user where deleted_status is null";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserModel user = new UserModel
                            {
                                id = reader["id_user"] != DBNull.Value ? (int)reader["id_user"] : (int?)null,
                                nama = reader["nm_user"] != DBNull.Value ? reader["nm_user"].ToString() : null
                            };
                            users.Add(user);
                        }
                    }
                }
            }
            return View("Users/Index", users);
        }

        public IActionResult EditUsers(int iD)
        {
            string conn_ = _configuration.GetConnectionString("conn_");

            UserModel user = new UserModel();

            using (MySqlConnection connection = new MySqlConnection(conn_))
            {
                connection.Open();
                string query = "SELECT * FROM mst_user where id_user = @userId";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", iD);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new UserModel
                            {
                                id = reader["id_user"] != DBNull.Value ? (int)reader["id_user"] : (int?)null,
                                nama = reader["nm_user"] != DBNull.Value ? reader["nm_user"].ToString() : null,
                                email = reader["email"].ToString()
                            };
                        }
                    }
                }
            }
            if (user == null)
            {
                return NotFound(); // Or handle when the user is not found
            }
            return View("Users/Edit", user);
        }

        [HttpPost]
        public IActionResult Save()
        {
            string conn_ = _configuration.GetConnectionString("conn_");

            using (MySqlConnection connection = new MySqlConnection(conn_))
            {
                connection.Open();
                var id = Request.Form["id"];
                var nama = Request.Form["nama"];
                var email = Request.Form["email"];

                string query = "";

                if (id == "")
                {
                    query = "insert into mst_user (nm_user, email) values(@namaUser, @emailUser)";
                }
                else
                {
                    query = "UPDATE mst_user set nm_user = @namaUser, email = @emailUser where id_user = @idUser";
                }

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@namaUser", nama);
                    command.Parameters.AddWithValue("@emailUser", email);
                    command.Parameters.AddWithValue("@idUser", id);

                    int rowsUpdated = command.ExecuteNonQuery();

                    if (rowsUpdated > 0)
                    {
                        TempData["SuccessMessage"] = "Berhasil Menyimpan Data!";
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            // return RedirectToAction("Users");
        }

        [HttpPost]
        public IActionResult DeleteUsers(int iD)
        {
            string conn_ = _configuration.GetConnectionString("conn_");

            using (MySqlConnection connection = new MySqlConnection(conn_))
            {
                connection.Open();

                string query = "UPDATE mst_user set deleted_status = 1 where id_user = @idUser";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idUser", iD);

                    int rowsUpdated = command.ExecuteNonQuery();

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
            }
        }
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}