using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ArduinoLibrary.SketchUploader
{
    static class Helpers
    {

       public static string ConvertPedToCpp(string code)
        {
            StringBuilder cppBuilder = new StringBuilder();
            string x = "";
            string program = "";
            int _idx = 0;

            program = "#include <Arduino.h>" + Environment.NewLine + code;

            Regex r = new Regex("(^\\s*#(?:\\\\\\n|.)*)", RegexOptions.Multiline);
            Match m = r.Match(program);

            while (m != null && m.Success)
            {
                x += m.Groups[0].Value;
                _idx = m.Index + m.Groups[0].Value.Length;
                //    Console.Out.Write(m.Groups[0].Value);
                m = m.NextMatch();
            }

            x = program.Substring(0, _idx);
            cppBuilder.AppendLine(program);

            // Create prototype function
            r = new Regex("^\\s*[\\w\\[\\]\\*]+\\s+[\\[\\]\\*\\w\\s]+\\([,\\[\\]\\*\\w\\s]*\\)(?=\\s*\\{)", RegexOptions.Multiline);
            m = r.Match(program);
            cppBuilder.Insert(_idx, "\r\n"); _idx += 2;
            while (m != null && m.Success)
            {
                string _str = m.Groups[0].Value + ";\n";
                cppBuilder.Insert(_idx, _str);
                _idx += _str.Length;
                m = m.NextMatch();
            }
            cppBuilder.Insert(_idx, "\r\n");
            //

            cppBuilder.AppendLine();
            cppBuilder.AppendLine("int main(void)");
            cppBuilder.AppendLine("{");
            cppBuilder.AppendLine("\tinit();");
            cppBuilder.AppendLine("\tsetup();");
            cppBuilder.AppendLine("\tfor (;;)");
            cppBuilder.AppendLine("\t\tloop();");
            cppBuilder.AppendLine("\treturn 0;");
            cppBuilder.AppendLine("}");
            cppBuilder.AppendLine();


            //            loadIncludeFile(program);

            return cppBuilder.ToString(); ;
        }

        public static IEnumerable<string> GetIncludeFiles(string code)
        {
            Regex r = new Regex("\\s*#include\\s+[<\"](\\S+)[\">]");
            Match m = r.Match(code);

            while (m != null && m.Success)
            {
                yield return m.Groups[1].Value;
                m = m.NextMatch();
            }
        }


        public static string RunProcess(string filename, string arguments)
        {
            return RunProcess(filename, arguments, false);
        }
        public static string RunProcess(string filename, string arguments, bool openInNewWindow)
        {

            Debug.WriteLine("----");
            Debug.WriteLine(filename + " " + arguments);

            System.IO.StreamReader sr;
            Process builder = new Process();

            builder.StartInfo.RedirectStandardOutput = true;
            builder.StartInfo.RedirectStandardError = true;

            builder.StartInfo.UseShellExecute = false;
            builder.StartInfo.CreateNoWindow = true;
            builder.StartInfo.WindowStyle = ProcessWindowStyle.Normal; // ProcessWindowStyle.Hidden;
            builder.StartInfo.FileName = filename;
            builder.StartInfo.Arguments = arguments;


            if (openInNewWindow)
            {
                builder.StartInfo.CreateNoWindow = false;
                builder.StartInfo.UseShellExecute = true;
                builder.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                builder.StartInfo.RedirectStandardOutput = false;
                builder.StartInfo.RedirectStandardError = false;
            }


            builder.Start();
            builder.WaitForExit();

            if (!openInNewWindow)
            {
                sr = builder.StandardOutput;
                string _strOutput = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(_strOutput))
                    Console.Out.WriteLine(_strOutput);

                sr = builder.StandardError;

                return sr.ReadToEnd();
            }
            return "";

        }

    }
}
