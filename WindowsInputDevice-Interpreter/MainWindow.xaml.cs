using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace WindowsInputDevice_Interpreter
{
    public partial class MainWindow : Window
    {
        PortManager port = new PortManager();
        string selectedForConfig = "";
        private const int SW_RESTORE = 9;
        string destinationWindow = "";

        [DllImport("User32.dll")]
        public static extern Int32 SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte VKey, byte scanCode, uint flags, IntPtr eInfo);

        private const int KEYEVENTF_EXTENDEDNKEY = 0x0001;
        private const int KEYEVENTF_KEYDOWN = 0x0000;
        private const int KEYEVENTF_KEYUP = 0x0002;

        private static void Executer(string keyval)
        {
            int decVal = Convert.ToInt32(keyval, 16);
            byte byteVal = Convert.ToByte(decVal);
            keybd_event(byteVal, 0, KEYEVENTF_KEYDOWN, IntPtr.Zero);
            keybd_event(byteVal, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
        }

        Dictionary<string, string> valueForBtn = new Dictionary<string, string>
        {
            {"d",""},
            {"c",""},
            {"l",""},
            {"p",""},
            {"m",""}
        };

        public MainWindow()
        {
            InitializeComponent();
            port.configure();
            port.getPort().DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

            ComboCreator pCombo = new ComboCreator(comboPorts);
            pCombo.populateCombo();
            comboPorts.SelectionChanged += new SelectionChangedEventHandler(comboPorts_SelectedIndexChanged);

            ComboCreator dCombo = new ComboCreator(doubleCombo);
            dCombo.populateCombo();
            doubleCombo.SelectionChanged += new SelectionChangedEventHandler(doubleCombo_SelectedIndexChanged);
            
            ComboCreator fCombo = new ComboCreator(foregroundCombo);
            fCombo.populateCombo();
            foregroundCombo.SelectionChanged += new SelectionChangedEventHandler(foregroundCombo_SelectedIndexChanged);
        }
        
        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Process[] processes = Process.GetProcessesByName(destinationWindow);
            foreach (Process process in processes)
            {
                ShowWindow(process.MainWindowHandle, SW_RESTORE);
                SetForegroundWindow(process.MainWindowHandle);
            }

            string dataReceived = port.getPort().ReadExisting();
            if (valueForBtn[dataReceived] != "")
                Executer(valueForBtn[dataReceived]);
            //Console.WriteLine(dataReceived);
            else
                MessageBox.Show("Must specify a value for the button.");
        }

        public void btnConnect(object sender, EventArgs e)
        {
            SerialPort localPort = port.getPort();
            try
            {
                if (!localPort.IsOpen)
                {
                    localPort.Open();
                    MessageBox.Show(this, "Port  " + localPort.PortName + " open!");
                    comboPorts.IsEnabled = false;
                }
                else
                    Console.WriteLine("Already Open");
            }
            catch (UnauthorizedAccessException uae)
            {
                MessageBox.Show(uae.ToString());
            }
        }

        public void setButton(object sender, EventArgs e)
        {
            selectedForConfig = ((Button)sender).Name.ToString();
            labelSelected.Content = ((Button)sender).Content.ToString();
        }


        private void comboPorts_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            port.getPort().PortName = ((ComboBox)sender).SelectedValue.ToString();
        }

        public void foregroundCombo_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            destinationWindow = foregroundCombo.SelectedValue.ToString();
        }

        public void doubleCombo_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                using (StreamReader sr = new StreamReader("../../keymap.json"))
                {
                    string line = sr.ReadLine();
                    while (!line.Contains(doubleCombo.SelectedValue.ToString()))
                    {
                        sr.Peek();
                        line = sr.ReadLine();
                        if (line == "}")
                        {
                            break;
                        }
                    }

                    Regex regexNum = new Regex("\"(0x.*?)\"");

                    var matchCollection = regexNum.Matches(line);
                    valueForBtn[selectedForConfig] = matchCollection[0].Groups[1].Value.ToString();

                    Console.WriteLine(matchCollection[0].Groups[1].Value.ToString());
                }
            }
            catch (IOException ae)
            {
                MessageBox.Show(ae.Message);
            }
        }

    }
}
