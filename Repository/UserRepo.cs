using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using todolist.Models;

namespace todolist.Repository
{
    public class UserRepo
    {
        private readonly string _connectionString;

        public UserRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<UserModel> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
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

            return users;
        }

        public UserModel EditUsers(int iD)
        {
            UserModel users = new UserModel();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
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
                            users = new UserModel
                            {
                                id = reader["id_user"] != DBNull.Value ? (int)reader["id_user"] : (int?)null,
                                nama = reader["nm_user"] != DBNull.Value ? reader["nm_user"].ToString() : null,
                                email = reader["email"].ToString()
                            };
                        }
                    }
                }
            }
            return users;
        }

        public int Save(UserModel obj)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "";

                if (obj.id == null || obj.id == 0)
                {
                    query = "insert into mst_user (nm_user, email) values(@namaUser, @emailUser)";
                }
                else
                {
                    query = "UPDATE mst_user set nm_user = @namaUser, email = @emailUser where id_user = @idUser";
                }

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@namaUser", obj.nama);
                    command.Parameters.AddWithValue("@emailUser", obj.email);
                    command.Parameters.AddWithValue("@idUser", obj.id ?? 0);

                    int rowsUpdated = command.ExecuteNonQuery();

                    return rowsUpdated;
                }
            }

        }

        public int Delete(int iD)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "UPDATE mst_user set deleted_status = 1 where id_user = @idUser";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idUser", iD);
                    int rowsUpdated = command.ExecuteNonQuery();
                    return rowsUpdated;
                }
            }
        }





    }

}
