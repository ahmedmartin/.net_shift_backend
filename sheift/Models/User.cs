using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sheift.Models
{
    public partial class User
    {
        public string UserName { get; set; } = null!;

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        public int DeptId { get; set; }
        public string Telephone { get; set; } = null!;
        public string? EntryTelephone { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string Password { get; set; } = null!;
    }
}
