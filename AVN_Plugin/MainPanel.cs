using VCRevitRibbonUtil;
using Autodesk.Revit.UI;
using AVN_Plugin.Properties;




namespace AVN_Plugin
{
    internal class MainPanel : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            Ribbon.GetApplicationRibbon(a)
                .Tab("AVN_Plugin")
                .Panel("LVL")

                .CreateButton<LevelNaming>("LVL",
                    "Заполнение уровней",
                    b => b
                        .SetLargeImage(Resources.Logo_LVL_32)
                        .SetSmallImage(Resources.Logo_LVL_16)
                        .SetLongDescription("Заполняет уровни в соответствии со стандартом GENPRO"))
                .CreateSeparator()

                .CreateButton<LevelReLink>("LvlReLink",
                    "перепривязка уровней",
                    b => b
                        .SetLargeImage(Resources.Logo_LVL_32)
                        .SetSmallImage(Resources.Logo_LVL_16)
                        .SetLongDescription("отвязывает элементы от уровней, не входящих в РН \"01_Уровнии и Оси\""))

                .CreateSeparator()
                .CreateButton<PlaceWorkSets>("PlaceWorkSets",
                    "Cвязи по РН",
                    b => b
                        .SetLargeImage(Resources.WS)
                        .SetLongDescription("Переносит связи по Рабочим наборам")

                
                
                 






                );

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        
    }

}
