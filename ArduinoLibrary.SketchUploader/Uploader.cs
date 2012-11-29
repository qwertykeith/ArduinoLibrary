using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArduinoLibrary.SketchUploader
{
    /// <summary>
    /// <para>
    /// compiles and uploads code form a sketch to an arduino
    /// using the files and cofiguration distributed in 
    /// the arduino v1.01 software 
    /// http://arduino.googlecode.com/files/arduino-1.0.1-windows.zip
    /// thanks to http://arduinosketch.codeplex.com/ for some of the code and inspiration
    /// </para>
    /// <para>
    /// PLEASE NOTE: this has only been tested on an atmega2560
    /// </para>
    /// <para>
    /// ALSO NOTE: there is a bug in the avrdude.exe that comes with the ardion software
    /// you need to replace it with the version posted here
    /// http://code.google.com/p/arduino/issues/detail?id=806
    /// </para>
    /// </summary>
    public class Uploader
    {
        bool hasError = false;

        static BoardsInfo getBoards(string arduinoSoftwareRoorDir)
        {
            var prefFile = arduinoSoftwareRoorDir + @"\hardware\arduino\boards.txt";
            var text = File.ReadAllText(prefFile);

            return new BoardsInfo(text);

        }


        public static IEnumerable<string> GetBoardNames(string arduinoSoftwareRoorDir)
        {
            if (!Directory.Exists(arduinoSoftwareRoorDir)) return Enumerable.Empty<string>();
            return getBoards(arduinoSoftwareRoorDir).Select(p => p.Name);

        }

        public static UploadConfig GetUploadConfig(string arduinoSoftwareRoorDir, string tempPath, string boardName, CommunicationTypeEnum comType)
        {
            var board = getBoards(arduinoSoftwareRoorDir).Where(p => p.Name == boardName).Single();

            return new UploadConfig(arduinoSoftwareRoorDir, tempPath, board)
            {
                Force = true,
                VerboseOutput = true,
                Communication = comType,
            };
        }

        public event EventHandler OnMessage;
        public event EventHandler OnError;
        public event EventHandler OnSuccess;

        public string Message { get; set; }

        AvrdudeUploader avrUploader;

        UploadConfig info;
        public Uploader(UploadConfig info)
        {
            this.info = info;
            avrUploader = new AvrdudeUploader(info);
        }

        void runProcess(string filename, string arguments)
        {
            var m = Helpers.RunProcess(filename, arguments);
            if (m != "") message(m);
        }

        void message(string m)
        {
            Message = m;
            if (OnMessage != null) OnMessage(this, null);
        }

        public void UploadCode(ArduinoConnection con, string code)
        {
            hasError = false;
            // close the arduino if it was open
            bool wasOpen = con.IsConnected;
            if (wasOpen)
            {
                message("Closing current arduino connection");
                con.CloseConnection();
            }

            if (con.PortName == "") throw new Exception("Arduino port not specified");

            var c = new Compiler(info);
            c.OnError += c_OnError;
            c.OnMessage += c_OnMessage;
            c.OnSuccess += c_OnSuccess;

            try
            {
                // need to compile it first
                c.Compile(code);

                if (!hasError)
                {
                    // upload sketch 
                    message("Uploading to device via " + con.PortName);

                    doUpload(con);
                }

            }
            catch (Exception ex)
            {
                message("Error uploading!");
                if (OnError != null) OnError(this, null);
            }
            finally
            {
                c.OnError -= c_OnError;
                c.OnMessage -= c_OnMessage;
                c.OnSuccess -= c_OnSuccess;
                c = null;

                // open the connection again if needed
                if (wasOpen)
                {
                    message("Re-opening arduino connection");
                    con.OpenConnection();
                }
            }

            if (!hasError)
            {
                if (OnSuccess != null) OnSuccess(this, null);
            }
        }

        void doUpload(ArduinoConnection con)
        {
            string _fileName = "";

            this.avrUploader.SetCurrentPortName(con.PortName);

            DirectoryInfo _dirInfo = new DirectoryInfo(info.TempDir);
            FileInfo[] _fileInfos = _dirInfo.GetFiles();

            foreach (var _file in _fileInfos)
            {
                if (_file.Extension.Equals(".hex"))
                {
                    _fileName = _file.FullName;
                    break;
                }
            }

            // Upload hex file by avrdude
            var s = avrUploader.uploadViaBootloader(_fileName);
            message(s);
            message("Upload complete");

            // cleanup
            Directory.Delete(info.TempDir, true);
        }

        void c_OnSuccess(object sender, EventArgs e)
        {
            message("Compile success!");
        }

        void c_OnMessage(object sender, EventArgs e)
        {
            var c = sender as Compiler;
            message(c.Message);
        }

        void c_OnError(object sender, EventArgs e)
        {
            message("Error compiling!");
            if (OnError != null) OnError(this, null);
            hasError = true;
        }

    }
}
