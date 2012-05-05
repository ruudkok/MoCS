using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.BuildService.Business.Results
{
    public class ValidationResultxxx
    {
        public SubmitStatusCode Status { get; set; }
        public List<string> Messages { get; private set; }

        public ValidationResultxxx()
        {
            Status = SubmitStatusCode.Unknown;
            Messages = new List<string>();
        }
    }
}
