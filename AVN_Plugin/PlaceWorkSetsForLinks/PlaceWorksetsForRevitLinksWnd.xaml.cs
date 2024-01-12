using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AVN_Plugin.PlaceWorkSetsForLinks
{
    /// <summary>
    /// Логика взаимодействия для PlaceWorksetsForRevitLinksWnd.xaml
    /// </summary>
    public partial class PlaceWorksetsForRevitLinksWnd : Window
    {
        public PlaceWorksetsForRevitLinksWnd()
        {
            InitializeComponent();
        }
        private void OpenFileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Json (*.json*)|*.json*"; // Установите необходимые фильтры для типов файлов



            openFileDialog.ShowDialog();

            PlaceWorkSetsForRevitLinksViewModel viewModel = (PlaceWorkSetsForRevitLinksViewModel)DataContext;
            viewModel.JsonFilePath = openFileDialog.FileName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
