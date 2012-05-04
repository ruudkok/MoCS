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
    public class CompileProcess : BaseProcess
    {
        public CompileProcess(ProcessSettings settings, IFileSystem fileSystem)
            : base(settings, fileSystem)
        {
        }

        public override ExecuteProcessResult Process()
        {
            return Compile();
        }

        private ExecuteProcessResult Compile()
        {
            ProcessStartInfo startInfo = ProcessStartInfoFactory.CreateCompileProcessStartInfo(Settings);
            ExecuteProcessResult result = ExecuteProcess(startInfo);

            if (result.ExitCode == 0)
            {
                result.Result = ExitCode.Success;
            }
            else
            {
                result.Result = ExitCode.Failure;
            }

            return result;
        }
    }
}
