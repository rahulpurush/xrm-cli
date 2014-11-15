﻿using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Octono.Xrm.ConsoleTaskRunner;
using Octono.Xrm.Tasks;
using Octono.Xrm.Tasks.IO;

namespace Octono.Xrm.Tests
{

    [TestClass]
    public class ServerConnectionTests
    {
        private Mock<IConfigurationManager> CreateConfig()
        {
            var config = new Mock<IConfigurationManager>();
            config.Setup(x => x.AppSettings).Returns(new NameValueCollection());
            config.Setup(x => x.ConnectionStrings).Returns(new Dictionary<string,ServerConnection>());
            return config;
        }
        private ServerConnection CreateServerConnection(string[] args)
        {
            return new ServerConnection(args,new ConsoleLogger(), CreateConfig().Object);
        }

        [TestMethod]
        public void SaveIsTrueWhenSaveOptionSpecified()
        {
            var connection = CreateServerConnection(new[] {"import", "s:servername", "o:orgname", "--save"});
            Assert.IsTrue(connection.Save);
        }

        [TestMethod]
        public void WritesConnectionSettingsToFileWhenSaveOptionSpecified()
        {
            var config = CreateConfig();
            new ServerConnection(new[] { "taskname", "s:servername", "o:orgname", "p:5555", "protocol:http", "--save" },new ConsoleLogger(), config.Object);
            config.Verify(x=>x.Save(),Times.Once);
        }

        [TestMethod]
        public void ReadsServerConnectionSettingsFromFileWhenNotSpecified()
        {
            var config = new Mock<IConfigurationManager>();
            var settings = new NameValueCollection
                {
                    {"server", "servername"},
                    {"org", "orgname"},
                    {"port", "5555"},
                    {"protocol", "http"}
                };

            config.SetupGet(x => x.AppSettings).Returns(settings);

            var conn = new ServerConnection(new[] { "taskname"},
                                 new ConsoleLogger(), config.Object);

            Assert.AreEqual(conn.ServerName, "servername");
            Assert.AreEqual(conn.Organisation, "orgname");
            Assert.AreEqual(conn.Port, "5555");
            Assert.AreEqual(conn.Protocol, "http");
        }

        [TestMethod]
        public void CanReadServerNameArgument()
        {
           var conn = CreateServerConnection(new []{"import"," server:servername","org:orgname","port:5555"});
            Assert.AreEqual("servername", conn.ServerName);
        }

        [TestMethod]
        public void CanReadOrgNameArgument()
        {
            var conn = CreateServerConnection( new [] { "import", "server:servername", "org:orgname", "port:5555" } );
            Assert.AreEqual("orgname", conn.Organisation);
        }

        [TestMethod]
        public void CanReadPortNumberArgument()
        {
            var conn = CreateServerConnection(new [] { "import", "server:servername", "org:orgname", "port:5555" });
            Assert.AreEqual("5555", conn.Port);
        }

        [TestMethod]
        public void UsesLocalhostWhenServerNameIsNotSpecified()
        {
            var validCommandArgsWithoutServerName = new [] {"taskname", "org:orgname"};

            var conn = CreateServerConnection(validCommandArgsWithoutServerName);

            Assert.AreEqual("localhost",conn.ServerName);
        }
    }
}