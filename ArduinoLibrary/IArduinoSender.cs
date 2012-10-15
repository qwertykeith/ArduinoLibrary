
namespace ArduinoLibrary
{
    /// <summary>
    /// represents a class that
    /// sends messages to arduino pins
    /// </summary>
    public interface IArduinoSender
    {
        /// <summary>
        /// sends a message
        /// </summary>
        /// <param name="cmd"></param>
        void Send(PinMessage cmd);

        /// <summary>
        /// code needed to load into the device
        /// to make this class work
        /// </summary>
        string Code { get; }
    }
}
