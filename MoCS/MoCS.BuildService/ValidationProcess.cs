using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MoCS.Business.Objects;
using MoCS.BuildService.Business.Processing;
using MoCS.BuildService.Business.Settings;
using MoCS.BuildService.Business.Results;
using MoCS.Business.Objects.Interfaces;

namespace MoCS.BuildService
{
    public class ValidationProcess
    {
        public Submit Submit { get; set; }
        public Thread Thread { get; private set; }
        public ExecuteProcessResult Result {get; private set;}

        public ValidationProcess(Submit submit, DateTime startTime)
        {
            this.Submit = submit;
            this.ProcessingDate = startTime;
        }

        public bool IsReady()
        {
            return(Result != null);
        }

        public DateTime ProcessingDate { get; private set; }

        public bool CheckForTimeOut(DateTime checkMoment, int maxRunningMilliseconds)
        {
            TimeSpan span = checkMoment.Subtract(this.ProcessingDate);
            if(span.TotalMilliseconds > maxRunningMilliseconds)
            {
                if(this.Result == null)
                {
                    this.Result = new ExecuteProcessResult();
                }

                this.Result.Result = ExitCode.Failure;
                this.Result.Output = "Timeout - it took more than " + maxRunningMilliseconds;
                return true;
            }

            return false;
        }

        public void SaveStatusToDatabase()
        {
            string details = "";
            
            if(!string.IsNullOrEmpty(this.Result.Output))
            {
                details = this.Result.Output;
            }

            if (details.Length >= 1000)
            {
                details.Substring(0, 999);
            }

            IBuildServiceFacade facade = ServiceLocator.Instance.GetService<IBuildServiceFacade>();

            switch (Result.Result)
            {
                case ExitCode.Failure:
                    facade.UpdateSubmitStatusDetails(Submit.Id, SubmitStatus.ErrorValidation, details, DateTime.Now);
                    break;
                case ExitCode.Success:
                    facade.UpdateSubmitStatusDetails(Submit.Id, SubmitStatus.Success, details, DateTime.Now);
                    break;
            }   
        }

        public void SetThread(Thread thread)
        {
            this.Thread = thread;
        }

        public void Process()
        {
            IBuildServiceFacade facade = ServiceLocator.Instance.GetService<IBuildServiceFacade>();
            
            ProcessSettings settings = ServiceLocator.Instance.GetService<ProcessSettings>().Clone();

            settings.Submit = this.Submit;

            settings.Assignment = facade.GetAssignmentById(this.Submit.TournamentAssignment.Assignment.Id, true);
            this.Submit.TournamentAssignment.Assignment = settings.Assignment;

            facade.UpdateSubmitStatusDetails(Submit.Id, SubmitStatus.Processing, "submit is being processed", DateTime.Now);

            IFileSystem fileSystem = ServiceLocator.Instance.GetService<IFileSystem>();
            MoCSValidationProcess process = new MoCSValidationProcess(settings, fileSystem);
            ExecuteProcessResult result = process.Process();

            this.Result = result;
            SaveStatusToDatabase();
        }
    }
}
