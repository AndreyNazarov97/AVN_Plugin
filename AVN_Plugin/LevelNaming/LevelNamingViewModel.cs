using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AVN_Plugin.LevelFiller;
using AVN_Plugin.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Media3D;

namespace AVN_Plugin
{
    public class LevelNamingViewModel : INotifyPropertyChanged
    {
        private UIDocument _uidoc;
        private Window _window;
        private LevelNamingWndHandler _handler;

        public LevelNamingViewModel(UIDocument uidoc, Window window, LevelNamingWndHandler handler)
        {
            _uidoc = uidoc;
            _window = window;
            _handler = handler;
        }

        private IList<Element> selectedLevels;

        public IList<Element> SelectedLevels
        {
            get { return selectedLevels; }
            set 
            { 
                selectedLevels = value;
                OnPropertyChanged(nameof(selectedLevels));
            }
        }

        private int startCount = 1;

        public int StartCount
        {
            get { return startCount; }
            set { 
                startCount = value;
                OnPropertyChanged(nameof(startCount));
            }
        }

        private string razdelName = "MEP";

        public string RazdelName
        {
            get { return razdelName; }
            set { 
                razdelName = value; 
                OnPropertyChanged(nameof(razdelName));
            }
        }

        public RelayCommand SelectLevelsCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Document doc = _uidoc.Document;
                    Selection choices = _uidoc.Selection;
                    SelectedLevels = choices.PickElementsByRectangle(new Filters.LevelFilter());

                    LevelsSort(SelectedLevels);

                    _window.WindowState = WindowState.Normal;
                    
                    _handler?.Raise(SelectedLevels, StartCount, RazdelName);

                    
                });
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        private void LevelsSort(IList<Element> selectedLevels)
        {
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
        }
    }
}
