using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace squittal.ScrimPlanetmans.Logging
{
    public class SqlScriptFileHandler
    {
        public static IEnumerable<string> GetAdHocSqlFileNames()
        {
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var adhocScriptDirectory = Path.GetFullPath(Path.Combine(basePath, "..", "..", "..", "..\\sql_adhoc"));

            var scripts = new List<string>();

            try
            {
                var files = Directory.GetFiles(adhocScriptDirectory);

                foreach (var file in files)
                {
                    if (!file.EndsWith(".sql"))
                    {
                        continue;
                    }

                    scripts.Add(Path.GetFileName(file));
                }

                return scripts.OrderBy(f => f).ToList();
            }
            catch
            {
                // Ignore
                return null;
            }
        }
    }
}
