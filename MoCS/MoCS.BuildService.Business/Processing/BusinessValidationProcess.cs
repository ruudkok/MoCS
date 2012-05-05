using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MoCS.Business.Objects.Interfaces;
using MoCS.Business.Objects;
using System.IO;
using System.Reflection;
using MoCS.BuildService.Business.Settings;
using MoCS.BuildService.Business.Results;

namespace MoCS.BuildService.Business.Processing
{
    public class BusinessValidationProcess : BaseProcess
    {
        public BusinessValidationProcess(ProcessSettings settings, IFileSystem fileSystem)
            : base(settings, fileSystem)
        {
        }

        public override ExecuteProcessResult Process()
        {
            return CheckBusinessRules(Path.Combine(Settings.OutputPath, "result.dll"));
        }

        private ExecuteProcessResult CheckBusinessRules(string outputDllPath)
        {
            ExecuteProcessResult result = new ExecuteProcessResult();

            //Compilation is successfull. Now check businessrules.
            //check if the assembly is found;
            if (!FileSystem.FileExists(outputDllPath))
            {
                result.Result = ExitCode.Failure;
                result.Output = "compiled dll was not found.";
                return result;
            }

            Type implementedClass = null;

            Assembly assembly = FileSystem.LoadAssembly(outputDllPath);

            //loop through the types in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                //try to find the class that was implemented 
                if (type.Name.Equals(Settings.Assignment.ClassNameToImplement)) 
                {
                    implementedClass = type;
                    break;
                }
            }

            //if the classToImplement cannot be found, return with an error
            if (implementedClass == null)
            {
                result.Result = ExitCode.Failure;
                result.Output = string.Format("The class to implement ({0}) is not found", Settings.Assignment.ClassNameToImplement);
                return result;
            }

            //check to see if it implements the required interface...
            Type requiredInterface = implementedClass.GetInterface(Settings.Assignment.InterfaceNameToImplement);
            if (requiredInterface == null)
            {
                result.Result = ExitCode.Failure;
                result.Output = string.Format("The class to implement ({0}) does not implement the required interface {1}", Settings.Assignment.ClassNameToImplement, Settings.Assignment.InterfaceNameToImplement);
                return result;
            }

            return result;
        }
    }
}
