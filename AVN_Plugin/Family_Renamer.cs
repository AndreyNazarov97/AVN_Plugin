using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVN_Plugin
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class Family_Renamer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection choices = uidoc.Selection;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> familys = collector.OfClass(typeof(Family)).ToElements();

            string categoryName = "Соединительные детали кабельных лотков";

            // Поиск семейств по имени категории
            List<ElementId> familyIds = new List<ElementId>();
            foreach (Element element in familys)
            {
                Family family = (Family)element;
                if (family.FamilyCategory.Name == categoryName)
                {
                    familyIds.Add(family.Id);
                }
            }

            string names = "";

            using (Transaction tr = new Transaction(doc, "Переименовывание семейств"))
            {
                tr.Start();

                foreach (ElementId id in familyIds)
                {

                    Element element = doc.GetElement(id);

                    element.Name = "Дкл_" + element.Name;

                    names = names + "\n" + element.Name;
                }

                tr.Commit();
            }
            
            TaskDialog.Show("Список имён семейств:", names);

            return Result.Succeeded;
        }
    }
}
