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


namespace todolist
{
    public class DbContext
    {
        private readonly IConfiguration _configuration;

        public DbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnection()
        {
            return _configuration.GetConnectionString("conn_");
        }
    }
}
