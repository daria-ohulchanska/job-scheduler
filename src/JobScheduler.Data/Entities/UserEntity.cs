using System.ComponentModel.DataAnnotations;

namespace JobScheduler.Data.Entities
{
    public class UserEntity : Entity
    {
        [Required]
        [MaxLength(20)]
        public string Username { get; set; }
        [Required]
        [MaxLength(20)]
        public string Password { get; set; }
    }
}
