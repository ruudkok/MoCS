using MoCS.BuildService.Business.Results;
using MoCS.BuildService.Business.Settings;
using MoCS.Business.Objects.Interfaces;

namespace MoCS.BuildService.Business.Processing
{
    public class CleanUpProcess : BaseProcess
    {
        public CleanUpProcess(ProcessSettings settings, IFileSystem fileSystem)
            : base(settings, fileSystem)
        {
        }

        public override ExecuteProcessResult Process()
        {
            return CleanUp();
        }

        private ExecuteProcessResult CleanUp()
        {
            if (Settings.CleanUp)
            {
             //   fileSystem.DeleteDirectory(base._settings.OutputPath);
            }

            return new ExecuteProcessResult() { ExitCode = 0, Result = ExitCode.Success };
        }
    }
}
