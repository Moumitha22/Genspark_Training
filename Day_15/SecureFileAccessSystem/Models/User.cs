using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureFileAccessSystem.Models
{
    public class User
    {
        public string UserName { get; set; }
        public Role Role { get; set; }

        public User(string userName, Role role)
        {
            this.UserName = userName;
            this.Role = role;
        }
    }
}
