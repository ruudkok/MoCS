using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business.Results
{
    public class ExecuteProcessResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; }
        public ExitCode Result { get; set; }
    }
}
