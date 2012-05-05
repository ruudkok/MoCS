using System.Diagnostics;
using System.IO;
using MoCS.BuildService.Business.Settings;

namespace MoCS.BuildService.Business
{
    public class ProcessStartInfoFactory
    {
        public static ProcessStartInfo CreateCompileProcessStartInfo(ProcessSettings settings)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"""" + settings.CscPath + @"""";
            startInfo.Arguments = "/out:" + @"""" + Path.Combine(settings.OutputPath, "result.dll") + @"""";
            startInfo.Arguments += " /nologo";
            startInfo.Arguments += " /lib:" + @"""" + settings.NunitAssemblyPath + @"""" + " /reference:NUnit.Framework.dll";
            startInfo.Arguments += " /target:library";
            startInfo.Arguments += " " + @"""" + Path.Combine(settings.SourcesPath,"*.cs") + @"""";
            return startInfo;
        }

        public static ProcessStartInfo CreateTestProcessStartInfoNunit(ProcessSettings settings)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"""" + settings.NunitConsolePath + @"""";
            startInfo.Arguments += " " + @"""" + Path.Combine(settings.OutputPath, "result.dll") + @"""";
            startInfo.Arguments += " /timeout=" + settings.NunitTimeOut.ToString();
            startInfo.Arguments += " /nologo";
            startInfo.Arguments += " /xml=" + @"""" +  settings.GetTestLogPath() + @"""";
            return startInfo;
        }
    }
}
