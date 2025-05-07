using System.ComponentModel;

namespace Delopro.Data.Enums
{
    public enum UserRoleType
    {
        [Description("Owner")]
        Owner = 1,
        [Description("Admin")]
        Admin = 2,
        [Description("User")]
        User = 3
    }
}
