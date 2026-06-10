using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Museum.Models
{
    public partial class Person
    {
        public string PersonTypeName => PersonTypeFkNavigation?.TypeName ?? "";
        public string RolesString => PersonRoles == null ? "" :
            string.Join(", ", PersonRoles.Select(pr => pr.RoleFkNavigation?.Title ?? ""));
    }
}
