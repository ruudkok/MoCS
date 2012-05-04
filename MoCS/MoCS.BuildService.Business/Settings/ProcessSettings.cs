using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MoCS.Business.Objects;

namespace MoCS.BuildService.Business.Settings
{
    public class ProcessSettings
    {
        public string CscPath { get; set; }
        public string NunitAssemblyPath { get; set; }
        public string NunitConsolePath { get; set; }
        public int NunitTimeOut { get; set; }
        public string BaseResultPath { get; set; }
        public string PollingIntervalValue { get; set; }
        public string ProcessingTimeOut { get; set; }
        public Submit Submit { get; set; }
        public Assignment Assignment {get;set;}
        public string OutputPath { get; set; }
        public bool CleanUp { get; set; }
        public string SourcesPath { get; set; }

        public void Validate()
        {
            if (!File.Exists(NunitConsolePath))
            {
                throw new ApplicationException("nunit assembly is not found at " + NunitConsolePath);
            }

            if (!Directory.Exists(NunitAssemblyPath))
            {
                throw new ApplicationException("nunit path is not found at " + NunitAssemblyPath);
            }

            if (!Directory.Exists(BaseResultPath))
            {
                throw new ApplicationException("base result path is not found at " + BaseResultPath);
            }
        }

        public ProcessSettings Clone()
        {
            ProcessSettings settings = new ProcessSettings();
            settings.CscPath = this.CscPath;
            settings.NunitAssemblyPath = this.NunitAssemblyPath;
            settings.NunitConsolePath = this.NunitConsolePath;
            settings.NunitTimeOut = this.NunitTimeOut;
            settings.BaseResultPath = this.BaseResultPath;
            settings.PollingIntervalValue = this.PollingIntervalValue;
            settings.ProcessingTimeOut = this.ProcessingTimeOut;
            settings.OutputPath = this.OutputPath;
            settings.CleanUp = this.CleanUp;
            settings.SourcesPath = this.SourcesPath;
            settings.TestLogFileName = this.TestLogFileName;

            return settings;
        }

        public string GetTestLogPath()
        {
            string path = Path.Combine(OutputPath, TestLogFileName);

            if (path.EndsWith(@"\"))
            {
                path.Substring(0, path.Length - 1);
            }

            return path;
        }

        public string GetTestAssemblyPath(string assemblyName)
        {
            return Path.Combine(OutputPath, assemblyName);
        }

        public string MSTestPath { get; set; }
        public string TestLogFileName { get; set; }

        public string BasePath { get; set; }

        public void UpdateOutputPath(string subdir)
        {
            OutputPath = Path.Combine(BasePath, subdir);
            if (OutputPath.EndsWith(@"\"))
            {
                OutputPath.Substring(0, OutputPath.Length - 1);
            }
        }
    }
}
