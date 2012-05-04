using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoCS.BuildService.Business.Processing;
using MoCS.BuildService.Business.Settings;
using MoCS.BuildService.Business.Results;

namespace MoCS.BuildService.Business.Tests
{
    [TestClass]
    public class MoCSValidationProcessTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            ProcessSettings settings = new ProcessSettings();

            BaseProcess process = new MoCSValidationProcess(settings);

            ExecuteProcessResult result = process.Process();


        }
    }
}
