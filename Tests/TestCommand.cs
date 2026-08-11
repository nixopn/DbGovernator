using DbGovernator.Abstractions;
using DbGovernator.NDbClasses;
using DbGovernator.Realisations;
using Moq;
using Npgsql;
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
        private Mock<IVisitor> _visitor2;
        private Mock<DbTransaction> _innerTransaction;
        private Mock<ITransactionVisitor> _transactionVisitor;
        private List<ITransactionVisitor> _transactionVisitors;

        [TestInitialize]
        public void Setup()
        {
            _innerCommand = new Mock<DbCommand>();
            _logger = new Mock<ILogger>();
            _visitor = new Mock<IVisitor>();
            _visitor2 = new Mock<IVisitor>();
            _visitors = new List<IVisitor>() { _visitor.Object, _visitor2.Object };
            _innerTransaction = new Mock<DbTransaction>();
            _transactionVisitor = new Mock<ITransactionVisitor>();
            _transactionVisitors = new List<ITransactionVisitor> { _transactionVisitor.Object };
        }

        [TestMethod]
        public void SetLoggerForVisitors()
        {
            var cmd = new NDbCommand(_innerCommand.Object, _visitors, _logger.Object);
            _visitor.VerifySet(x => x.Logger = _logger.Object);
            _visitor2.VerifySet(x => x.Logger = _logger.Object);
        }

        [TestMethod]
        public void AllVisitorsCalled()
        {
            int expectedResult = 299;
            _innerCommand.Setup(x => x.ExecuteNonQuery()).Returns(expectedResult);

            var executionStrategy = new ExecutionStrategy();
            executionStrategy.Logger = _logger.Object;

            _visitor.Setup(x => x.VisitPreparing(It.IsAny<PrepareCommand>()));
            _visitor.Setup(x => x.VisitBeforeExecution(It.IsAny<BeforeExecute>()));
            _visitor.Setup(x => x.VisitExecution(It.IsAny<ExecutionSt>()));
            _visitor.Setup(x => x.VisitAfterExecution(It.IsAny<AfterExecution>()));
            _visitor.Setup(x => x.VisitResultProcessing(It.IsAny<ResultProcessing>()));

            _visitor2.Setup(x => x.VisitPreparing(It.IsAny<PrepareCommand>()));
            _visitor2.Setup(x => x.VisitBeforeExecution(It.IsAny<BeforeExecute>()));
            _visitor2.Setup(x => x.VisitExecution(It.IsAny<ExecutionSt>()));
            _visitor2.Setup(x => x.VisitAfterExecution(It.IsAny<AfterExecution>()));
            _visitor2.Setup(x => x.VisitResultProcessing(It.IsAny<ResultProcessing>()));

            _visitors.Add(executionStrategy);

            var command = new NDbCommand(_innerCommand.Object, _visitors, _logger.Object);
            var result = command.ExecuteNonQuery();

            Assert.AreEqual(expectedResult, result);
            
            _visitor.Verify(x => x.VisitPreparing(It.IsAny<PrepareCommand>()), Times.Once);
            _visitor.Verify(x => x.VisitBeforeExecution(It.IsAny<BeforeExecute>()), Times.Once);
            _visitor.Verify(x => x.VisitExecution(It.IsAny<ExecutionSt>()), Times.Once);
            _visitor.Verify(x => x.VisitAfterExecution(It.IsAny<AfterExecution>()), Times.Once);
            _visitor.Verify(x => x.VisitResultProcessing(It.IsAny<ResultProcessing>()), Times.Once);

            _visitor2.Verify(x => x.VisitPreparing(It.IsAny<PrepareCommand>()), Times.Once);
            _visitor2.Verify(x => x.VisitBeforeExecution(It.IsAny<BeforeExecute>()), Times.Once);
            _visitor2.Verify(x => x.VisitExecution(It.IsAny<ExecutionSt>()), Times.Once);
            _visitor2.Verify(x => x.VisitAfterExecution(It.IsAny<AfterExecution>()), Times.Once);
            _visitor2.Verify(x => x.VisitResultProcessing(It.IsAny<ResultProcessing>()), Times.Once);
        }

        [TestMethod]
        public void SkipVisitorsWhenThrows()
        {
            int expectedResult = 299;
            _innerCommand.Setup(x => x.ExecuteNonQuery()).Returns(expectedResult);

            var executionStrategy = new ExecutionStrategy();
            executionStrategy.Logger = _logger.Object;

            _visitor.Setup(x => x.VisitPreparing(It.IsAny<PrepareCommand>()));
            _visitor.Setup(x => x.VisitBeforeExecution(It.IsAny<BeforeExecute>()));
            _visitor.Setup(x => x.VisitExecution(It.IsAny<ExecutionSt>())).Throws(new Exception("Test exception"));
            _visitor.Setup(x => x.VisitAfterExecution(It.IsAny<AfterExecution>()));
            _visitor.Setup(x => x.VisitResultProcessing(It.IsAny<ResultProcessing>()));

            _visitor2.Setup(x => x.VisitPreparing(It.IsAny<PrepareCommand>()));
            _visitor2.Setup(x => x.VisitBeforeExecution(It.IsAny<BeforeExecute>()));
            _visitor2.Setup(x => x.VisitExecution(It.IsAny<ExecutionSt>()));
            _visitor2.Setup(x => x.VisitAfterExecution(It.IsAny<AfterExecution>()));
            _visitor2.Setup(x => x.VisitResultProcessing(It.IsAny<ResultProcessing>()));

            _visitors.Add(executionStrategy);

            var command = new NDbCommand(_innerCommand.Object, _visitors, _logger.Object);
            var result = command.ExecuteNonQuery();

            Assert.AreEqual(expectedResult, result);

            _visitor.Verify(x => x.VisitPreparing(It.IsAny<PrepareCommand>()), Times.Once);
            _visitor.Verify(x => x.VisitBeforeExecution(It.IsAny<BeforeExecute>()), Times.Once);
            _visitor.Verify(x => x.VisitExecution(It.IsAny<ExecutionSt>()), Times.Once);
            _visitor.Verify(x => x.VisitAfterExecution(It.IsAny<AfterExecution>()), Times.Never);
            _visitor.Verify(x => x.VisitResultProcessing(It.IsAny<ResultProcessing>()), Times.Never);

            _visitor2.Verify(x => x.VisitPreparing(It.IsAny<PrepareCommand>()), Times.Once);
            _visitor2.Verify(x => x.VisitBeforeExecution(It.IsAny<BeforeExecute>()), Times.Once);
            _visitor2.Verify(x => x.VisitExecution(It.IsAny<ExecutionSt>()), Times.Once);
            _visitor2.Verify(x => x.VisitAfterExecution(It.IsAny<AfterExecution>()), Times.Once);
            _visitor2.Verify(x => x.VisitResultProcessing(It.IsAny<ResultProcessing>()), Times.Once);
        }

        [TestMethod]
        public void NDbTransactionSet()
        {
            NDbTransaction trs = new NDbTransaction(_innerTransaction.Object, _transactionVisitors, _logger.Object);
            var cmd = new NDbCommand(_innerCommand.Object, _visitors, _logger.Object);
            cmd.Transaction = trs;
            _innerCommand.VerifySet(x => x.Transaction = _innerTransaction.Object);
        }

        [TestMethod]
        public void DbTransactionSet()
        {
           
            var cmd = new NDbCommand(_innerCommand.Object, _visitors, _logger.Object);
            cmd.Transaction = _innerTransaction.Object;
            _innerCommand.VerifySet(x => x.Transaction = _innerTransaction.Object);
        }
    }
}
