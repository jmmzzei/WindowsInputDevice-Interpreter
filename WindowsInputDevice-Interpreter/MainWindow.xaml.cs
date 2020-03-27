using System;
using System.IO.Ports;
using System.Windows;

namespace WindowsInputDevice_Interpreter
{
    public partial class MainWindow : Window
    {
        SerialPort port = new SerialPort();

        public MainWindow()
        {
            InitializeComponent();
            configurePort(port);
        }

        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataReceived = port.ReadExisting();
            Console.WriteLine(dataReceived);
        }

        public void connect(object obj, EventArgs e)
        {
            if (!port.IsOpen)
                port.Open();
        }

        public void configurePort(SerialPort p)
        {
            port.PortName = "COM19";
            port.BaudRate = 9600;
            port.DtrEnable = true;
            port.DtrEnable = true;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }
    }
}
