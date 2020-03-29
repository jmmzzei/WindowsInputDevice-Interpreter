using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WindowsInputDevice_Interpreter
{
    class PortManager
    {
        private SerialPort port = new SerialPort();

        public void configure()
        {
            port.BaudRate = 9600;
            port.DtrEnable = true;
            port.DtrEnable = true;
            port.Parity = Parity.None;
            port.DataBits = 8;
        }

        public SerialPort getPort()
        {
            return port;
        }
    }
}
