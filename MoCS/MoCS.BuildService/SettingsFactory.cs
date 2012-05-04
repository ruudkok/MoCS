using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoCS.BuildService.Business;
using System.Configuration;
using MoCS.Business.Objects;
using MoCS.BuildService.Business.Settings;

namespace MoCS.BuildService
{
    public class SettingsFactory
    {
        public static ProcessSettings CreateProcessSettings()
        {
            ProcessSettings settings = new ProcessSettings();

            string cscPath = ConfigurationManager.AppSettings["CscPath"];
            string nunitAssemblyPath = ConfigurationManager.AppSettings["NunitAssemblyPath"];
            string nunitConsolePath = ConfigurationManager.AppSettings["NunitConsolePath"];

            nunitAssemblyPath = RemoveTrailingSlashFromPath(nunitAssemblyPath);
            nunitConsolePath = RemoveTrailingSlashFromPath(nunitConsolePath);

            settings.CscPath = cscPath;
            settings.NunitAssemblyPath = nunitAssemblyPath;
            settings.NunitConsolePath = nunitConsolePath;
            settings.NunitTimeOut = int.Parse(ConfigurationManager.AppSettings["ProcessingTimeOut"]);

            settings.BaseResultPath = ConfigurationManager.AppSettings["ResultBasePath"];

            settings.PollingIntervalValue = ConfigurationManager.AppSettings["PollingInterval"];
            settings.ProcessingTimeOut = ConfigurationManager.AppSettings["ProcessingTimeOut"];

            settings.NunitTimeOut = 5000;
            settings.CleanUp = false;

            settings.SourcesPath = "";   //will be filled during the process
            settings.OutputPath = "";    //will be filled during the process
            settings.TestLogFileName = "testresult.xml";

            if (!settings.BaseResultPath.EndsWith(@"\"))
            {
                settings.BaseResultPath += @"\";
            }

            return settings;
        }

        public static string RemoveTrailingSlashFromPath(string path)
        {
            if (path == null)
            {
                return null;
            }

            if (path.EndsWith(@"\"))
            {
                path = path.Substring(0, path.Length - 1);
            }

            return path;
        }
    }
}
