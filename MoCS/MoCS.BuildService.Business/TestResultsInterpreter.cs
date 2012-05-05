using System;
using System.IO;
using System.Xml;
using MoCS.BuildService.Business.Results;

namespace MoCS.BuildService.Business
{
    public class TestResultsInterpreter
    {
        public static TestResults InterpretTestResultsMSTest(XmlDocument doc)
        {
            TestResults results = new TestResults();
            XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
            nsm.AddNamespace("n", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

            results.Outcome = doc.SelectSingleNode("n:TestRun/n:ResultSummary", nsm).Attributes["outcome"].Value;
            results.CountExecuted = int.Parse(doc.SelectSingleNode("n:TestRun/n:ResultSummary/n:Counters", nsm).Attributes["executed"].Value);
            results.CountPassed = int.Parse(doc.SelectSingleNode("n:TestRun/n:ResultSummary/n:Counters", nsm).Attributes["passed"].Value);
            results.CountFailed = int.Parse(doc.SelectSingleNode("n:TestRun/n:ResultSummary/n:Counters", nsm).Attributes["failed"].Value);

            XmlNodeList testresults = doc.SelectNodes("n:TestRun/n:Results/n:UnitTestResult[@outcome='Failed']", nsm);
            string names = string.Empty;
            foreach (XmlNode testresult in testresults)
            {
                string message = testresult.Attributes["testName"].Value;
                XmlNode xmlErrorInfoNode = testresult.SelectSingleNode("n:Output/n:ErrorInfo/n:Message", nsm);
                if (xmlErrorInfoNode != null)
                {
                    message += ": " + xmlErrorInfoNode.InnerText;
                    results.ResultCode = ExitCode.Failure;
                }

                results.FailingTests.Add(message);
            }

            return results;
        }

        public static TestResults InterpretTestResultsNUnit(XmlDocument doc)
        {
            TestResults results = new TestResults();

            results.Outcome = doc.SelectSingleNode("test-results/test-suite").Attributes["result"].Value;
            results.CountExecuted = int.Parse(doc.SelectSingleNode("test-results").Attributes["total"].Value);
            results.CountFailed = int.Parse(doc.SelectSingleNode("test-results").Attributes["failures"].Value);
            results.CountPassed = results.CountExecuted - results.CountFailed;

            XmlNodeList testresults = doc.SelectNodes("//test-case[@success='False']");
            
            string message = string.Empty;
            string name = "";
            foreach (XmlNode testresult in testresults)
            {
                name = testresult.Attributes["name"].Value;
                message += "TEST: " +  name;
                XmlNodeList failuresList = testresult.SelectNodes("failure/message");

                foreach (XmlNode errormessage in failuresList)
                {
                    //testname + outcome
                    message += ": " + errormessage.InnerText;
                    message += Environment.NewLine;
                }

                results.FailingTests.Add(message);
            }

            results.ResultCode = ExitCode.Success;
            if(results.CountFailed > 0)
            {
                results.ResultCode = ExitCode.Failure;
            }

            return results;
        }

        public static TestResults InterpretTestResults(string path)
        {
            TestResults results = new TestResults();
            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                results = InterpretTestResultsNUnit(doc);
            }

            return results;
        }
    }
}
