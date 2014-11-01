﻿using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Moq;
using Octono.Xrm.Tasks;
using Octono.Xrm.Tasks.IO;

namespace Octono.Xrm.Tests
{
    [TestClass]
    public class ExportSolutionTaskTests
    {
        [TestMethod]
        public void ExportsAllSolutionsProvidedOnCommandLine()
        {
            var command = new ExportSolutionCommandLine(new[] {"export", "sol1,sol2"});
            var writer = new Mock<IFileWriter>();
            var service = new Mock<IOrganizationService>();
            var context = new Mock<IXrmTaskContext>();
            context.Setup(x => x.Service).Returns(service.Object);
            service.Setup(x => x.Execute(It.IsAny<OrganizationRequest>())).Returns(new ExportSolutionResponse());
            context.Setup(x => x.Log).Returns(new Mock<ILog>().Object);
            var task = new ExportSolutionTask(command, writer.Object);
            writer.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<string>())).Callback<byte[], string>((x, y) => Console.WriteLine(y));
            task.Execute(context.Object);
            writer.Verify(x=>x.Write(It.IsAny<byte[]>(),It.IsAny<string>()),Times.Exactly(2));
        }
    }
}
