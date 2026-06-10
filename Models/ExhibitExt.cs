using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Museum.Models
{
    public partial class Exhibit
    {
        public string CategoriesString => ExhibitCategories == null ? "" :
            string.Join(", ", ExhibitCategories.Select(ec => ec.CategoryFkNavigation?.Title ?? ""));

        public string MaterialsString => ExhibitMaterials == null ? "" :
            string.Join(", ", ExhibitMaterials.Select(em => em.MaterialFkNavigation?.Title ?? ""));

        public string TechniquesString => ExhibitTechniques == null ? "" :
            string.Join(", ", ExhibitTechniques.Select(et => et.TechniqueFkNavigation?.Title ?? ""));
    }
}
