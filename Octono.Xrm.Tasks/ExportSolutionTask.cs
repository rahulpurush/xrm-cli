﻿using System;
using System.IO;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Octono.Xrm.Tasks.IO;

namespace Octono.Xrm.Tasks
{
    public class ExportSolutionTask : IXrmTask
    {
        private readonly ExportSolutionCommandLine _command;
        private readonly IFileWriter _writer;

        public ExportSolutionTask(ExportSolutionCommandLine command, IFileWriter writer)
        {
            _command = command;
            _writer = writer;
        }

        public void Execute(IXrmTaskContext context)
        {
            if (ShowHelp(context.Log)) return;

            foreach (var solution in _command.SolutionNames)
            {
                if (_command.IncrementVersionBeforeExport)
                {
                    var incrementVersionTask = new IncrementSolutionVersionTask(solution);
                    incrementVersionTask.Execute(context);
                }

                string version = GetSolutionVersionNumber(solution,context.Service);
                String path = _command.BuildExportPath(solution, version);

                context.Log.Write(string.Format("Exporting {0} to {1}", solution, path));
                var response = (ExportSolutionResponse)context.Service.Execute(new ExportSolutionRequest()
                {
                    SolutionName = solution,
                    Managed = _command.Managed
                    
                });

                _writer.Write(response.ExportSolutionFile,path );
                context.Log.Write(string.Format("{0} exported successfully",solution));
            }
        }
        public bool RequiresServerConnection { get { return true; } }

        private bool ShowHelp(ILog log)
        {
            if (_command.ShowHelp)
            {
                log.Write("Usage");
                log.Write(@"export solutionname x:\path\to\solutionname.zip");
                log.Write(@"export solutionname1,solutionname2 x:\path\to\exports\folder");
                log.Write(@"export solutionname1,solutionname2");
                log.Write("\tExports listed solutions in the Exports folder");
                log.Write("Switches");
                log.Write("-m Managed");
                return true;
            }
            return false;
        }

        private string GetSolutionVersionNumber(string solution, IOrganizationService service)
        {
            using (var context = new OrganizationServiceContext(service))
            {
                try
                {
                    return context.CreateQuery("solution")
                                  .First(x => x.GetAttributeValue<string>("uniquename") == solution)
                                  .GetAttributeValue<string>("version");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }
    }
}
