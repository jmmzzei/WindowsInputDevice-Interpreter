using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WindowsInputDevice_Interpreter
{
    class ComboCreator
    {
        private ComboBox comboBox;
        public ComboCreator(ComboBox comboBox)
        {
            this.comboBox = comboBox;
        }
        public void populateCombo()
        {
            if (this.comboBox.Name == "comboPorts")
            {
                Console.WriteLine("INNNNNNNNN");
                populatePortCombo();
            } else if(this.comboBox.Name == "doubleCombo"){
                populateDoubleCombo();
            }
            else if(this.comboBox.Name == "foregroundCombo")
            {
                populateForegroundCombo();
            }
        }

        private void populateDoubleCombo()
        {
            try
            {
                using (StreamReader sr = new StreamReader("../../keymap.json"))
                {
                    string line = sr.ReadLine();
                    while (!line.Contains("-----"))
                    {
                        sr.Peek();
                        line = sr.ReadLine();
                        if (line == "}")
                        {
                            break;
                        }
                        Regex regexNum = new Regex("\"([^(0x)].*?)\"");

                        var matchCollection = regexNum.Matches(line);
                        for (int i = 0; i < matchCollection.Count; i++)
                        {
                            this.comboBox.Items.Add(matchCollection[i].Groups[1].Value);
                        }

                    }
                }
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void populateForegroundCombo()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                this.comboBox.Items.Add(process.ProcessName);
            }

        }

        private void populatePortCombo()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (var port in ports)
                comboBox.Items.Add(port);
        }
    }
}
