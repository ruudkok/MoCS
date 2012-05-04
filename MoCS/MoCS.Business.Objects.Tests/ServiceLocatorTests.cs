using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoCS.Business.Objects.Interfaces;

namespace MoCS.Business.Objects.Tests
{
    [TestClass]
    public class ServiceLocatorTests
    {
        [TestMethod]
        public void GetService_ServiceFilled_ReturnService()
        {
            ServiceLocator locator = ServiceLocator.Instance;
            locator.AddService(typeof(IFileSystem), new FileSystemWrapper());
            IFileSystem fs = locator.GetService<IFileSystem>();
            Assert.IsNotNull(fs);
        }

        [TestMethod]
        public void GetService_ReplaceService_ReturnService()
        {
            ServiceLocator locator = ServiceLocator.Instance;
            locator.AddService(typeof(IFileSystem), new FileSystemWrapper());
            locator.AddService(typeof(IFileSystem), new FileSystemWrapper());
            IFileSystem fs = locator.GetService<IFileSystem>();
            Assert.IsNotNull(fs);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void GetService_ServiceNotFound_Exception()
        {
            ServiceLocator locator = ServiceLocator.Instance;
            IFileSystem fs = locator.GetService<IFileSystem>();
        }
    }
}
