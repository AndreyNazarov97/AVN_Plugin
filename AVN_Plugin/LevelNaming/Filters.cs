using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVN_Plugin.LevelFiller
{
    public class Filters
    {
        public class LevelFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {
                if ((BuiltInCategory)elem.Category.Id.IntegerValue == BuiltInCategory.OST_Levels)
                {
                    return true;
                }
                return false;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }
    }
}
