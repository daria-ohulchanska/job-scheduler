using Microsoft.AspNetCore.Identity;

namespace JobScheduler.Data.Entities
{
    public class UserEntity : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
