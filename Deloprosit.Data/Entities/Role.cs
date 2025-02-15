﻿namespace Deloprosit.Data.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public virtual ICollection<AccountRole>? UserRoles { get; set; }
    }
}
