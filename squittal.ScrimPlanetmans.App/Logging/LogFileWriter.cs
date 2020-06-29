using System;
using System.IO;

namespace squittal.ScrimPlanetmans.Logging
{
    public class LogFileWriter
    {
        public static bool WriteToLogFile(string fileName, string logMessage)
        {
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var matchLogsDirectory = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..\\match_logs"));

            try
            {
                FileStream fileStream = new FileStream($"{matchLogsDirectory}\\{fileName}", FileMode.Append, FileAccess.Write);
            
                StreamWriter streamWriter = new StreamWriter(fileStream);
            
                streamWriter.WriteLine(logMessage);
            
                streamWriter.Close();
                fileStream.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
