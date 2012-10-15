using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary
{
    /// <summary>
    /// sends a byte value (voltage) to a pin on an arduino...
    /// code is provided to load onto the device to make this work
    /// </summary>
    public class SimplePinSender : IArduinoSender
    {
        byte[] buffer = new byte[] { 0, 0 };

        ArduinoConnection con;

        public SimplePinSender(ArduinoConnection arduino)
        {
            this.con = arduino;
        }

        public void Send(PinMessage cmd)
        {
            Send(cmd.PinNumber, cmd.Value);
        }

        public void Send(int pinNumber, byte value)
        {
            buffer[0] = (byte)(pinNumber);
            buffer[1] = (byte)(value);
            con.Write(buffer, 2);
        }

        public string Code
        {
            get
            {
                return @"
                    #define PINS 16
                    #define BAUD 115200 // 9600 //57600 

                    int state;
                    byte pin;

                    void setup()
                    {
                      Serial.begin(BAUD); 
                      for(int i=2; i<PINS; i++){  
                        pinMode(i, OUTPUT);
                        digitalWrite(i, LOW);
                      }
                      Serial.flush();
                    }

                    void loop()
                    {

                      if(Serial.available()>1)
                      {    
                        pin = Serial.read();
                        state = Serial.read();
                        analogWrite(pin, state);
                      }
                    }
                    ";
            }
        }

    }
}
