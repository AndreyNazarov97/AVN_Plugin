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
            

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<Element> familys = collector.OfClass(typeof(Family)).ToElements();

            //string categoryName = "Соединительные детали кабельных лотков";
            string categoryName = "Соединительные детали воздуховодов";
            string prefixName = "Дв";

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

                // Вывод списка имён семейств и их изменение
                foreach (ElementId id in familyIds)
                {

                    Element element = doc.GetElement(id);
                    int index = element.Name.IndexOf("_");

                    //Обрезаем имя семейство до первого символа "_" и добавляем префикс
                    if(index != -1)
                    {
                        element.Name = prefixName + "_" + element.Name.Substring(index + 1);
                    }
                    else
                    {
                        element.Name = prefixName + "_" + element.Name;
                    }
                 
                    names = names + "\n" + element.Name;
                }

                tr.Commit();
            }
            
            TaskDialog.Show("Список имён семейств:", names);

            return Result.Succeeded;
        }
    }
}
