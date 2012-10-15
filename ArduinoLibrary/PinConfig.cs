using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary
{
    public enum PinTypeEnum { DIGITAL, PWM }

    /// <summary>
    /// describes a pin on an ardiono
    /// </summary>
    public class PinConfig
    {
        public PinConfig(PinTypeEnum pintType)
        {
            this.PintType = pintType;
        }

        public PinTypeEnum PintType { get; set; }

        public PinConfig Copy()
        {
            var c = (PinConfig)this.MemberwiseClone();
            return c;
        }

        public int VoltageSteps
        {
            get
            {
                switch (PintType)
                {
                    case PinTypeEnum.DIGITAL:
                        return 2;
                    case PinTypeEnum.PWM:
                        return 256;
                    default:
                        return 2;
                }
            }
        }

    }
}
