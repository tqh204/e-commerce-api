using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Role
    {
        public int roleId { get; set; }
        public string RoleName { get; set; }

        //public ICollection<User> Users { get; set; } = new List<User>();
    }
}
