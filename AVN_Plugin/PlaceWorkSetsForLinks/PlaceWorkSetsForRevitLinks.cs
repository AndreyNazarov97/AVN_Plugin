using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AVN_Plugin.PlaceWorkSetsForLinks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AVN_Plugin
{
    public class JsonData
    {
        public List<RevitLinkData> RevitLinks { get; set; }
        public List<RevitLinkData> RevitLinksTypes { get; set; }
    }

    public class RevitLinkData
    {
        public string LinkNameContains { get; set; }
        public string WorksetNameContains { get; set; }
        public string CurrentHouse { get; set; }
    }

    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]
    public class PlaceWorkSets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            PlaceWorkSetsForRevitLinksWndHandler handler = new PlaceWorkSetsForRevitLinksWndHandler();
            handler.Initialize();

            PlaceWorksetsForRevitLinksWnd wnd = new PlaceWorksetsForRevitLinksWnd();
            PlaceWorkSetsForRevitLinksViewModel vm = new PlaceWorkSetsForRevitLinksViewModel(uidoc, wnd, handler);

            wnd.DataContext = vm;
            wnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            wnd.Show();


            return Result.Succeeded;
        }

    }
}

