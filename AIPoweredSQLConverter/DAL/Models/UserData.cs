using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIPoweredSQLConverter.DAL.Models
{
    public class UserData
    {
        [Key]
        public required string Username { get; set; }

        public string? EncryptedApiKey { get; set; }

        [MaxLength(4000)]
        public string? UserSQLData { get; set; }
    }
}
