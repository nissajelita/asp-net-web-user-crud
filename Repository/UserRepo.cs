// using System;
// using System.Collections.Generic;
// using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using todolist.Models;
using todolist.Helpers;
using Dapper;

namespace todolist.Repository
{
    public class UserRepo
    {
        // private readonly string _connectionString;

        // public UserRepo(string connectionString)
        // {
        //     _connectionString = connectionString;
        // }

        private DataContext _context;

        public UserRepo(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            using var conn = _context.CreateConnection();
            string query = "SELECT * FROM mst_user where deleted_status is null";
            return await conn.QueryAsync<UserModel>(query);

        }
        public async Task<UserModel> GetUsersByUname(string username)
        {
            // Initialize with a default or empty object
            using var conn = _context.CreateConnection();
            string query = "SELECT * FROM mst_user WHERE username = @username";
            var users = await conn.QuerySingleOrDefaultAsync<UserModel>(query, new { username });
            return users;


        }


        public async Task<UserModel> EditUsers(int iD)
        {
            using var conn = _context.CreateConnection();
            string query = "SELECT * FROM mst_user where id_user = @iD";
            var users = await conn.QueryFirstAsync<UserModel>(query, new { iD });
            return users;
        }

        public async Task Save(UserModel obj)
        {
            using var conn = _context.CreateConnection();
            string query = "";

            if (obj.id_user == 0)
            {
                query = "INSERT INTO mst_user (nm_user, email, username, password) VALUES (@namaUser, @emailUser, @username, @password)";
            }
            else
            {
                if (!string.IsNullOrEmpty(obj.password))
                {
                    query = "UPDATE mst_user SET nm_user = @namaUser, email = @emailUser, password = @password WHERE id_user = @idUser";
                }
                else
                {
                    query = "UPDATE mst_user SET nm_user = @namaUser, email = @emailUser WHERE id_user = @idUser";
                }
            }

            await conn.ExecuteAsync(query, new { namaUser = obj.nm_user, emailUser = obj.email, username = obj.username, password = obj.password, idUser = obj.id_user });
        }

        //     public int SaveSession(UserLogin obj)
        //     {
        //         using (MySqlConnection connection = new MySqlConnection(_connectionString))
        //         {
        //             connection.Open();

        //             string query = "";

        //             query = "insert into login_info (id_user, login_time) values(@userId, @loginTime)";


        //             using (MySqlCommand command = new MySqlCommand(query, connection))
        //             {
        //                 command.Parameters.AddWithValue("@userId", obj.id_user);
        //                 command.Parameters.AddWithValue("@loginTime", obj.login_time);

        //                 int results = command.ExecuteNonQuery();

        //                 return results;
        //             }
        //         }

        //     }

        public async Task<dynamic> Delete(int iD)
        {
            using var conn = _context.CreateConnection();
            string query = "UPDATE mst_user set deleted_status = 1 where id_user = @idUser";
            int rowsUpdated = await conn.ExecuteAsync(query, new { idUser = iD });

            return rowsUpdated;
        }
    }

}
