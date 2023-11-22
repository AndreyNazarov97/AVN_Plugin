using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AVN_Plugin.LevelFiller;

namespace AVN_Plugin
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class LevelNaming : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection choices = uidoc.Selection;
            string razdelName = "AR";
            int startCounter = 1;
            
            IList<Element> selectedLevels = choices.PickElementsByRectangle(new Filters.LevelFilter());

            //Сортировка полученного списка уровней по высоте расположения уровня
            for (int i = 0; i < selectedLevels.Count; i++)
            {

                for (int j = i + 1; j < selectedLevels.Count; j++)
                {
                    if (selectedLevels[i].get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble() >
                        selectedLevels[j].get_Parameter(BuiltInParameter.LEVEL_ELEV).AsDouble())
                    {
                        (selectedLevels[i], selectedLevels[j]) = (selectedLevels[j], selectedLevels[i]);
                    }
                }
            }
            //Перебираем все элементы в выбраном списке и перезадем параметр "Имя"
            foreach (Element level in selectedLevels)
            {
                Parameter levelValue = level.LookupParameter("Фасад");
                Parameter param = level.get_Parameter(BuiltInParameter.LEVEL_ELEV);
                Parameter levelName = level.LookupParameter("Имя");



                using (Transaction newTr = new Transaction(doc, "Переименовать уровни"))
                {
                    if (startCounter == 0)
                    {
                        startCounter++;
                    }

                    newTr.Start("Переименование уровней");

                    string strCounter = string.Format("{0:d2}", startCounter);
                    string strLevelValue = string.Format("{0:f3}", levelValue.AsDouble() * 304.8 / 1000);

                    if (levelValue.AsDouble() * 304.8 / 1000 > 0)
                    {
                        levelName.Set($"{razdelName}_{strCounter}_План этажа_+{strLevelValue}");
                        //TaskDialog.Show("test", $"{strCounter}_План этажа_+{strLevelValue}");
                    }
                    else
                    {
                        levelName.Set($"{razdelName}_{strCounter}_План этажа_{strLevelValue}");
                        //TaskDialog.Show("test", $"{razdelName}_{strCounter}_План этажа_{strLevelValue}");
                    }


                    startCounter++;

                    newTr.Commit();
                }
            }

            return Result.Succeeded;
        }
        
    }
}

