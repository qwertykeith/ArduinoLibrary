using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary.Actuators
{
    /// <summary>
    /// describes the behaviour of an actuator and 
    /// provides some functions to help use it
    /// </summary>
    public class OnOffActuator
    {

        /// <summary>
        /// arbitrary description for this actuator
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// how long the actuator should stay down for at low velocity
        /// this will be a higher number than the OnLengthMsLowVelocity eg: 90
        /// </summary>
        public int OnLengthMsLowVoltage { get; set; }
        /// <summary>
        /// how long the actuator should stay down for at high velocity
        /// this will be a higher number than the OnLengthMsLowVelocity eg: 50
        /// </summary>
        public int OnLengthMsHighVoltage { get; set; }

        /// <summary>
        /// between 0 and 1, the minimum ratio used when 
        /// mapping calculating the actual voltage to send to a pin
        /// </summary>
        public double MinVoltageRatio { get; set; }

        Dictionary<byte, byte> voltageCurveMappingsCache = new Dictionary<byte, byte>();


        double voltageCurve;
        /// <summary>
        /// a number representing the steepness of the curve that maps the 
        /// eventual voltage value sent to an actuator
        /// </summary>
        public double VelocityCurve
        {
            get { return voltageCurve; }
            set
            {
                voltageCurve = value;
                for (int i = 0; i < 256; i++)
                {
                    voltageCurveMappingsCache[(byte)i] = (byte)(Math.Pow(i / 255.0, voltageCurve) * 255);
                }
            }
        }

        /// <summary>
        /// create an actuator that stays on for the same length of time regardless of voltage
        /// </summary>
        /// <param name="onLengthMs"></param>
        public OnOffActuator(int onLengthMs)
            : this(onLengthMs, onLengthMs)
        {
        }

        /// <summary>
        /// create an actuator that varies the length of time it stays 
        /// on for depending on the voltage it is sent
        /// </summary>
        public OnOffActuator(int onLengthMsLowVoltage, int onLengthMsHighVoltage)
        {
            VelocityCurve = 1;
            OnLengthMsHighVoltage = onLengthMsHighVoltage;
            OnLengthMsLowVoltage = onLengthMsLowVoltage;
        }

        /// <summary>
        /// using informtation about this actuator
        /// maps to a new voltage using the min, max and voltage curve
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns></returns>
        public byte MapData(byte voltage)
        {
            if (voltage <= 0) return 0;

            if (voltage > 255) voltage = 255;
            voltage = voltageCurveMappingsCache[voltage];

            var max = 255;
            var min = (int)(MinVoltageRatio * 255.0);

            return (byte)ArduinoUtils.Map(voltage, 0, 255, min, max);

        }

        /// <summary>
        /// get the length in ms the actuator will send voltage for
        /// </summary>
        /// <param name="velByte"></param>
        /// <returns></returns>
        public double GetOnLengthMsForVoltage(byte velByte)
        {
            if (velByte == 0) return 0;

            // hmmm NOT RIGHT?
            var velRatio = velByte / 255.0;
            return (velRatio * (OnLengthMsHighVoltage - OnLengthMsLowVoltage)) + OnLengthMsLowVoltage;

        }


        public OnOffActuator Copy()
        {
            return (OnOffActuator)this.MemberwiseClone();
        }

    }
}
