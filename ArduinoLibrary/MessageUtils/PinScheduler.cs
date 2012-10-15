using System.Collections.Generic;
using System.Linq;

namespace ArduinoLibrary.MessageUtils
{
    /// <summary>
    /// thread safe
    /// collection class to schedule and de-schedule (retrieve)
    /// pin events (pin messages with time stamps) in non-real time
    /// </summary>
    public class PinScheduler
    {
        List<PinEvent> toRemove;

        object locker = new object();

        List<PinEvent> events = new List<PinEvent>();

        public IEnumerable<PinEvent> GetEvents() { return events; }

        public void Clear()
        {
            lock (locker)
            {
                events.Clear();
                toRemove = new List<PinEvent>();
            }
        }

        public PinScheduler()
        {
        }

        public void Schedule(PinMessage m, double ms)
        {
            Schedule(new PinEvent() { Message = m, TotalMsDelta = ms });
        }

        public void Schedule(IEnumerable<PinEvent> p)
        {
            lock (locker)
            {
                events.AddRange(p);
            }
        }

        /// <summary>
        /// schedule this event
        /// </summary>
        /// <param name="p"></param>
        public void Schedule(PinEvent p)
        {
            lock (locker)
            {
                events.Add(p);
            }
        }

        /// <summary>
        /// return and remove all events that exist up to and
        /// including the ms given
        /// </summary>
        /// <param name="upToMs"></param>
        /// <returns></returns>
        public IEnumerable<PinMessage> Deschedule(double upToMs)
        {

            IList<PinEvent> es;
            lock (locker)
            {
                es = new List<PinEvent>(events.OrderBy(p => p.TotalMsDelta));
            }
            for (int i = 0; i < es.Count; i++)
            {
                var e = es[i];
                if (e.TotalMsDelta <= upToMs)
                {
                    var m = e.Message;

                    toRemove.Add(e);

                    yield return m;
                }
                else
                {
                    // it's past the time...
                    // just break from the loop
                    break;
                }
            }

            // remove all the messages we just sent from the events list
            lock (locker)
            {
                for (int i = 0; i < toRemove.Count; i++)
                {
                    events.Remove(toRemove[i]);
                }
                toRemove.Clear();
            }

        }
    }
}
