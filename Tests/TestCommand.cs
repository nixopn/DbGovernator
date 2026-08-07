using DbGovernator.Abstractions;
using DbGovernator.NDbClasses;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public sealed class TestCommand
    {
        private Mock<DbCommand> _innerCommand;
        private Mock<ILogger> _logger;
        private List<IVisitor> _visitors;
        private Mock<IVisitor> _visitor;

        [TestInitialize]
        public void Setup()
        {
            _innerCommand = new Mock<DbCommand>();
            _logger = new Mock<ILogger>();
            _visitor = new Mock<IVisitor>();
            _visitors = new List<IVisitor>() { _visitor.Object };
        }

        [TestMethod]
        public void SetLoggerForVisitors()
        {
            var cmd = new NDbCommand(_innerCommand.Object, _visitors, _logger.Object);
            _visitor.VerifySet(x => x.Logger = _logger.Object);
        }

        [TestMethod]
        public void AllVisitorsCalled()
        {
            Thread.Sleep(22);
        }
    }
}
