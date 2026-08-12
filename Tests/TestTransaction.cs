using DbGovernator.Abstractions;
using DbGovernator.NDbClasses;
using DbGovernator.Realisations;
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
    public sealed class TestTransaction
    {
        private Mock<DbTransaction> _innerTransaction;
        private Mock<ITransactionVisitor> _transactionVisitor;
        private List<ITransactionVisitor> _transactionVisitors;
        private Mock<ILogger> _logger;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger>();
            _innerTransaction = new Mock<DbTransaction>();
            _transactionVisitor = new Mock<ITransactionVisitor>();
            _transactionVisitor.Object.Logger = _logger.Object;
            _transactionVisitors = new List<ITransactionVisitor> {  };
        }

        [TestMethod]
        public void BeginTransactionVisitorException()
        {
            _transactionVisitor.Setup(x => x.VisitBegin(It.IsAny<BeginTransactionStep>())).Throws<InvalidOperationException>();
            _transactionVisitors.Add(_transactionVisitor.Object);
            var trs = new NDbTransaction(_innerTransaction.Object, _transactionVisitors, _logger.Object);

            Assert.IsNotNull(trs);
            _transactionVisitor.Verify(x => x.VisitBegin(It.IsAny<BeginTransactionStep>()), Times.Once);
        }

        [TestMethod]
        public void BeginNoException()
        {
            _transactionVisitor.Setup(x => x.VisitBegin(It.IsAny<BeginTransactionStep>()));
            _transactionVisitors.Add(_transactionVisitor.Object);
            var trs = new NDbTransaction(_innerTransaction.Object, _transactionVisitors, _logger.Object);

            Assert.IsNotNull(trs);
            _transactionVisitor.Verify(x => x.VisitBegin(It.IsAny<BeginTransactionStep>()), Times.Once);
        }

        [TestMethod]
        public void CommitTransactionVisitorException()
        {
            _transactionVisitor.Setup(x => x.VisitCommit(It.IsAny<CommitTransactionStep>())).Throws<InvalidOperationException>();
            _transactionVisitors.Add(_transactionVisitor.Object);
            var trs = new NDbTransaction(_innerTransaction.Object, _transactionVisitors, _logger.Object);
            _innerTransaction.Setup(x => x.Commit());
            trs.Commit();
            Assert.IsNotNull(trs);
            _transactionVisitor.Verify(x => x.VisitCommit(It.IsAny<CommitTransactionStep>()), Times.Once);
            _innerTransaction.Verify(x => x.Commit(), Times.Once);
        }

        [TestMethod]
        public void CommitNoException()
        {
            _transactionVisitor.Setup(x => x.VisitBegin(It.IsAny<BeginTransactionStep>()));
            _transactionVisitors.Add(_transactionVisitor.Object);
            var trs = new NDbTransaction(_innerTransaction.Object, _transactionVisitors, _logger.Object);
            _innerTransaction.Setup(x => x.Commit());
            trs.Commit();
            Assert.IsNotNull(trs);
            _transactionVisitor.Verify(x => x.VisitCommit(It.IsAny<CommitTransactionStep>()), Times.Once);
            _innerTransaction.Verify(x => x.Commit(), Times.Once);
        }

        [TestMethod]
        public void RollBackTransactionVisitorException()
        {
            _transactionVisitor.Setup(x => x.VisitRollback(It.IsAny<RollbackTransactionStep>())).Throws<InvalidOperationException>();
            _transactionVisitors.Add(_transactionVisitor.Object);
            var trs = new NDbTransaction(_innerTransaction.Object, _transactionVisitors, _logger.Object);
            _innerTransaction.Setup(x => x.Rollback());
            trs.Rollback();
            Assert.IsNotNull(trs);
            _transactionVisitor.Verify(x => x.VisitRollback(It.IsAny<RollbackTransactionStep>()), Times.Once);
            _innerTransaction.Verify(x => x.Rollback(), Times.Once);
        }

        [TestMethod]
        public void RollBackCommitNoException()
        {
            _transactionVisitor.Setup(x => x.VisitBegin(It.IsAny<BeginTransactionStep>()));
            _transactionVisitors.Add(_transactionVisitor.Object);
            var trs = new NDbTransaction(_innerTransaction.Object, _transactionVisitors, _logger.Object);
            _innerTransaction.Setup(x => x.Rollback());
            trs.Rollback();
            Assert.IsNotNull(trs);
            _transactionVisitor.Verify(x => x.VisitRollback(It.IsAny<RollbackTransactionStep>()), Times.Once);
            _innerTransaction.Verify(x => x.Rollback(), Times.Once);
        }

    }
}
