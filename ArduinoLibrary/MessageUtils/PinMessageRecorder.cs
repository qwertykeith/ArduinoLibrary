using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary.MessageUtils
{
    /// <summary>
    /// remembers messages timestamped at the current DeltaMs
    /// </summary>
    public class PinMessageRecorder
    {

        DateTime startTime;

        Queue<PinEvent> events = new Queue<PinEvent>();

        public bool FirstNoteStartsRecording { get; set; }

        public PinMessageRecorder()
        {
        }

        public double DeltaMs
        {
            get
            {
                DateTime now = DateTime.Now;
                return (now - startTime).TotalMilliseconds;

            }
        }

        public PinEvent[] GetEvents()
        {
            var c = new PinEvent[events.Count];
            events.CopyTo(c, 0);
            return c;
        }

        public void BeginRecord()
        {
            // the events need to be clear it this is starting
            events.Clear();
            startTime = DateTime.Now;
        }

        public void Record(PinMessage cmd)
        {
            if ((events.Count == 0) && (FirstNoteStartsRecording))
            {
                BeginRecord();
            }

            //            System.Diagnostics.Debug.WriteLine("Recorded pin " + cmd.PinNumber + "," + cmd.Value + " #" + events.Count + " at " + DeltaMs);

            events.Enqueue(new PinEvent()
            {
                Message = cmd,
                TotalMsDelta = DeltaMs
            });
        }
    }
}
