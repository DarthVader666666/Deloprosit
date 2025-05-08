using System.ComponentModel;

namespace Delopro.Server.Enums
{
    public enum UserStatus
    {
        [Description("Подтвержден")]
        Confirmed,
        [Description("Не подтвержден")]
        NotConfirmed,
        [Description("Удален")]
        Deleted
    }
}
