using DbGovernator.Abstractions;
using DbGovernator.NDbClasses;
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
    public sealed class TestConnection
    {
        private NpgsqlConnection _innerConnection;
        private Mock<ILogger> _logger;
        private List<IVisitor> _visitors;
        private Mock<IVisitor> _visitor;
        private NDbConnection _connection;
        private Mock<ITransactionVisitor> _transactionVisitor;
        private List<ITransactionVisitor> _transactionVisitors;

        [TestInitialize]
        public void Setup()
        {
            _innerConnection = new NpgsqlConnection();
            _logger = new Mock<ILogger>();
            _visitor = new Mock<IVisitor>();
            _visitors = new List<IVisitor>() { _visitor.Object };
            _transactionVisitor = new Mock<ITransactionVisitor>();
            _transactionVisitors = new List<ITransactionVisitor> { _transactionVisitor.Object };
        }

        [TestMethod]
        public void CreateDbCommandNoTransaction()
        {
            var cmd = new NpgsqlCommand();
            _connection = new NDbConnection(_innerConnection, _visitors, _logger.Object, _transactionVisitors);
            var res = _connection.CreateCommand();
            Assert.IsNull(res.Transaction);
        }


    }
}
