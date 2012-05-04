using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using MoCS.Business.Facade;
using MoCS.Business.Objects;
using MoCS.BuildService.Business;
using MoCS.Business.Objects.Interfaces;
using MoCS.BuildService.Business.Settings;

namespace MoCS.BuildService
{
    class Program
    {
        private static SubmitWatcher _watcher;

        static void Main(string[] args)
        {
            //setup the servicelocator
            ServiceLocator locator = ServiceLocator.Instance;
            locator.AddService(typeof(IFileSystem), new FileSystemWrapper());
            locator.AddService(typeof(ProcessSettings), SettingsFactory.CreateProcessSettings());
            locator.AddService(typeof(IBuildServiceFacade), new ClientFacade());
            locator.AddService(typeof(ILogger), new ConsoleLogger());

            bool everythingOk = true;
            try
            {
                locator.GetService<ProcessSettings>().Validate();
            }
            catch (Exception ex)
            {
                locator.GetService<ILogger>().Log(ex.Message);
                everythingOk = false;
            }

            if (everythingOk)
            {
                _watcher = new SubmitWatcher();
                Console.WriteLine("Start watching for submits to process.. Press enter to quit.");
                _watcher.StartWatching();
            }

            Console.ReadLine();
        }
    }
}
