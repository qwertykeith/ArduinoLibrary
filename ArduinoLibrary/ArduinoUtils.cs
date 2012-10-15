using System.Collections.Generic;
using System.Linq;

namespace ArduinoLibrary
{
    /// <summary>
    /// static helper classes and extensions
    /// </summary>
    public static class ArduinoUtils
    {

        public static long Map(long x, long in_min, long in_max, long out_min, long out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static double MapRange(double val, double min, double max)
        {
            return ((max - min) * val) + min;
        }

        public static bool IsAllOff(this IEnumerable<PinMessage> commands)
        {
            return commands.Sum(p => p.Value) == 0;
        }

        public static void ConnectToLast(this ArduinoConnection con)
        {
            con.PortName = ArduinoConnection.GetAvailablePorts().Last();
            con.OpenConnection();
        }       
    }
}
