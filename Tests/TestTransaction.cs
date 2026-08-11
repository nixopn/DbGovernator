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
        public void BeginException()
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
    }
}
