using System.Collections.Generic;
using MoCS.BuildService.Business.Results;
using MoCS.BuildService.Business.Settings;
using MoCS.Business.Objects.Interfaces;

namespace MoCS.BuildService.Business.Processing
{
    public class MoCSValidationProcess : BaseProcess
    {
        private List<BaseProcess> _processes;

        public MoCSValidationProcess(ProcessSettings settings, IFileSystem fileSystem)
            : base(settings, fileSystem)
        {
            InitializeProcesses();
        }

        /// <summary>
        /// Initializes a new instance of the ContinuousIntegrationClientProcess for testing purposes
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="fileSystem"></param>
        public MoCSValidationProcess(ProcessSettings settings, IFileSystem fileSystem, List<BaseProcess> processes) : base(settings, fileSystem)
        {
            _processes = processes;
            InitializeProcesses();
        }

        private void InitializeProcesses()
        {
            _processes = new List<BaseProcess>();
            _processes.Add(new BootstrapProcess(Settings, FileSystem));
            _processes.Add(new CompileProcess(Settings, FileSystem));
            _processes.Add(new BusinessValidationProcess(Settings, FileSystem));
            _processes.Add(new TestProcess(Settings, FileSystem));
            _processes.Add(new CleanUpProcess(Settings, FileSystem));
        }

        public override ExecuteProcessResult Process()
        {
            return Start();
        }

        private ExecuteProcessResult Start()
        {
            ExecuteProcessResult totalResult = new ExecuteProcessResult();

            foreach (BaseProcess process in _processes)
            {
                totalResult = process.Process();

                if (totalResult.Result == ExitCode.Failure)
                {
                    break;
                }
            }

            return totalResult;
        }
    }
}
