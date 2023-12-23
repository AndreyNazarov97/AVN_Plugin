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
using AVN_Plugin.LevelFilter;

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

            LevelNamingWndHandler handler = new LevelNamingWndHandler();
            handler.Initialize();

            LevelNamingWnd wnd = new LevelNamingWnd();
            LevelNamingViewModel vm = new LevelNamingViewModel(uidoc, wnd, handler);

            wnd.DataContext = vm;
            wnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            wnd.Show();

            return Result.Succeeded;
        }
        
    }
}

