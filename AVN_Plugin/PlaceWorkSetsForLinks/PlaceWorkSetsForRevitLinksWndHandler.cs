using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AVN_Plugin
{

    public class PlaceWorkSetsForRevitLinksWndHandler : IExternalEventHandler
    {
        private Document _doc;
        private JsonData _jsonData;
        private int _docHouseNumber;
        public void Execute(UIApplication app)
        {
            FilteredWorksetCollector worksets = new FilteredWorksetCollector(_doc).OfKind(WorksetKind.UserWorkset);
            FilteredElementCollector revitLinksCollector = new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType();
            FilteredElementCollector revitLinksTypeCollector = new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsElementType();

            using (Transaction tr = new Transaction(_doc))
            {
                tr.Start("Change worksets");

                if(_jsonData != null)
                {
                    foreach (RevitLinkInstance rli in revitLinksCollector)
                    {
                        SetWorkSet(rli, worksets, _jsonData.RevitLinks);
                    }

                    foreach (RevitLinkType rlt in revitLinksTypeCollector)
                    {
                        if (PlaceWorkSetsForRevitLinksViewModel.ExtractNumberFromInput(rlt.Name) == _docHouseNumber)
                        {
                            Parameter worksetParam = rlt.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                            SetWorksetById(worksetParam, worksets, _jsonData.RevitLinksTypes.First().WorksetNameContains);
                        }
                        else
                        {
                            Parameter worksetParam = rlt.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                            SetWorksetById(worksetParam, worksets, _jsonData.RevitLinksTypes.Last().WorksetNameContains);
                        }



                        //foreach (var linkData in jsonData.RevitLinksTypes)
                        //{
                        //    if (linkData.LinkNameContains != null && linkData.WorksetNameContains != null)
                        //    {
                        //        if (rlt.Name.Contains(linkData.LinkNameContains))
                        //        {
                        //            Parameter worksetParam = rlt.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                        //            SetWorksetById(worksetParam, worksets, linkData.WorksetNameContains);
                        //        }
                        //    }
                        //}
                    }
                }

                

                tr.Commit();
                MessageBox.Show("Готово");
            }
        }



        private ExternalEvent _externalEvent;
        public void Initialize()
        {
            _externalEvent = ExternalEvent.Create(this);
        }
        public void Raise(Document doc, JsonData jsonData, int docHouseNumber)
        {
            _docHouseNumber = docHouseNumber;
            _jsonData = jsonData;
            _doc = doc;
            _externalEvent.Raise();
        }
        public string GetName()
        {
            return "PlaceWorkSets";
        }

        private static void SetWorkSet(RevitLinkInstance rli, FilteredWorksetCollector worksets, List<RevitLinkData> linkDataList)
        {
            foreach (var linkData in linkDataList)
            {
                if (rli.Name.Contains(linkData.LinkNameContains))
                {
                    SetWorksetById(rli.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM), worksets, linkData.WorksetNameContains);
                }
            }
        }

        private static void SetWorksetById(Parameter worksetParam, FilteredWorksetCollector worksets, string worksetNameContains)
        {
            foreach (var ws in worksets)
            {
                if (ws.Name.Contains(worksetNameContains))
                {
                    worksetParam.Set(ws.Id.IntegerValue);
                }
            }
        }
    }
}
