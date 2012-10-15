using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary.Actuators
{
    /// <summary>
    /// describes an arduino pint that is connected to an actuator
    /// </summary>
    public class ActuatorPin
    {
        public OnOffActuator Actuator { get; private set; }
        public PinConfig Pin { get; private set; }

        public ActuatorPin(PinConfig pin)
        {
            this.Pin = pin;
        }

        public ActuatorPin(PinConfig pin, OnOffActuator actuator)
            : this(pin)
        {
            this.Actuator = actuator;
        }

        public ActuatorPin SetActuator(OnOffActuator actuator)
        {
            this.Actuator = actuator;
            return this;
        }

    }
}
