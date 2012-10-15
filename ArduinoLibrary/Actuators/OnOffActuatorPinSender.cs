using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArduinoLibrary.Actuators
{
    /// <summary>
    /// sends a message to a pin
    /// immediately sends a pin on message, 
    /// and follows it up with an off message when it's needed
    /// </summary>
    public class OnOffActuatorPinSender : IArduinoSender, IDisposable
    {
        class NoteInfo
        {
            public DateTime Off { get; set; }
            public PinMessage Message { get; set; }
        }

        ActuatorPins pins;
        SimplePinSender pinSender;

        Dictionary<int, NoteInfo> noteons = new Dictionary<int, NoteInfo>();
        public OnOffActuatorPinSender(ArduinoConnection con, ActuatorPins pins)
        {
            this.pinSender = new SimplePinSender(con);
            this.pins = pins;
        }

        object noteLock = new object();

        public void Panic()
        {
            foreach (var p in pins.PinNumbers)
            {
                pinSender.Send(new PinMessage()
                {
                    PinNumber = p,
                    Value = 0
                });
            }
        }

        void playThreadLoop()
        {

            while (noteons.Keys.Count > 0)
            {
                var now = DateTime.Now;

                // get copy of notes for thread safety reasons
                IList<NoteInfo> ns;
                lock (noteLock)
                {
                    ns = noteons.Values.ToList();
                }

                foreach (var n in ns)
                {
                    if (now > n.Off)
                    {
                        // send the message straight away
                        send(new PinEvent()
                        {
                            Message = n.Message,
                            TotalMsDelta = 0
                        });
                    }
                }
            }
            playerThread = null;
        }

        Thread playerThread;


        void queueEvent(PinEvent e)
        {
            // schedule the off note
            var dueOffAt = DateTime.Now.AddMilliseconds(e.TotalMsDelta);

            lock (noteLock)
            {
                noteons[e.Message.PinNumber] = new NoteInfo()
                {
                    Message = e.Message,
                    Off = dueOffAt,
                };
            }

            // start thread if needed
            if (playerThread == null)
            {
                playerThread = new Thread(new ThreadStart(playThreadLoop));
                playerThread.Start();
            }

        }

        void send(PinEvent e)
        {
            // send it right away if needed (optimisation)
            if (e.TotalMsDelta == 0)
            {
                // yep... go send
                //                Debug.WriteLine("> " + e.Message.PinNumber + " --- " + e.Message.Value);
                pinSender.Send(e.Message);
                if (e.Message.Value == 0)
                {
                    // this is off.. take it from the queue
                    lock (noteLock)
                    {
                        noteons.Remove(e.Message.PinNumber);
                    }
                }
            }
            else
            {
                // no.. needs to be queued
                queueEvent(e);
            }

        }

        public IEnumerable<PinEvent> getOnOffEvents(PinMessage m, double ms)
        {
            var pinToSend = m.PinNumber;

            var actuator = pins.GetActuatorPin(pinToSend).Actuator;

            // calculate the off time
            var offTime = ms + actuator.GetOnLengthMsForVoltage(m.Value);

            // do the velocity (it is scaled in the actuator)
            var value = actuator.MapData(m.Value);

            // on
            yield return new PinEvent()
            {
                TotalMsDelta = ms,
                Message = new PinMessage()
                {
                    PinNumber = pinToSend,
                    Value = value
                }
            };
            // off
            yield return new PinEvent()
            {
                TotalMsDelta = offTime,
                Message = new PinMessage()
                {
                    PinNumber = pinToSend,
                    Value = 0
                }
            };
        }

        public void Send(PinMessage cmd)
        {
            //            System.Diagnostics.Debug.WriteLine(value);
            // get the actual pin messages (on and off)
            var events = getOnOffEvents(cmd, 0);

            foreach (var e in events)
            {
                // send each message
                if (!noteons.ContainsKey(cmd.PinNumber))
                {
                    // not already playing.. send it
                    send(e);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("skip " + cmd.PinNumber);
                }
            }
        }

        public string Code { get { return pinSender.Code; } }



        public void Dispose()
        {
            // make sure everything is off
            Panic();
            // kill the thread if it's running
            if (playerThread != null)
            {
                playerThread.Abort();
                playerThread = null;
            }
        }
    }
}
