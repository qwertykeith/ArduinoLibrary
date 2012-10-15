using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoLibrary.SketchUploader
{
    /// <summary>
    /// thanks to http://arduinosketch.codeplex.com/
    /// </summary>
    public static class StringConst
    {
        public const string PRINT_SIZE = "--format=avr --mcu={0} {1}.elf";

    
        /// <summary>
        /// 0=mcu,
        /// 1=df_cpu
        /// 2=inclide directory
        /// 3=file name
        /// 4=object file name
        /// </summary>
        public const string GCC_INCLUDE_LIB =@"-c -g -Os -w -fno-exceptions -ffunction-sections -fdata-sections -mmcu={0} -DF_CPU={1} {2} {3} -o{4}";

        /// <summary>
        /// 0=mcu,
        /// 1=df_cpu
        /// 2=inclide directory
        /// 3=file name
        /// 4=object file name
        /// </summary>
        public const string GPP_INCLUDE_LIB =@"-c -g -Os -w -ffunction-sections -fdata-sections -mmcu={0} -DF_CPU={1} {2} {3} -o{4}";

        public const string GPP_LINKER ="-Os -Wl,--gc-sections -mmcu={0} -o{1}.elf {2} -L{3} -lcore -lm";

        /// <summary>
        /// 0-file name,
        /// 1=file name
        /// </summary>
        public const string CREATE_FLASH_IMAGE ="-O ihex -j .eeprom --set-section-flags=.eeprom=alloc,load --no-change-warnings --change-section-lma .eeprom=0 {0}.elf {1}.eep ";

        /// <summary>
        /// 0-file name,
        /// 1=file name
        public const string CREATE_EEPROM_IMAGE ="-O ihex -R .eeprom {0}.elf  {1}.hex ";

        /// <summary>
        /// 0=mcu 
        /// 1=df_cpu 
        /// 2=arduino path 
        /// 3=file name
        /// 4=applet path
        /// 5=file name
        /// </summary>
        const string STD_ARDUINO_LIB_GCC = @"-c -g -Os -w -ffunction-sections -fdata-sections -mmcu={0} -DF_CPU={1} {2} {3} -o{4}";

        public static string BuildGccCommand(object[] param)
        {
            return string.Format(STD_ARDUINO_LIB_GCC, param);
        }


        /// <summary>
        /// 0=applet path, 
        /// 1=applet path
        /// </summary>
        public const string STD_LINKING_LIB = "rcs {0}libcore.a {1}";

        public static string LinkingCommand(object[] param)
        {
            return string.Format(StringConst.STD_LINKING_LIB, param);
        }


        /// <summary>
        /// 0=mcu, 
        /// 1=df_cpu, 
        /// 2=include path, 
        /// 3=file name, 
        /// 4=filename
        /// </summary>
        public const string BUILD_SKETCH ="-c -g -Os -w -fno-exceptions -ffunction-sections -fdata-sections -mmcu={0} -DF_CPU={1} {2} {3} -o{4}";

        public static string BuildSketchCommand(object[] param)
        {
            return string.Format(StringConst.BUILD_SKETCH, param);
        }

       
    }
}
