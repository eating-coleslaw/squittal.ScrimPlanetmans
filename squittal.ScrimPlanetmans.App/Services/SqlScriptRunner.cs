using System;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Extensions.Logging;

namespace squittal.ScrimPlanetmans.Services
{
    public class SqlScriptRunner : ISqlScriptRunner
    {
        private readonly string _sqlDirectory = "Data/SQL";
        private readonly string _basePath;
        private readonly string _scriptDirectory;

        private readonly Server _server = new Server("(LocalDB)\\MSSQLLocalDB");

        private readonly ILogger<SqlScriptRunner> _logger;

        public SqlScriptRunner(ILogger<SqlScriptRunner> logger)
        {
            _logger = logger;

            _basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            _scriptDirectory = Path.Combine(_basePath, _sqlDirectory);
        }

        public void RunSqlScript(string fileName)
        {
            var scriptPath = Path.Combine(_scriptDirectory, fileName);

            var scriptFileInfo = new FileInfo(scriptPath);

            string scriptText = scriptFileInfo.OpenText().ReadToEnd();

            _server.ConnectionContext.ExecuteNonQuery(scriptText);
        }

        public void RunSqlDirectoryScripts(string directoryName)
        {
            var directoryPath = Path.Combine(_scriptDirectory, directoryName);

            try
            {
                var files = Directory.GetFiles(directoryPath);

                foreach (var file in files)
                {
                    if (!file.EndsWith(".sql"))
                    {
                        continue;
                    }

                    RunSqlScript(file);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error running SQL scripts in directory {directoryName}: {ex}");
            }
        }
    }
}
