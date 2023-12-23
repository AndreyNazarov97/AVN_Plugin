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
    internal class Helpers
    {
        public double feetToMillimeters(double feet)
        {
            return feet * 304.8;
        }

        public double millimeterToFeet(double millimeters)
        {
            return millimeters / 304.8;
        }

        public IList<Element> AllVisibleElementsInView(Document doc, View view)
        {
            // Получаем коллекцию видимых элементов, не являющихся типами элементов
            var visibleElements = new FilteredElementCollector(doc, view.Id)
                .WhereElementIsNotElementType()
                .Where(x => !x.IsHidden(view)).ToList();

            return visibleElements;
        }


        public IList<Element> AllVisibleElementsInViewWithLevelId(Document doc, View view)
        {

            var visibleElements = AllVisibleElementsInView(doc, view);

            // Определяем ID уровня (замените на фактический ID вашего уровня)
            ElementId levelId = new ElementId(-1);

            // Фильтруем элементы по уровню
            var filteredElements = visibleElements
                .Where(x => x.LevelId != levelId)
                .ToList();

            return filteredElements;
        }
        public IList<Element> AllVisibleElementsInViewWithRBS_START_LEVEL_PARAM(Document doc, View view)
        {
            var visibleElements = AllVisibleElementsInView(doc, view);

            // Фильтруем элементы по уровню
            var filteredElements = visibleElements
                .Where(x => x.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM) != null)
                .ToList();

            return filteredElements;

        }

        public IList<Element> AllVisibleElementsInViewWithFAMILY_LEVEL_PARAM(Document doc, View view)
        {
            var visibleElements = AllVisibleElementsInView(doc, view);

            // Фильтруем элементы по уровню
            var filteredElements = visibleElements
                .Where(x => x.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null)
                .ToList();

            return filteredElements;

        }


        

        public void PrintElements(IList<Element> elements)
        {

            var elementsNameAndTypeStrs = elements.Select(e => $"Id:{e.Id} Name:{e.Name} TypeName:{e.GetType().Name}");


            string elementsNameAndType = string.Join("\n", elementsNameAndTypeStrs);

            TaskDialog.Show("Список выбранных элементов", elementsNameAndType);

        }
    }
}

   
