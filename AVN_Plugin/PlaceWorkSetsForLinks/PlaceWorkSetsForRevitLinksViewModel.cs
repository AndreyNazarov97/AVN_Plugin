using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AVN_Plugin;
using AVN_Plugin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;


namespace AVN_Plugin
{

    public class PlaceWorkSetsForRevitLinksViewModel : INotifyPropertyChanged
    {
        private UIDocument _uidoc;
        private Window _window;
        private PlaceWorkSetsForRevitLinksWndHandler _handler;

        public PlaceWorkSetsForRevitLinksViewModel(UIDocument uidoc, Window window, PlaceWorkSetsForRevitLinksWndHandler handler)
        {
            _uidoc = uidoc;
            _window = window;
            _handler = handler;
        }

        private int docHouseNumber;

        public int DocHouseNumber
        {
            get { return docHouseNumber; }
            set
            {
                docHouseNumber = value;
                OnPropertyChanged(nameof(docHouseNumber));
            }
        }
        private string jsonFilePath = @"G:\Общие диски\BIM-coordinator\Скрипты\Рабочие наборы связей\PlaceWorkSetsConfig.json";

        public string JsonFilePath
        {
            get { return jsonFilePath; }
            set
            {
                jsonFilePath = value;
                OnPropertyChanged(nameof(jsonFilePath));
            }
        }



        public RelayCommand PlaceWorksetsCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Document doc = _uidoc.Document;
                    try
                    {
                        string jsonContent = File.ReadAllText(JsonFilePath);
                        JsonData jsonData = JsonConvert.DeserializeObject<JsonData>(jsonContent);
                        DocHouseNumber = ExtractNumberFromInput(doc.Title);


                        _handler?.Raise(doc, jsonData, DocHouseNumber);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public static int ExtractNumberFromInput(string input)
        {
            // Используем регулярное выражение для поиска числа между подчёркиваниями
            Match match = Regex.Match(input, @"_(\d+)_");

            // Проверяем, найдено ли число
            if (match.Success)
            {
                // Пытаемся преобразовать найденное число в int
                if (int.TryParse(match.Groups[1].Value, out int result))
                {
                    return result;
                }
            }

            // Возвращаем -1 в случае ошибки или отсутствия числа
            return -1;
        }
    }
}

