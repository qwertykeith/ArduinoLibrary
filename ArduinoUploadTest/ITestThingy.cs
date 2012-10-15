using ArduinoLibrary.SketchUploader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoUploadTest
{
    interface ITestThingy
    {
        bool Running { get; set; }
        void Run();
    }
}
