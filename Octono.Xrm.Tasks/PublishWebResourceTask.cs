﻿using System;
using Microsoft.Crm.Sdk.Messages;

namespace Octono.Xrm.Tasks
{
    public class PublishWebResourceTask : XrmTask
    {
        private readonly Guid _id;

        public PublishWebResourceTask(Guid id)
        {
            _id = id;
        }

        public override void Execute(IXrmTaskContext context)
        {
            //Publish the change
            context.Log.Write("Publishing");
            var publish = new PublishXmlRequest();
            string webResourceXml = string.Format("<importexportxml><webresources><webresource>{0}</webresource></webresources></importexportxml>", _id);

            publish.ParameterXml = webResourceXml;
            context.Service.Execute(publish);

        }
    }
}