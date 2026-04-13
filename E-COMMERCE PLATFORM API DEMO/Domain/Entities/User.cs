using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
        public int roleId { get; set; }//Foreign key
        public Roles role { get; set; }
        public DateTime createAt { get; set; } = DateTime.UtcNow;
    }
}
