using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary.Actuators
{
    /// <summary>
    /// optimised collection class for a bunch of Actuators
    /// </summary>
    public class OnOffActuators
    {

        Dictionary<int, OnOffActuator> actuatorConfig = new Dictionary<int, OnOffActuator>();
        public OnOffActuators(int onMs)
        {
            SetAllActualtorConfigs(new OnOffActuator(onMs));
        }

        public OnOffActuator GetActuator(int pinnumber)
        {
            if (!actuatorConfig.ContainsKey(pinnumber)) return null;
            return actuatorConfig[pinnumber];
        }

        public OnOffActuators SetAllActualtorConfigs(OnOffActuator cfg)
        {
            foreach (var pinNum in actuatorConfig.Keys.ToList())
            {
                SetActuator(pinNum, cfg.Copy());
            }
            return this;
        }

        public OnOffActuators SetActuator(int pin, OnOffActuator config)
        {
            actuatorConfig[pin] = config;
            return this;
        }
    }
}
