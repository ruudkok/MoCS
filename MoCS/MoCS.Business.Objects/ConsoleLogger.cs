using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoCS.Business.Objects.Interfaces;

namespace MoCS.Business.Objects
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.Out.WriteLine(DateTime.Now.ToLongTimeString() + " " + message);
        }
    }
}
