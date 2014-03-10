using ArduinoLibrary;
using ArduinoLibrary.SketchUploader;
using ArduinoUploadTest.Tests;
using System;
using System.Linq;
using System.Threading;

namespace ArduinoUploadTest
{
    class Program
    {

        static ArduinoConnection a;
        static Uploader u;

        static void Main(string[] args)
        {


            // descript the arduino
            a = new ArduinoConnection();
            // just use the last com port for this.. that one's usually the one
            a.PortName = ArduinoConnection.GetAvailablePorts().Last();

            // create the uploader
            // this is where the root of the ardiono ide installation lives
            var root = @"C:\Users\Administrator\Documents\Software\arduino-1.0.1";
            var tempDir = Environment.CurrentDirectory + "/Temp";

            var info = Uploader.GetUploadConfig(root, tempDir,
                "Arduino Mega 2560 or Mega ADK",
                CommunicationTypeEnum.USB);

            u = new Uploader(info);
            u.OnError += u_OnError;
            u.OnMessage += u_OnMessage;
            u.OnSuccess += u_OnSuccess;

            // do some tests
            runTest(new TestBlink(a, u));
            Thread.Sleep(4000);
            runTest(new TestSendOnOffMessage(a, u));
        }


        static void runTest(ITestThingy test)
        {
            test.Running = true;
            test.Run();
            while (test.Running) ;
        }

        static void u_OnSuccess(object sender, EventArgs e)
        {
            Console.WriteLine("Done!");
        }

        static void u_OnMessage(object sender, EventArgs e)
        {
            Console.WriteLine((sender as Uploader).Message);
        }

        static void u_OnError(object sender, EventArgs e)
        {
            Console.WriteLine("Oops error!");
        }
    }
}
