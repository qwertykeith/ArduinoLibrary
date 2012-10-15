using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace ArduinoLibrary.SketchUploader
{
    class AvrdudeUploader
    {
        SerialPort port;

        UploadConfig info;
        public AvrdudeUploader(UploadConfig info)
        {
            this.info = info;
            this.port = new SerialPort();
        }

        public void SetCurrentPortName(string portName)
        {
            port.PortName = portName;
        }

        public string uploadViaBootloader(string fileName)
        {
            List<string> commandDownloader = new List<string>();

            string protocol = info.Board.UploadProtocol;

            commandDownloader.Add("-c" + protocol);
            commandDownloader.Add("-C" + info.AvrdudeConfig);
            commandDownloader.Add("-P" + port.PortName);
            commandDownloader.Add("-b" + info.Board.UploadSpeed);
            if (protocol.Contains("stk500")) commandDownloader.Add("-D"); // don't erase
            commandDownloader.Add("-Uflash:w:" + fileName + ":i");

            Console.Out.WriteLine(fileName);

            return avrdude(commandDownloader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        string avrdude(IEnumerable<string> param)
        {

            List<string> commandDownloader = new List<string>();
            string avrdude = info.Avrdude; 
            commandDownloader.Add(info.VerboseOutput ? "-v" : "-q");
            commandDownloader.Add("-p" + info.Board.Mcu);

            commandDownloader.AddRange(param);

            return executeUploadCommand(avrdude, commandDownloader);
        }


        string executeUploadCommand(string filename, IEnumerable<string> param)
        {
            string args = param.Aggregate((a, b) => a + " " + b) + " ";

            return Helpers.RunProcess(filename, args, true);
        }


    }
}
