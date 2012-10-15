using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary
{
    /// <summary>
    /// low level message to be sent to an ardiuno pin
    /// </summary>
    public class PinMessage
    {
        public PinMessage() { }
        public PinMessage(int number, byte value)
            : this()
        {
            this.PinNumber = number;
            this.Value = value;
        }

        public byte Value { get; set; }
        public int PinNumber { get; set; }


        /// <summary>
        /// static helper to create a message with zero voltage for a pin
        /// </summary>
        public static PinMessage Off(int pin)
        {
            return new PinMessage()
            {
                PinNumber = pin,
                Value = 0
            };
        }

        /// <summary>
        /// static helper to create a message to send to a pin
        /// </summary>
        public static PinMessage On(int pin, byte value)
        {
            return new PinMessage()
            {
                PinNumber = pin,
                Value = value
            };
        }

        public override string ToString()
        {
            return "p: " + PinNumber + " v:" + Value;
        }
    }
}
