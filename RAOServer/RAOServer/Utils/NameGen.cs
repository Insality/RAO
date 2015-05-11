using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace RAOServer.Utils {
    internal class NameGen {
        public static string GenLocation() {
            return _runPython(string.Format("{0} Location 1", Settings.NameGenScript));
        }

        private static string _runPython(string args) {
            string result;
            var start = new ProcessStartInfo();
            start.FileName = Settings.Python3Path;
            start.EnvironmentVariables.Add("PYTHONIOENCODING", "UTF-8");
            start.Arguments = args;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.StandardOutputEncoding = Encoding.GetEncoding("UTF-8");
            using (var process = Process.Start(start)){
                using (var reader = process.StandardOutput){
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}