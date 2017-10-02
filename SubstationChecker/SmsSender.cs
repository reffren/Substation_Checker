using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubstationChecker
{
    class SmsSender
    {
        SerialPort modemPort;
        Thread smsThread;

        private string smsMessage;
        private string phoneNumber;
        private string comPort;

        public SmsSender(string message, string phone, string port)
        {
            smsMessage = message;
            phoneNumber = phone;
            comPort = port;
        }

        public void SendSms()
        {
            smsThread = new Thread(Send);
            smsThread.Start();
        }

        private void InitializeModemPort()
        {
            modemPort = new SerialPort();

            modemPort.BaudRate = 2400;
            modemPort.DataBits = 7;

            modemPort.StopBits = StopBits.One;
            modemPort.Parity = Parity.Odd;

            modemPort.ReadTimeout = 500;
            modemPort.WriteTimeout = 500;

            modemPort.Encoding = Encoding.GetEncoding("windows-1251");
            modemPort.PortName = "COM" + comPort;

            if (modemPort.IsOpen)
                modemPort.Close();

            try
            {
                modemPort.Open();
            }
            catch
            {
                modemPort.Close();
            }
        }

        private void Send()
        {
            try
            {
                System.Threading.Thread.Sleep(500);
                modemPort.WriteLine("AT\r\n"); // start for modem 
                System.Threading.Thread.Sleep(500);

                modemPort.Write("AT+CMGF=0\r\n"); // PDU format
                System.Threading.Thread.Sleep(500);
            }
            catch { }

            try
            {
                phoneNumber = phoneNumber.Replace("-", "").Replace(" ", "").Replace("+", ""); // remove dashes, gaps and pluses from the number 
                phoneNumber = "01" + "00" + phoneNumber.Length.ToString("X2") + "91" + EncodePhoneNumber(phoneNumber); //always nulls + length of number +  international format + encode phone number
                smsMessage = StringToUCS2(smsMessage); // encode message to UCS2 format

                string leninByte = (smsMessage.Length / 2).ToString("X2"); //get the length of message
                smsMessage = phoneNumber + "00" + "08" + leninByte + smsMessage; // pid + DCS + length of message + message itself

                double lenMes = smsMessage.Length / 2; // get the number of octets
                modemPort.Write("AT+CMGS=" + (Math.Ceiling(lenMes)).ToString() + "\r\n"); //send number of octets
                System.Threading.Thread.Sleep(500);

                smsMessage = "00" + smsMessage;

                modemPort.Write(smsMessage + char.ConvertFromUtf32(26) + "\r\n"); // send sms
                System.Threading.Thread.Sleep(15000);

                modemPort.Write("AT+CMGD=1,4 \r\n"); // delete all messages
                System.Threading.Thread.Sleep(500);

            }
            catch
            {
                modemPort.Close();
            }
        }


        // encoding sms text to UCS2
        public static string StringToUCS2(string str)
        {
            UnicodeEncoding ue = new UnicodeEncoding();
            byte[] ucs2 = ue.GetBytes(str);

            int i = 0;
            while (i < ucs2.Length)
            {
                byte b = ucs2[i + 1];
                ucs2[i + 1] = ucs2[i];
                ucs2[i] = b;
                i += 2;
            }
            return BitConverter.ToString(ucs2).Replace("-", "");
        }

        // encoding phone number to PDU format
        private static string EncodePhoneNumber(string phoneNumber)
        {
            string searchPhone = "8";
            int existNum = phoneNumber.IndexOf(searchPhone); //text before +7
            if (existNum != -1)
            {
                string editedNum = phoneNumber.Remove(existNum, 1);
                phoneNumber = "7" + editedNum;
            }
            string result = "";
            if ((phoneNumber.Length % 2) > 0) phoneNumber += "F";

            int i = 0;
            while (i < phoneNumber.Length)
            {
                result += phoneNumber[i + 1].ToString() + phoneNumber[i].ToString();
                i += 2;
            }
            return result.Trim();
        }
    }
}
