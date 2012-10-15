using ArduinoLibrary;
using ArduinoLibrary.SketchUploader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoUploadTest.Tests
{
    class TestBlink : ITestThingy
    {
        public bool Running { get; set; }


        ArduinoConnection a;
        Uploader u;
        public TestBlink(ArduinoConnection a, Uploader u)
        {
            this.a = a;
            this.u = u;
        }   

        public void Run()
        {
            var code = @"
                int led = 13;
                void setup() {                
                 pinMode(led, OUTPUT);   
                }

                void loop() {
                  digitalWrite(led, LOW);    
                  delay(250);               
                 digitalWrite(led, HIGH);   
                  delay(800);               
                }
            ";

            u.UploadCode(a, code);
            Running = false;
        }
    }
}
