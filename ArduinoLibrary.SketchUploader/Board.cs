using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ArduinoLibrary.SketchUploader
{
    /// <summary>
    /// represents one entry on the ardiouno boards.txt
    /// </summary>
    class Board
    {
        public Board(string boardText)
        {
            // extremely annoying file format

            var lines = Regex.Split(boardText, "\n")
                .Select(p => p.Trim())
                .Where(p => p != "")
                ;

            // very very first thing is the namespace
            // which we don't really care about
            var ns = lines.First().Split('.').First().Trim();


            foreach (var s in lines)
            {
                var field = s.Split('=').First().Replace(ns + ".", ""); // get rid of namepace
                var value = s.Split('=').Last().Trim();

                switch (field)
                {
                    case "name":
                        Name = value;
                        break;

                    case "upload.maximum_size":
                        UploadMaximum = value;
                        break;
                    case "upload.protocol":
                        // HACK becuase of bug in avrdude adruino code
                        // this is broken use "wiring"
                        // see http://code.google.com/p/arduino/issues/detail?id=806
                        // https://github.com/arduino/Arduino/commit/d83543cff15d8a1f4623b1e2babc6c2f4a867815
                        UploadProtocol = value == "stk500v2" ? "wiring" : value;
                        break;
                    case "upload.speed":
                        UploadSpeed = value;
                        break;
                    case "bootloader.unlock_bits":
                        BootloaderUnlockBit = value;
                        break;
                    case "bootloader.extended_fuses":
                        BootloaderExtenedFuses = value;
                        break;
                    case "bootloader.high_fuses":
                        BootloaderHighFuses = value;
                        break;
                    case "bootloader.low_fuses":
                        BootloaderLowFuses = value;
                        break;
                    case "bootloader.path":
                        BootLoaderPath = value;
                        break;
                    case "bootloader.lock_bits":
                        BootloaderLockBits = value;
                        break;
                    case "build.variant":
                        Variant = value;
                        break;
                    case "build.mcu":
                        Mcu = value;
                        break;
                    case "build.f_cpu":
                        FCpu = value;
                        break;
                    case "build.core":
                        Core = value;
                        break;
                   
                    default:
                        break;
                }
            }
        }

        public string Name { get; private set; }
        public string UploadMaximum { get; private set; }
        public string UploadProtocol { get; set; }
        public string UploadSpeed { get; set; }
        public string BootloaderUnlockBit { get; set; }
        public string BootloaderExtenedFuses { get; set; }
        public string BootloaderHighFuses { get; set; }
        public string BootloaderLowFuses { get; set; }
        public string BootLoaderPath { get; set; }
        public string BootloaderLockBits { get; set; }
        public string Variant { get; set; }

        public string Mcu { get; set; }
        public string FCpu { get; set; }
        public string Core { get; set; }
        public string AvrdudeMcu { get { return Mcu.Replace("atmega", " ").Trim(); } }


    }
}
