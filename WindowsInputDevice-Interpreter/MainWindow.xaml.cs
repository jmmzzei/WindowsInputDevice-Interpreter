using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

namespace WindowsInputDevice_Interpreter
{
    public partial class MainWindow : Window
    {
        SerialPort port = new SerialPort();

        Dictionary<string, Action> keystroke = new Dictionary<string, Action>
        {
            {"d", Next },
            {"l", Prev },
            {"c", Play },
            {"+", Up },
            {"-", Down }
        };

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte VKey, byte scanCode, uint flags, IntPtr eInfo);

        private const int KEYEVENTF_EXTENDEDNKEY = 1;
        private const int KEYEVENTF_KEYUP = 0;
        private const int VK_MEDIA_NEXT = 0xB0;
        private const int VK_MEDIA_PREV = 0xB1;
        private const int VK_MEDIA_PLAY_STOP = 0xB3;
        private const int VK_MEDIA_VOL_UP = 0xAF;
        private const int VK_MEDIA_VOL_DOWN = 0xAE;
        private const int VK_MEDIA_VOL_MUTE = 0xAD;

        private static void Play()
        {
            keybd_event(VK_MEDIA_PLAY_STOP, KEYEVENTF_KEYUP, KEYEVENTF_EXTENDEDNKEY, IntPtr.Zero);
        }
        private static void Next()
        {
            keybd_event(VK_MEDIA_NEXT, KEYEVENTF_KEYUP, KEYEVENTF_EXTENDEDNKEY, IntPtr.Zero);
        }
        private static void Prev()
        {
            keybd_event(VK_MEDIA_PREV, KEYEVENTF_KEYUP, KEYEVENTF_EXTENDEDNKEY, IntPtr.Zero);
        }
        private static void Mute()
        {
            keybd_event(VK_MEDIA_VOL_MUTE, KEYEVENTF_KEYUP, KEYEVENTF_EXTENDEDNKEY, IntPtr.Zero);
        }
        private static void Down()
        {
            keybd_event(VK_MEDIA_VOL_DOWN, KEYEVENTF_KEYUP, KEYEVENTF_EXTENDEDNKEY, IntPtr.Zero);
        }
        private static void Up()
        {
            keybd_event(VK_MEDIA_VOL_UP, KEYEVENTF_KEYUP, KEYEVENTF_EXTENDEDNKEY, IntPtr.Zero);
        }

        public MainWindow()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            foreach (var item in ports)
                comboPorts.Items.Add(item);
            configurePort(port);

            comboPorts.SelectionChanged += new SelectionChangedEventHandler(comboPorts_SelectedIndexChanged);
        }

        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataReceived = port.ReadExisting();
            keystroke[dataReceived].Invoke();
        }

        public void connect(object obj, EventArgs e)
        {
            if (!port.IsOpen)
            {
                port.Open();
                MessageBox.Show(this, "Port  " + port.PortName + " open!");
                comboPorts.IsEnabled = false;
            }
            else 
                Console.WriteLine("Already Open");
        }

        public void comboPorts_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            port.PortName = comboPorts.SelectedValue.ToString();
        }
        
        public void configurePort(SerialPort port)
        {
            port.BaudRate = 9600;
            port.DtrEnable = true;
            port.DtrEnable = true;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }
    }
}
