using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MoCS.Business.Objects;
using NSubstitute;
using MoCS.Business.Objects.Interfaces;
using MoCS.BuildService.Business;
using System.IO;
using MoCS.BuildService.Business.Settings;
using MoCS.BuildService.Business.Processing;
using MoCS.BuildService.Business.Results;
using MoCS.Business.Facade;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadFileIntoText();
        }

        private void LoadFileIntoText()
        {
            string text = File.ReadAllText((@"C:\mocs\submits\1\goodsolution.cs"));
            this.richTextBox2.Text = text;
        }

        public Submit CreateTestSubmit()
        {
            Submit submit = new Submit();
            submit.FileName = "HelloWorld.cs";
            submit.Data = File.ReadAllBytes(@"C:\mocs\submits\1\goodsolution.cs");
            submit.Team = new Team() { Name = "Team" };
            submit.TournamentAssignment = new TournamentAssignment();

            return submit;
        }

        private Assignment CreateTestAssignment(IBuildServiceFacade facade)
        {
            //setup the mock for facade
            Assignment assignment = new Assignment();
            facade.GetAssignmentById(Arg.Any<int>(), true).Returns(assignment);
            assignment.Path = @"c:\mocs\assignments\1\";
            assignment.AssignmentFiles.Add(new AssignmentFile() { Name = "InterfaceFile", FileName = "IHelloWorld.cs" });
            assignment.AssignmentFiles.Add(new AssignmentFile() { Name = "NunitTestFileServer", FileName = "HelloWorldTestsServer.cs" });
            assignment.AssignmentFiles.Add(new AssignmentFile() { Name = "NunitTestFileClient", FileName = "HelloWorldTests.cs" });
            assignment.AssignmentFiles.Add(new AssignmentFile() { Name = "ServerFileToCopy", FileName = "serverfile1.xml" });
            assignment.AssignmentFiles.Add(new AssignmentFile() { Name = "ServerFileToCopy", FileName = "serverfile2.dll" });
            assignment.Id = 1;
            assignment.ClassNameToImplement = "HelloWorld";
            assignment.InterfaceNameToImplement = "IHelloWorld";

            return assignment;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var logger = Substitute.For<ILogger>();
            var facade = Substitute.For<IBuildServiceFacade>();
            var filesys = new FileSystemWrapper();

            ProcessSettings processSettings = new ProcessSettings();
            processSettings.BaseResultPath = @"c:\mocs\";
            processSettings.NunitAssemblyPath = @"C:\Program Files\NUnit 2.5.9\bin\net-2.0\framework";
            processSettings.CscPath = @"C:\WINDOWS\Microsoft.NET\Framework\v3.5\csc.exe";
            processSettings.NunitConsolePath = @"C:\Program Files\NUnit 2.5.9\bin\net-2.0\nunit-console.exe";

            processSettings.SourcesPath = "";
            processSettings.OutputPath = "";
            processSettings.TestLogFileName = "testresult.xml";
            processSettings.NunitTimeOut = 5000;
            processSettings.CleanUp = true;

            processSettings.Assignment = CreateTestAssignment(facade);

            processSettings.Submit = CreateTestSubmit();
            processSettings.Submit.TournamentAssignment.Assignment = processSettings.Assignment;

            ServiceLocator services = ServiceLocator.Instance;
            services.AddService(typeof(IFileSystem), filesys);

            BaseProcess process = new MoCSValidationProcess(processSettings, filesys);
            ExecuteProcessResult result = process.Process();

            richTextBox1.Clear();
            if (result.Result == ExitCode.Success)
            {
                this.panel1.BackColor = Color.Green;
            }
            else
            {
                this.panel1.BackColor = Color.Red;
                richTextBox1.AppendText(result.Output);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string textToSubmit = this.richTextBox2.Text;

            ClientFacade facade = new ClientFacade();

            Team team = new Team() {Id=1, Name="Team01", Password="Team01"};
            
            team = facade.AuthenticateTeam(team);

            Submit submit = new Submit();
            submit.Team = team;
            submit.Data = new UTF8Encoding().GetBytes(textToSubmit);
            submit.AssignmentEnrollment = new AssignmentEnrollment() {Id=1, Team = team};
            submit.TournamentAssignment = new TournamentAssignment() { Id = 1 };
            submit.SubmitDate = DateTime.Now;
            submit.FileName = "HelloWorld.cs";

            facade.SaveSubmit(submit);
        }
    }

    public class MemoryTestStream : Stream
    {
        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        public override void Flush()
        {
        }

        public override bool CanRead
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}
