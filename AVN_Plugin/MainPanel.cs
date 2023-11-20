using VCRevitRibbonUtil;
using Autodesk.Revit.UI;
using AVN_Plugin.Properties;
using AVN_Plugin;



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

                .CreateButton<ElementsId>("Ids",
                    "id элементов",
                    b => b
                        .SetLargeImage(Resources.Ids_32x32)
                        .SetLongDescription("Показывает id выбранных элементов")





                );

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
