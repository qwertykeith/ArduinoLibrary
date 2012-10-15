using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary.Actuators
{
    /// <summary>
    /// collection class for ActuatorPins
    /// </summary>
    public class ActuatorPins
    {
        Dictionary<int, ActuatorPin> apins = new Dictionary<int, ActuatorPin>();

        public ActuatorPins(IEnumerable<int> pinNumbers, PinConfig pconfig, OnOffActuator actuator)
        {
            foreach (var p in pinNumbers)
            {
                apins[p] = new ActuatorPin(pconfig, actuator);
            }
        }

        public ActuatorPins(IEnumerable<int> pinNumbers, PinTypeEnum pintType, OnOffActuator actuator)
            : this(pinNumbers, new PinConfig(pintType), actuator)
        {
        }

        public ActuatorPins(IEnumerable<int> pins, PinTypeEnum pintType, int onLengthMs) : this(pins, new PinConfig(pintType), new OnOffActuator(onLengthMs)) { }
        public ActuatorPins(IEnumerable<int> pins, PinConfig pin, int onLengthMs) : this(pins, pin, new OnOffActuator(onLengthMs)) { }

        public int Count { get { return apins.Count; } }
        public IEnumerable<int> PinNumbers { get { return apins.Keys; } }
        public ActuatorPin GetActuatorPin(int pin) { return apins[pin]; }

        public ActuatorPins SetActuator(int pin, OnOffActuator actuator)
        {
            if (apins.ContainsKey(pin)) throw new Exception("No Pin to connect to");
            apins[pin].SetActuator(actuator);

            return this;
        }

   

        /// <summary>
        /// adds this actuator definition to all pins
        /// </summary>
        /// <param name="actuator"></param>
        /// <returns></returns>
        public ActuatorPins SetAllActuators(OnOffActuator actuator)
        {
            foreach (var p in apins.Keys)
            {
                SetActuator(p, actuator);
            }
            return this;
        }

   



    }
}
