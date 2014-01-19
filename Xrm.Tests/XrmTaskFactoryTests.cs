﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Moq;
using xrm;

namespace Xrm.Tests
{
    [TestClass]
    public class XrmTaskFactoryTests
    {
        [TestMethod]
        public void WhenCommandIsImport_ReturnsImportSolutionTask()
        {
            var factory = new XrmTaskFactory(new Mock<IFileReader>().Object, new Mock<IOrganizationService>().Object,new ConsoleLogger());
            IXrmTask task = factory.CreateTask(new[] { "import", "filename" });
            Assert.IsInstanceOfType(task, typeof(ImportSolutionTask));
        }


        [TestMethod]
        public void WhenCommandIsExport_ReturnsExportSolutionTask()
        {
            var factory = new XrmTaskFactory(new Mock<IFileReader>().Object, new Mock<IOrganizationService>().Object,new ConsoleLogger());
            IXrmTask task = factory.CreateTask(new[] { "export", "filename" });
            Assert.IsInstanceOfType(task, typeof(ExportSolutionTask));            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenValidCommandIsNotSpecifiedThrowsException()
        {
            var factory = new XrmTaskFactory(new Mock<IFileReader>().Object, new Mock<IOrganizationService>().Object, new ConsoleLogger());
            factory.CreateTask(new[] { "invalidcommand" });
        }
        [TestMethod]
        public void AcceptsOrganizationServiceInConstructor()
        {
            var factory = new XrmTaskFactory(new Mock<IFileReader>().Object,
                                             new Mock<IOrganizationService>().Object, new ConsoleLogger());

            Assert.IsNotNull(factory);
        }

    }
}
