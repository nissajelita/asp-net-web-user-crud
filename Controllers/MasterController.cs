// using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using todolist.Models;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;



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
                string query = "SELECT * FROM mst_user";
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
            return View(users);
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