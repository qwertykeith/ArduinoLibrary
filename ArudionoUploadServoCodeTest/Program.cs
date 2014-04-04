using ArduinoLibrary;
using ArduinoLibrary.SketchUploader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArudionoUploadServoCodeTest
{
    class Program
    {
        static ArduinoConnection a;
        static Uploader u;

        const string code = @"
                 #include <Servo.h> 
                Servo myservo;  
                void setup()
                {
                  Serial.begin(300); 
                  myservo.attach(2);  // attaches the servo on pin 2 to the servo object 
                  Serial.flush();
                }

                void loop()
                {
                 }
            ";


        static void Main(string[] args)
        {
            // create the arduino connection
            a = new ArduinoConnection();
            // just use the last com port for this.. it's usually the one
            a.PortName = ArduinoConnection.GetAvailablePorts().Last();

            // create the uploader
            // this is where the root of the ardiono ide installation lives
            var root = @"C:\arduino-1.0.1";
            var tempDir = Environment.CurrentDirectory + "/Temp";

            var info = Uploader.GetUploadConfig(root, tempDir,
                "Arduino Mega 2560 or Mega ADK",
                CommunicationTypeEnum.USB);

            u = new Uploader(info);
            u.OnError += u_OnError;
            u.OnMessage += u_OnMessage;
            u.OnSuccess += u_OnSuccess;

            // upload code
            u.UploadCode(a, code);

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

