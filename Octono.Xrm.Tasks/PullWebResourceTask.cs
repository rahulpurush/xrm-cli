﻿using System;
using System.IO;
using Microsoft.Xrm.Sdk;
using Octono.Xrm.Tasks.IO;

//TODO: Add support for retrieving all customizable resources
//TODO: Add support for path format resource names i.e new_/resource.js

namespace Octono.Xrm.Tasks
{
    public class PullWebResourceTask : IXrmTask
    {
        private readonly PullWebResourceCommandLine _commandLine;
        private readonly IFileWriter _writer;

        public PullWebResourceTask(PullWebResourceCommandLine commandLine, IFileWriter writer)
        {
            _commandLine = commandLine;
            _writer = writer;
            RequiresServerConnection = true;
        }

        public void Execute(IXrmTaskContext context)
        {
            var query       = new WebResourceQuery(context.Service);
            var entity      = query.Retrieve(_commandLine.Name);
            var content     = entity.GetAttributeValue<string>("content").FromBase64String();
            var optionset   = entity.GetAttributeValue<OptionSetValue>("webresourcetype");
            var type        = WebResourceType.ToFileExtension(optionset.Value);
            var filePath    = Path.Combine(_commandLine.Path, _commandLine.Name + type );
            
            _writer.Write(System.Text.Encoding.UTF8.GetBytes(content),filePath);
        }

        public bool RequiresServerConnection { get; private set; }
    }

    public static class WebResourceType
    {
        public static string ToFileExtension(int value)
        {
            switch (value)
            {
                case 1:
                    return ".htm";
                case 2:
                    return ".css";
                case 3:
                    return ".js";
                case 4:
                    return ".xml";
                case 5:
                    return ".png";
                case 6:
                    return ".jpg";
                case 7:
                    return ".gif";
                case 8:
                    return ".xap";
                case 9:
                    return ".xsl";
                case 10:
                    return ".ico";
                default:
                    throw new ArgumentOutOfRangeException(string.Format("The web resource type with value {0} is unknown.", value));
            }
        }
    }
}