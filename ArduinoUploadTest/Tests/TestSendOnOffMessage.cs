using ArduinoLibrary;
using ArduinoLibrary.Actuators;
using ArduinoLibrary.SketchUploader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ArduinoUploadTest.Tests
{
    class TestSendOnOffMessage : ITestThingy
    {
        static OnOffActuatorPinSender messageSender;

        ArduinoConnection a;
        Uploader u;
        public TestSendOnOffMessage(ArduinoConnection a, Uploader u)
        {
            this.a = a;
            this.u = u;
        }

        public bool Running { get; set; }

        public void Run()
        {
            var pins = new int[] { 2, 3, 4 };
            var pinConfig = new PinConfig(PinTypeEnum.PWM);
            var actuatorPins = new ActuatorPins(pins, pinConfig, 50);

            // create a sender to send pin info
            messageSender = new OnOffActuatorPinSender(a, actuatorPins);

            var code = messageSender.Code;


            // upload the code
            u.UploadCode(a, code);

            a.OnConnected += a_OnConnected;
            a.OnDisconnected += a_OnDisconnected;
            a.OpenConnection();


        }

        void a_OnDisconnected(object sender, EventArgs e)
        {
            // let the console app finish
            Running = false;

        }

        void a_OnConnected(object sender, EventArgs e)
        {
            // give
            Thread.Sleep(1500);

            // send a few messages out
            messageSender.Send(new PinMessage(2, 255));
            Thread.Sleep(500);
            messageSender.Send(new PinMessage(3, 160));
            Thread.Sleep(500);
            messageSender.Send(new PinMessage(4, 240));

            Thread.Sleep(1000);

            a.CloseConnection();

        }


    }
}
