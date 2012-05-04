using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business.Results
{
    public class SubmitResultxxx
    {
        public SubmitStatusCode Status { get; set; }
        public List<string> Messages { get; private set; }

        public SubmitResultxxx()
        {
            Status = SubmitStatusCode.Unknown;
            Messages = new List<string>();
        }
    }
}
