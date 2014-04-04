using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary.SketchUploader
{
    /// <summary>
    /// uses arduino library files 1.01
    /// http://arduino.cc/en/Main/Software
    /// </summary>
    public class UploadConfig
    {
        internal UploadConfig(string arduinoPath, string tempPath, Board board)
        {
            this.TempDir = tempPath.Replace("/",@"\").Trim('/').Trim('\\'); // do the replace so dos is happy
            this.ArduinoDirectory = arduinoPath;
            this.Board = board;
        }
        internal Board Board { get; private set; }

        public string TempDir { get; private set; }
        public string TempCodePath { get { return TempDir + @"\code.pde"; } }
        public string TempCppCodePath { get { return TempDir + @"\" + TempCppCodeFileName; } }
        public string TempCppCodeFileName { get { return "code.cpp"; } }
        public string ArduinoDirectory { get; private set; }
        public string ArduinoLibDirectory { get { return ArduinoDirectory + @"\libraries\"; } }

        public string Avrdude { get { return ArduinoDirectory + @"\hardware\tools\avr\bin\avrdude.exe"; } }
        public string AvrdudeConfig { get { return ArduinoDirectory + @"\hardware\tools\avr\etc\avrdude.conf"; } }

        public string Gcc { get { return ArduinoDirectory + @"\hardware\tools\avr\bin\avr-gcc"; } }
        public string Gpp { get { return ArduinoDirectory + @"\hardware\tools\avr\bin\avr-g++"; } }
        public string Ar { get { return ArduinoDirectory + @"\hardware\tools\avr\bin\avr-ar"; } }
        public string Objcp { get { return ArduinoDirectory + @"\hardware\tools\avr\bin\avr-objcopy"; } }
        public string Avrsize { get { return ArduinoDirectory + @"\hardware\tools\avr\bin\avr-size"; } }
        public string ArduCore { get { return ArduinoDirectory + @"\hardware\arduino\cores\" + Board.Core; } }
        public string IncludeFiles { get { return ArduinoDirectory + @"\hardware\arduino\cores\" + Board.Core; } }
        public string IncludeLibrariesRootPath { get { return ArduinoDirectory + @"\libraries\"; } }
        public string VariantDir { get { return ArduinoDirectory + @"\hardware\arduino\variants\" + Board.Variant; } }

        public CommunicationTypeEnum Communication { get; set; }
        public bool Force { get; set; }
        public bool VerboseOutput { get; set; }
    }

    public enum CommunicationTypeEnum { USB, Serial }

}
