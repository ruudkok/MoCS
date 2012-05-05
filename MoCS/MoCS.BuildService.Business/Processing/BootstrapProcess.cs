using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MoCS.Business.Objects;
using MoCS.Business.Objects.Interfaces;
using System.IO;
using MoCS.BuildService.Business.Settings;
using MoCS.BuildService.Business.Results;

namespace MoCS.BuildService.Business.Processing
{
    public class BootstrapProcess : BaseProcess
    {
        public BootstrapProcess(ProcessSettings settings, IFileSystem fileSystem)
            : base(settings, fileSystem)
        {
        }

        public override ExecuteProcessResult Process()
        {
            return Bootstrap();
        }

        private ExecuteProcessResult Bootstrap()
        {
            Submit submit = Settings.Submit;

            ExecuteProcessResult result = new ExecuteProcessResult();
            result.ExitCode = 0;
            result.Result = ExitCode.Success;

            try
            {
                IFileSystem fileSystem = ServiceLocator.Instance.GetService<IFileSystem>();

                Assignment assignment = Settings.Assignment;
  
                string teamSubmitDirName = CreateTeamDirectory(Settings.Submit.Team.Id.ToString(), assignment);

                Settings.OutputPath = teamSubmitDirName;
                Settings.SourcesPath = teamSubmitDirName;

                CopyFiles(assignment, submit, teamSubmitDirName);
            }
            catch(Exception ex)
            {
                result.Output = ex.Message;
                result.Result = ExitCode.Failure;
            }

            return result;
        }

        public string CreateTeamDirectory(string teamName, Assignment assignment)
        {
            FileSystem.CreateDirectoryIfNotExists(Settings.BaseResultPath);

            //create a directory for the assignment
            FileSystem.CreateDirectoryIfNotExists(Settings.BaseResultPath + assignment.Id);

            string teamDirName = teamName + "_" + DateTime.Now.ToString("ddMMyyyy_HHmmss");
            string teamSubmitDirName = Settings.BaseResultPath + assignment.Id + @"\" + teamDirName;
           
            //create a new directory for the teamsubmit
            FileSystem.CreateDirectory(teamSubmitDirName);

            return teamSubmitDirName;
        }

        public void CopyFiles(Assignment assignment, Submit submit, string teamSubmitDirName)
        {
            IFileSystem fileSystem = ServiceLocator.Instance.GetService<IFileSystem>();

            // Copy nunit.framework.dll to this directory
            fileSystem.FileCopy(Path.Combine(Settings.NunitAssemblyPath, "nunit.framework.dll"),
                        Path.Combine(teamSubmitDirName, "nunit.framework.dll"), true);

            //copy the file to this directory
            CopySubmittedFileToDirectory(teamSubmitDirName, submit);

            // Copy the interface file//////////////////////////////////////////////////////////////////////////////
            //delete the file if it existed already
            DeleteAndCopySingleAssignmentFile("InterfaceFile", teamSubmitDirName, assignment);

            DeleteAndCopySingleAssignmentFile("NunitTestFileServer", teamSubmitDirName, assignment);

            DeleteAndCopySingleAssignmentFile("NunitTestFileClient", teamSubmitDirName, assignment);

            DeleteAndCopyMulitpleAssignmentFiles("ServerFileToCopy", teamSubmitDirName, assignment);
        }

        private void DeleteAndCopySingleAssignmentFile(string fileName, string teamSubmitDirName, Assignment assignment)
        {
            AssignmentFile file = assignment.AssignmentFiles.Find(af => af.Name == fileName);

            FileSystem.DeleteFileIfExists(Path.Combine(teamSubmitDirName, file.FileName));

            FileSystem.FileCopy(Path.Combine(assignment.Path, file.FileName),
                        Path.Combine(teamSubmitDirName, file.FileName));
        }

        private void DeleteAndCopyMulitpleAssignmentFiles(string fileName, string teamSubmitDirName, Assignment assignment)
        {
            List<AssignmentFile> files = assignment.AssignmentFiles.FindAll(af => af.Name == fileName);
            foreach (AssignmentFile file in files)
            {
                FileSystem.DeleteFileIfExists(Path.Combine(teamSubmitDirName, file.FileName));

                FileSystem.FileCopy(Path.Combine(assignment.Path, file.FileName),
                            Path.Combine(teamSubmitDirName, file.FileName));
            }
        }

        public void CopySubmittedFileToDirectory(string teamSubmitDirName, Submit submit)
        {
            //copy the file to this directory
            using (Stream target = FileSystem.FileOpenWrite(Path.Combine(teamSubmitDirName, submit.FileName)))
            {
                try
                {
                    target.Write(submit.Data, 0, submit.Data.Length);
                }
                finally
                {
                    target.Flush();
                }
            }
        }
    }
}
