using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace ArduinoLibrary
{
    /// <summary>
    /// describes a specific pin on an arduino
    /// eg: pin 3 on an "arduino mega" would be created to have a PWM pin config
    /// </summary>
    public class ArduinoPin
    {
        public int PinNumber { get; private set; }
        public PinConfig Config { get; private set; }

        internal ArduinoPin(int pinnum, PinConfig config)
        {
            this.PinNumber = pinnum;
            this.Config = config;
        }



    }
}
