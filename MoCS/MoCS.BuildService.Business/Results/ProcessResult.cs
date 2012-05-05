using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business.Results
{
    public class ProcessResult
    {
        public ExitCode ResultCode { get; set; }
        public TestResults TestResults { get; set; }

        public ProcessResult()
        {
            TestResults = new TestResults();
        }
    }

    public enum ExitCode
    {
        Success,
        Failure
    }
}
