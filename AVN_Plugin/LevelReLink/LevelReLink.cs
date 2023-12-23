using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace AVN_Plugin
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    internal class LevelReLink : IExternalCommand
    {
        public Helpers _help = new Helpers();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            // Создание правила фильтрации для выбора "плохих" уровней
            ParameterValueProvider provider = new ParameterValueProvider(new ElementId(BuiltInParameter.ELEM_PARTITION_PARAM));
            FilterStringRuleEvaluator evaluator = new FilterStringContains();
            FilterStringRule rule = new FilterStringRule(provider, evaluator, "01", false);
            
            //инвертируем правило фильтрации
            FilterInverseRule notEqualsRule = new FilterInverseRule(rule);

            // Создание фильтра по параметру
            ElementParameterFilter wrongLevelsFilter = new ElementParameterFilter(notEqualsRule);

            // Применение фильтра к FilteredElementCollector
            FilteredElementCollector wrongLevelsCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WherePasses(wrongLevelsFilter)
                .WhereElementIsNotElementType();

            //foreach (var level in wrongLevelsCollector)
            //{
            //    TaskDialog.Show("Wrong Level Found", $"Level Id: {level.Id}, Level Name: {level.Name}");
            //}

            List<ElementId> wrongLevelsIds = new List<ElementId>();
            foreach (Element elem in wrongLevelsCollector)
            {
                wrongLevelsIds.Add(elem.Id);
            }

            //Здесь мы ищем вид, на котором будем искать элементы, возможно это будет не нужно
            FilteredElementCollector viewsCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views);
            View myView = viewsCollector.FirstOrDefault(l => l.Name.Contains("Navisworks")) as View;

            //Здесь мы получаем перекрытия в неправильных уровнях
            IList<Element> floorsInView = new FilteredElementCollector(doc, myView.Id).OfCategory(BuiltInCategory.OST_Floors).Where(x => wrongLevelsIds.Contains(x.LevelId)).ToList();
            IList<Element> wallsInView = new FilteredElementCollector(doc, myView.Id).OfCategory(BuiltInCategory.OST_Walls).Where(x => wrongLevelsIds.Contains(x.LevelId)).ToList();
            IList<Element> doorsInView = new FilteredElementCollector(doc, myView.Id).OfCategory(BuiltInCategory.OST_Doors).Where(x => wrongLevelsIds.Contains(x.LevelId)).ToList();
            IList<Element> windowsInView = new FilteredElementCollector(doc, myView.Id).OfCategory(BuiltInCategory.OST_Windows).Where(x => wrongLevelsIds.Contains(x.LevelId)).ToList();
            IList<Element> doorsAndWindows = doorsInView.Concat(windowsInView).ToList();

            IList<Element> allElemsInActiveViewWithRBS_START_LEVEL_PARAM = _help.AllVisibleElementsInViewWithRBS_START_LEVEL_PARAM(doc, myView).Where(x => wrongLevelsIds.Contains(x.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).AsElementId())).ToList();
            IList<Element> allElemsInActiveViewWithWithFAMILY_LEVEL_PARAM = _help.AllVisibleElementsInViewWithFAMILY_LEVEL_PARAM(doc, myView).Where(x => wrongLevelsIds.Contains(x.LevelId)).ToList();

            //_help.PrintElements(allElemsInActiveViewWithWithFAMILY_LEVEL_PARAM);


            //здесь перебираю элементы по категориям
            using (Transaction newTr = new Transaction(doc, "Задать new level"))
            {
                newTr.Start();

                ////перебиваем уровень во всех перекрытиях
                //foreach (Element floor in floorsInView)
                //{
                //    //Здесь надо запилить функцию поиска ближайшего хорошего уровня или пока что по первому уровню привязывать
                //    ElementId neareastLevelId = new ElementId(2607);

                //    Level nearestLevel = (Level)doc.GetElement(neareastLevelId);
                //    Level curLevel = (Level)doc.GetElement(floor.LevelId);

                //    var levelsdif = GetDiffB2Floors(nearestLevel, curLevel);
                //    var heigtAboveParam = floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble();

                //    floor.get_Parameter(BuiltInParameter.LEVEL_PARAM).Set(neareastLevelId);
                //    floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(levelsdif + heigtAboveParam);
                //}

                //foreach (Element elem in doorsAndWindows)
                //{
                //    //Здесь надо запилить функцию поиска ближайшего хорошего уровня или пока что по первому уровню привязывать
                //    ElementId neareastLevelId = new ElementId(2607);

                //    Level nearestLevel = (Level)doc.GetElement(neareastLevelId);
                //    Level curLevel = (Level)doc.GetElement(elem.LevelId);

                //    var levelsdif = GetDiffB2Floors(nearestLevel, curLevel);
                //    var heigtAboveParam = elem.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble();

                //    elem.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(neareastLevelId);
                //    elem.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(levelsdif + heigtAboveParam);
                //}

                ReLinkElementsWithRBS_START_LEVEL_PARAM(allElemsInActiveViewWithRBS_START_LEVEL_PARAM);
                ReLinkElementsWithFAMILY_LEVEL_PARAM(allElemsInActiveViewWithWithFAMILY_LEVEL_PARAM);
                ReLinkWalls(wallsInView);
                
                

                

                newTr.Commit();
            }

            return Result.Succeeded;
        }


        //Функция возвращает разницу между двумя уровнями
        public double GetDiffB2Floors(Level level1, Level level2)
        {
            var elevlvl1 = level1.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
            var elevlvl2 = level2.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();

            return elevlvl2 - elevlvl1;
        }
        public double GetDiffB2Walls(Level level1, Level level2)
        {
            double elevlvl1 = level1.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();
            double elevlvl2 = level2.get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble();

            if (level1 == null) elevlvl1=0;
            if (level2 == null) elevlvl2=0;
            
            return elevlvl2 - elevlvl1;
        }

        public void ReLinkElementsWithRBS_START_LEVEL_PARAM(IList<Element> elements)
        {
            foreach (Element elem in elements)
            {
                //Здесь надо запилить функцию поиска ближайшего хорошего уровня или пока что по первому уровню привязывать
                ElementId neareastLevelId = new ElementId(2607);

                elem.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).Set(neareastLevelId);
            }
        }
        public void ReLinkElementsWithFAMILY_LEVEL_PARAM(IList<Element> elements)
        {
            foreach (Element elem in elements)
            {
                //Здесь надо запилить функцию поиска ближайшего хорошего уровня или пока что по первому уровню привязывать
                ElementId neareastLevelId = new ElementId(2607);

                if (elem.Category.Name.Contains("Двер") | elem.Category.Name.Contains("Окн"))
                {
                    Level nearestLevel = (Level)elem.Document.GetElement(neareastLevelId);
                    Level curLevel = (Level)elem.Document.GetElement(elem.LevelId);
                    var levelsdif = GetDiffB2Floors(nearestLevel, curLevel);

                    var heigtAboveParam = elem.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
                    elem.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(neareastLevelId);
                    elem.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(levelsdif + heigtAboveParam);

                }
                else if (elem.Category.Name.Contains("Соединительные детали"))
                {
                    Level nearestLevel = (Level)elem.Document.GetElement(neareastLevelId);
                    Level curLevel = (Level)elem.Document.GetElement(elem.LevelId);
                    var levelsdif = GetDiffB2Floors(nearestLevel, curLevel);

                    var heigtAboveParam = elem.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
                    elem.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(neareastLevelId);
                    elem.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(levelsdif + heigtAboveParam);
                }

            }
        }

        public void ReLinkWalls(IList<Element> elements)
        {
            foreach (Element wall in elements)
            {
                //Здесь надо запилить функцию поиска ближайшего хорошего уровня или пока что по первому уровню привязывать
                ElementId neareastLevelId = new ElementId(2607);

                Level nearestBaseLevel = (Level)wall.Document.GetElement(neareastLevelId);
                Level curBaseLevel = (Level)wall.Document.GetElement(wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId());

                Level nearestHeightLevel = (Level)wall.Document.GetElement(neareastLevelId);
                Level curHeightLevel = (Level)wall.Document.GetElement(wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId());


                var levelsBasedif = GetDiffB2Walls(nearestBaseLevel, curBaseLevel);
                var heigtBaseAboveParam = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();


                wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(neareastLevelId);
                wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(levelsBasedif + heigtBaseAboveParam);


                if (curHeightLevel != null)
                {
                    var levelsHeightdif = GetDiffB2Walls(nearestHeightLevel, curHeightLevel);
                    var heigtHeightAboveParam = wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).AsDouble();

                    wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(neareastLevelId);
                    wall.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(levelsHeightdif + heigtHeightAboveParam);
                }
            }

        }
    }

}
