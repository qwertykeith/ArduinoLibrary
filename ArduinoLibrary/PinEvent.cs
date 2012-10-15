using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary
{
    /// <summary>
    /// a time-stamped message for sending to a pin
    /// </summary>
    public class PinEvent
    {
        public double TotalMsDelta { get; set; }
        public PinMessage Message { get; set; }
    }
}
