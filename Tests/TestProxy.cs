using DbGovernator;
using DbGovernator.Abstractions;
using DbGovernator.LinqToDB;
using DbGovernator.NDbClasses;
using DbGovernator.Realisations;
using LinqToDB;
using LinqToDB.Data;
using Moq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public sealed class TestProxy
    {
        private Mock<DbCommand> _innerCommand;
        private Mock<ILogger> _logger;
        private List<IVisitor> _visitors;
        private Mock<IVisitor> _visitor;
        private Mock<IVisitor> _visitor2;
        private Mock<DbTransaction> _innerTransaction;
        private Mock<ITransactionVisitor> _transactionVisitor;
        private List<ITransactionVisitor> _transactionVisitors;
        private Mock<ConnectionStringProvider> _connectionStringProvider;

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
            _connectionStringProvider = new Mock<ConnectionStringProvider>();
        }

        [TestMethod]
        public void NDbCommandExecuteNoQuery()
        {
            int expectedResult = 299;
            _innerCommand.Setup(x => x.ExecuteNonQuery()).Returns(expectedResult);

            var executionStrategy = new ExecutionStrategy();
            executionStrategy.Logger = _logger.Object;

            _visitors.Add(executionStrategy);

            var command = new NDbCommand(_innerCommand.Object, _visitors, _logger.Object);
            var result = command.ExecuteNonQuery();

            Assert.AreEqual(expectedResult, result);

            _innerCommand.Verify(x => x.ExecuteNonQuery(), Times.Once);
        }

        [TestMethod]
        public void NDbCommandExecuteScalar()
        {
            int expectedResult = 299;
            _innerCommand.Setup(x => x.ExecuteScalar()).Returns(expectedResult);

            var executionStrategy = new ExecutionStrategy();
            executionStrategy.Logger = _logger.Object;

            _visitors.Add(executionStrategy);

            var command = new NDbCommand(_innerCommand.Object, _visitors, _logger.Object);
            var result = command.ExecuteScalar();

            Assert.AreEqual(expectedResult, result);

            _innerCommand.Verify(x => x.ExecuteScalar(), Times.Once);
        }

        [TestMethod]
        public void LinqToDbProxy()
        {
            var executionStrategy = new ExecutionStrategy();
            executionStrategy.Logger = _logger.Object;

            _visitors.Clear();
            _visitors.Add(_visitor.Object);
            _visitors.Add(executionStrategy);
            Mock<DbConnection> npgsqlCon = new Mock<DbConnection>();
            Mock<NDbConnection> ndbCon = new Mock<NDbConnection>(npgsqlCon.Object, _visitors, _logger.Object, _transactionVisitors);
            Mock<NDbCommand> mockCommand = new Mock<NDbCommand>(_innerCommand.Object, _visitors, _logger.Object);
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(299);
            //ndbCon.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);
            NDbDataConnectionFactory dataConnectionFactory = new NDbDataConnectionFactory(_visitors, _logger.Object, _transactionVisitors, _connectionStringProvider.Object);
            var db = dataConnectionFactory.CreateConnection(ndbCon.Object);
            //var newUser = new User { Name = "AAALinqToDBUser" };
            //var insertedId = db.Insert(newUser);
            //var insertedId = db.Execute("Insert into users(name) values ('aaaaaaaa');");
            //Assert.AreEqual(299, insertedId);
            mockCommand.Verify(x => x.ExecuteNonQuery(), Times.Never);
            //ndbCon.Verify(x => x.CreateCommand(), Times.Once);
        }

    }
}
