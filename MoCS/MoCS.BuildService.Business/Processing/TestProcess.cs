using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using MoCS.BuildService.Business.Results;
using MoCS.BuildService.Business.Settings;
using MoCS.Business.Objects.Interfaces;

namespace MoCS.BuildService.Business.Processing
{
    public class TestProcess : BaseProcess
    {
        public TestProcess(ProcessSettings settings, IFileSystem fileSystem)
            : base(settings, fileSystem)
        {
        }

        public override ExecuteProcessResult Process()
        {
            TestResults testResults = RunUnitTests();

            ExecuteProcessResult result = new ExecuteProcessResult();

            result.Result = testResults.ResultCode;

            if (testResults.ResultCode == ExitCode.Failure)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string message in testResults.FailingTests)
                {
                    sb.AppendLine(message);
                }

                result.Output = sb.ToString();
            }
   
            return result;
        }

        private TestResults RunUnitTests()
        {
            ProcessStartInfo startInfo = null;
            startInfo = ProcessStartInfoFactory.CreateTestProcessStartInfoNunit(Settings);
            ExecuteProcessResult result = ExecuteProcess(startInfo);

            TestResults testResults = TestResultsInterpreter.InterpretTestResults(Settings.GetTestLogPath());
            
            if (testResults.FailingTests.Count == 0 && result.ExitCode == 0)
            {
                testResults.ResultCode = ExitCode.Success;
            }
            else
            {
                testResults.ResultCode = ExitCode.Failure;
            }

            return testResults;
        }
    }
}
