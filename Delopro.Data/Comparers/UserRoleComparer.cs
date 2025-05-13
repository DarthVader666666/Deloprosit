using Delopro.Data.Entities;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Delopro.Data.Comparers
{
    public class UserRoleComparer: IEqualityComparer<UserRole?>
    {
        public bool Equals(UserRole? x, UserRole? y)
        {
            var r = x != null && y != null && x.UserId == y.UserId && x.RoleId == y.RoleId;
            return r;
        }

        public int GetHashCode([DisallowNull] UserRole? obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
