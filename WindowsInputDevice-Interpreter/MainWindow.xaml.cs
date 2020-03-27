using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace WindowsInputDevice_Interpreter
{
    public partial class MainWindow : Window
    {
        SerialPort port = new SerialPort();

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
            Console.WriteLine(dataReceived);
        }

        public void connect(object obj, EventArgs e)
        {
            if (!port.IsOpen)
            {
                port.Open();
                MessageBox.Show(this, "Port  " + port.PortName + " open!");
            }
            else 
                Console.WriteLine("Already Open");
        }

        public void comboPorts_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            port.PortName = comboPorts.SelectedValue.ToString();
            comboPorts.IsEnabled = false;
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
