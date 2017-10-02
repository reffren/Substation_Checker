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
using System.Windows.Shapes;

namespace SubstationChecker
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //string portArduino = tbArduinoPort.Text;
            //string portModem = tbModemPort.Text;
           
            ((MainWindow)Application.Current.MainWindow).arduinoPortNumber = tbArduinoPort.Text;
            ((MainWindow)Application.Current.MainWindow).modemPortNumber = tbModemPort.Text;
            this.Close();
        }
    }
}
