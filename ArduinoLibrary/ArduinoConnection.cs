using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ArduinoLibrary
{

    /// <summary>
    /// provides very low level access to the serial port that an ardiuno is plugged in to
    /// </summary>
    public class ArduinoConnection : IDisposable 
    {
        SerialPort port;

        public int BaudRate { get; set; }


        public event EventHandler OnConnected;
        public event EventHandler OnConnectError;
        public event EventHandler OnConnecting;
        public event EventHandler OnDisconnected;
        public event EventHandler OnDisconnecting;

        public ArduinoConnection()
        {
            this.BaudRate = 115200;
            port = new SerialPort();
            port.ErrorReceived += new SerialErrorReceivedEventHandler(port_ErrorReceived);
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        /// <summary>
        /// send a byte array to the ardiuno's port
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="count"></param>
        public void Write(byte[] bytes, int count)
        {
            if (port.IsOpen) port.Write(bytes, 0, count);
        }

        public bool IsConnected { get { return port.IsOpen; } }

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public string PortName { get; set; }

        void port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }




        public static ArduinoConnection Create(string portName)
        {
            return new ArduinoConnection()
            {
                PortName=portName
            };
        }

        /// <summary>
        /// lists the names of all available COM ports
        /// </summary>
        /// <returns></returns>
        public static string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public void OpenConnection()
        {
            if ((PortName ?? "") != "")
            {
                if (!port.IsOpen)
                {
                    //                if ((PortName ?? "") == "") PortName = AvailablePorts().Last();

                    if (OnConnecting != null) OnConnecting(this, null);
                    
                    System.Diagnostics.Debug.WriteLine("port: " + PortName);
                    port = new SerialPort(PortName, BaudRate);
                    port.ErrorReceived += new SerialErrorReceivedEventHandler(port_ErrorReceived);
                    port.Open();
                    System.Diagnostics.Debug.WriteLine("Wait for bootup");
                    //                    System.Threading.Thread.Sleep(2000);
                    System.Diagnostics.Debug.WriteLine("Go");

                    if (port.IsOpen)
                    {
                        if (OnConnected != null) OnConnected(this, null);
                    }
                    else
                    {
                        if (OnConnectError != null) OnConnectError(this, null);
                    }
                }
            }
            else
            {
                //                throw new Exception("No port specified for the arduino");
                if (OnConnectError != null) OnConnectError(this, null);
            }
        }

        public void CloseConnection()
        {
            if (port.IsOpen)
            {
                if (OnDisconnecting != null) OnDisconnecting(this, null);
                port.Close();
                System.Diagnostics.Debug.WriteLine("Port closed...");
                if (OnDisconnected != null) OnDisconnected(this, null);

            }
        }

        public virtual void Dispose()
        {
            CloseConnection();
            Console.WriteLine("Disconnected after dispose");
        }
    }
}
