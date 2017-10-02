using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace SubstationChecker
{
    public partial class MainWindow : Window
    {
        static SerialPort arduinoPort;
        Thread arduinoTread;

        private string phone;
        public string arduinoPortNumber;
        public string modemPortNumber;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            phone = tbNumPhone.Text;
            InitializeArduinoPort();
            arduinoTread = new Thread(GetDataFromArduino);
            arduinoTread.Start();
        }

        private void GetDataFromArduino()
        {
            while (true)
            {
                string messageFromArduino = arduinoPort.ReadExisting(); // get data from arduino port
                Thread.Sleep(500);

                if (messageFromArduino.Contains("zero"))
                    CreateSms("0"); 
                if (messageFromArduino.Contains("one"))
                    CreateSms("1");
                if (messageFromArduino.Contains("two"))
                    CreateSms("2");
                if (messageFromArduino.Contains("three"))
                    CreateSms("3");
                if (messageFromArduino.Contains("four"))
                    CreateSms("4");
                if (messageFromArduino.Contains("five"))
                    CreateSms("5");
                if (messageFromArduino.Contains("six"))
                    CreateSms("6");
                if (messageFromArduino.Contains("seven"))
                    CreateSms("7");
            }
        }

        private void CreateSms(string messageToEmployee)
        {
            SmsSender sms = new SmsSender(messageToEmployee, phone, modemPortNumber);
            sms.SendSms();
        }
        private void InitializeArduinoPort()
        {
            arduinoPort = new SerialPort();

            arduinoPort.BaudRate = 9600;
            arduinoPort.DtrEnable = true;

            arduinoPort.ReadTimeout = 1000;
            arduinoPort.PortName = "COM" + arduinoPortNumber;

            if (arduinoPort.IsOpen)
                arduinoPort.Close();

            try
            {
                arduinoPort.Open();
            }
            catch
            {
                arduinoPort.Close();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }
    }
}
