namespace todolist.Models
{
    public class UserModel
    {
        public int? id_user { get; set; }
        public string? nm_user { get; set; }
        public string? email { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }

    }

    public class UserLogin
    {
        public int? id_user { get; set; }
        public DateTime? login_time { get; set; }
        public DateTime? logout_time { get; set; }
    }
}