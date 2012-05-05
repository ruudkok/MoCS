using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MoCS.BuildService.Business.Settings;
using MoCS.BuildService.Business.Results;
using MoCS.Business.Objects.Interfaces;

namespace MoCS.BuildService.Business.Processing
{
    public class BaseProcess
    {
        protected ProcessSettings Settings;
        protected IFileSystem FileSystem;

        /// <summary>
        /// Initializes a new instance of the BaseProcess class
        /// </summary>
        /// <param name="settings"></param>
        public BaseProcess(ProcessSettings settings, IFileSystem fileSystem)
        {
            Settings = settings;
            FileSystem = fileSystem;
        }

        public virtual ExecuteProcessResult Process()
        {
            return new ExecuteProcessResult();
        }

        protected virtual ExecuteProcessResult ExecuteProcess(ProcessStartInfo psi)
        {
            ExecuteProcessResult result = new ExecuteProcessResult();
            result.ExitCode = -99;
            
            try
            {
                    psi.RedirectStandardOutput = true;
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;

                    using (Process exeProcess = System.Diagnostics.Process.Start(psi))
                    {
                        exeProcess.WaitForExit();
                        result.Output = exeProcess.StandardOutput.ReadToEnd();
                        exeProcess.WaitForExit();
                        result.ExitCode = exeProcess.ExitCode;
                    }
            }
            catch 
            { 
            }

            return result;
        }
    }
}
