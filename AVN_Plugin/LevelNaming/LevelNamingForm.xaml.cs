﻿using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AVN_Plugin.LevelFilter
{
    /// <summary>
    /// Логика взаимодействия для LevelFillerForm.xaml
    /// </summary>
    public partial class LevelFillerForm : Window
    {
        

        public LevelFillerForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LevelNaming levelNaming = new LevelNaming();

            levelNaming

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbDisciplineName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
