using System.Text.Json.Serialization;

namespace Entities.Models.UserModel
{
    public class UserLoginModel
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string? UserMailAdress { get; set; }
        public string? Password { get; set; }
    }
}
