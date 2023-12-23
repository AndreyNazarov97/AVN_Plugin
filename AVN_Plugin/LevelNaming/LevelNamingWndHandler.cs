using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AVN_Plugin
{
    public class LevelNamingWndHandler : IExternalEventHandler
    {
        private IList<Element> _selectedLevels;
        private int _startCount;
        private string _razdelName;
        public void Execute(UIApplication app)
        {
            if (_selectedLevels != null)
            {
                using (Transaction newTr = new Transaction(_selectedLevels.First().Document, "Переименовать уровни"))
                {
                    newTr.Start("Переименование уровней");


                    foreach (Element level in _selectedLevels)
                    { 
                    Parameter levelValue = level.LookupParameter("Фасад");
                    Parameter param = level.get_Parameter(BuiltInParameter.LEVEL_ELEV);
                    Parameter levelName = level.LookupParameter("Имя");


                        if (_startCount == 0)
                        {
                            _startCount++;
                        }

                        

                        string strCounter = string.Format("{0:d2}", _startCount);
                        string strLevelValue = string.Format("{0:f3}", levelValue.AsDouble() * 304.8 / 1000);

                        if (levelValue.AsDouble() * 304.8 / 1000 > 0)
                        {
                            levelName.Set($"{_razdelName}_{strCounter}_План этажа_+{strLevelValue}");
                            //TaskDialog.Show("test", $"{strCounter}_План этажа_+{strLevelValue}");
                        }
                        else
                        {
                            levelName.Set($"{_razdelName}_{strCounter}_План этажа_{strLevelValue}");
                            //TaskDialog.Show("test", $"{razdelName}_{strCounter}_План этажа_{strLevelValue}");
                        }
                        _startCount++;
                    }
                    newTr.Commit();
                }
            }
        }

        private ExternalEvent _externalEvent;
        public void Initialize()
        {
            _externalEvent = ExternalEvent.Create(this);
        }

        public void Raise(IList<Element> selectedLevels, int startCount, string razdelName)
        {
            _selectedLevels = selectedLevels;
            _startCount = startCount;
            _razdelName = razdelName;
            _externalEvent.Raise();
        }

        public string GetName()
        {
            return "SetLevelNames";
        }
    }
}
