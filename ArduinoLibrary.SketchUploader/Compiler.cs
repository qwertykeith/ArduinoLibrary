using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ArduinoLibrary.SketchUploader
{
    /// <summary>
    /// Compiles, links and creates the image nessesary to upload an arduino sketch to a device
    /// thanks to http://arduinosketch.codeplex.com/ for inspiration and lots of code
    /// </summary>
    public class Compiler
    {

        public event EventHandler OnMessage;
        public event EventHandler OnError;
        public event EventHandler OnSuccess;

        public string Message { get; set; }

        void message(string m)
        {
            Message = m;
            if (OnMessage != null) OnMessage(this, null);
        }

        string runProcess(string filename, string arguments, bool stopOnMessage)
        {
            var m = Helpers.RunProcess(filename, arguments);
            if (m != "") message(m);
            if (stopOnMessage && (m != "")) throw new Exception(m);
            return m;
        }

        StringBuilder objLinker;

        UploadConfig info;
        public Compiler(UploadConfig info)
        {
            this.info = info;
        }

        public void Compile(string code)
        {
            message("Compiling");
            try
            {
                // make the temp directory to put all this stuff in
                if (!Directory.Exists(info.TempDir)) Directory.CreateDirectory(info.TempDir);

                // compile sketch 
                compile(code);

                if (OnSuccess != null) OnSuccess(this, null);
            }
            catch (Exception ex)
            {
                message("Error compiling! " + ex.Message);
                if (OnError != null) OnError(this, null);
            }
            message("Finished compiling");
        }

        void compile(string code)
        {
            var cppFilename = info.TempCppCodePath;
            var filesDir = info.TempDir + @"\";  // needs to be like this in the params

            message("Converting to cpp");
            // Create build output
            var cpp = Helpers.ConvertPedToCpp(code);
            // save
            File.WriteAllText(cppFilename, cpp);

            message("Compiling cpp");
            var includeFiles = Helpers.GetIncludeFiles(code).ToList();

            objLinker = new StringBuilder();

            string includeDirArgs = "-I" + info.IncludeFiles + " -I" + info.VariantDir + " -I" + filesDir;

            var cppFiles = new DirectoryInfo(filesDir).GetFiles().Where(p => p.Extension == ".cpp");
            //  Compile source file
            foreach (var cppFile in cppFiles)
            {
                message("gpp " + cppFile.Name);
                var objFileName = cppFile.FullName + ".o";
                runProcess(info.Gpp, StringConst.BuildSketchCommand(new object[] { info.Board.Mcu, info.Board.FCpu, includeDirArgs, cppFile.FullName, objFileName }), true);
                objLinker.Append(objFileName + " ");
            }

            // Compile custom library
            compileLib(includeDirArgs, filesDir, includeFiles);
            System.Threading.Thread.Sleep(1000);

            // Object linker
            message("Linking " + cppFilename);
            runProcess(info.Gcc, string.Format(StringConst.GPP_LINKER, new object[] { info.Board.Mcu, cppFilename, objLinker.ToString(), filesDir }), true);

            // Create flash image .hex
            message("Creating flash image");
            runProcess(info.Objcp, string.Format(StringConst.CREATE_FLASH_IMAGE, new object[] { cppFilename, cppFilename }), true);

            message("Creating eeprom image");
            // Create eeprom image .eep
            runProcess(info.Objcp, string.Format(StringConst.CREATE_EEPROM_IMAGE, new object[] { cppFilename, cppFilename }), true);

            // Show flash image size
            message("Computing image size");
            runProcess(info.Avrsize, string.Format(StringConst.PRINT_SIZE, new object[] { info.Board.Mcu, cppFilename }), false);

        }

        void compileLib(string strInclude, string applet, List<string> includeFiles)
        {

            var files = Directory.GetFiles(info.ArduCore)
                .Select(p => new FileInfo(p))
                ;

            var cFiles = files.Where(p => p.Extension == ".c");
            foreach (var f in cFiles)
            {
                message("gcc " + f.Name);
                var fname = applet + @"\" + f.Name + ".o";
                runProcess(info.Gcc, StringConst.BuildGccCommand(new object[] { info.Board.Mcu, info.Board.FCpu, strInclude, f.FullName, fname }), true);

            }

            var cppFiles = files.Where(p => p.Extension == ".cpp");
            foreach (var f in cppFiles)
            {
                message("gpp " + f.Name);
                var fname = applet + @"\" + f.Name + ".o";
                runProcess(info.Gpp, StringConst.BuildGccCommand(new object[] { info.Board.Mcu, info.Board.FCpu, strInclude, f.FullName, fname }), true);
            }

            var objectFileNames = cFiles.Concat(cppFiles)
                .Select(p => applet + @"\" + p.Name + ".o")
                .Aggregate((a, b) => a + " " + b);


            message("Linking");
            // Linker Library
            runProcess(info.Ar, StringConst.LinkingCommand(new object[] { applet, objectFileNames }), true);

            compileIncludeLib(strInclude, includeFiles);


        }

        /// <summary>
        /// needs a tidyup.. and not tested
        /// </summary>
        /// <param name="strInclude"></param>
        /// <param name="includeFiles"></param>
        void compileIncludeLib(string strInclude, List<string> includeFiles)
        {

            if (includeFiles.Count == 0) { return; }

            var libraryDirs = new DirectoryInfo(info.ArduinoLibDirectory).GetDirectories();
            FileInfo[] libFiles = null;
            var  objFileName = "";
            var libDirInfos = new List<DirectoryInfo>();
            foreach (var dir in libraryDirs)
            {
                libFiles = dir.GetFiles();
                foreach (var libFile in libFiles)
                {
                    includeFiles.ForEach(e => { if (libFile.Name.Equals(e)) { libDirInfos.Add(dir); } });
                }
            }

            libDirInfos.ForEach(e =>
            {
                libFiles = e.GetFiles();
                // Compile .c
                foreach (var libFile in libFiles)
                {
                    if (libFile.Extension.Equals(".c"))
                    {
                        message("gcc " + libFile.Name);
                        objFileName = libFile.FullName + ".o";
                        runProcess(info.Gcc, string.Format(StringConst.GCC_INCLUDE_LIB, new object[] { info.Board.Mcu, info.Board.FCpu, strInclude, libFile.FullName, objFileName }), true);
                        objLinker.Append(objFileName + " ");
                    }
                }
                // Compile .cpp
                foreach (var libFile in libFiles)
                {
                    if (libFile.Extension.Equals(".cpp"))
                    {
                        message("gpp " + libFile.Name);
                        objFileName = libFile.FullName + ".o";
                        runProcess(info.Gpp, string.Format(StringConst.GPP_INCLUDE_LIB, new object[] { info.Board.Mcu, info.Board.FCpu, strInclude, libFile.FullName, objFileName }), true);
                        objLinker.Append(objFileName + " ");
                    }
                }

                compileIncludeUtilityLib(e.FullName, strInclude);
            });
        }

        /// <summary>
        /// needs a tidyup.. and not tested
        /// </summary>
        /// <param name="utility"></param>
        /// <param name="strInclude"></param>
        void compileIncludeUtilityLib(string utility, string strInclude)
        {
            var utilityDir = string.Format(@"{0}\utility", utility);
            var objFileName = "";

            if (Directory.Exists(utilityDir))
            {
                var utilityFileInfo = new DirectoryInfo(utilityDir).GetFiles().ToList();
                utilityFileInfo.ForEach(e =>
                {
                    if (e.Extension.Equals(".c"))
                    {
                        message("gcc " + e.Name);
                        objFileName = e.FullName + ".o";
                        runProcess(info.Gcc, string.Format(StringConst.GCC_INCLUDE_LIB, new object[] { info.Board.Mcu, info.Board.FCpu, strInclude, e.FullName, objFileName }), true);
                        objLinker.Append(objFileName + " ");

                    }
                });
                utilityFileInfo.ForEach(e =>
                {
                    if (e.Extension.Equals(".cpp"))
                    {
                        message("gpp " + e.Name);
                        objFileName = e.FullName + ".o";
                        runProcess(info.Gpp, string.Format(StringConst.GCC_INCLUDE_LIB, new object[] { info.Board.Mcu, info.Board.FCpu, strInclude, e.FullName, objFileName }), true);
                        objLinker.Append(objFileName + " ");

                    }
                });
            }


        }


    }
}
