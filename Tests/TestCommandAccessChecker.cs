using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbGovernator;
using DbGovernator.Abstractions;
using DbGovernator.Realisations;
using System.Data.Common;
using Npgsql;

namespace Tests
{
    [TestClass]
    public sealed class TestCommandAccessChecker
    {
        private AccessChecker _accessChecker;
        private Mock<ILogger> _logger;
        private DbCommand _innerCommand;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger>();
            _accessChecker = new AccessChecker();
            _accessChecker.Logger = _logger.Object;
            _innerCommand = new NpgsqlCommand();
        }

        [TestMethod]
        public void AccessGranted()
        {
            _innerCommand.CommandText = "insert into users(name) values ('aaaaaaaa');";
            var context = new DbGovernator.ExecutionContext();
            context.QueryType = "insert";
            context.Command = _innerCommand;
            var step = new PrepareCommand();
            step.Context = context;
            _accessChecker.VisitPreparing(step);
            Assert.AreEqual("insert into users(name) values ('aaaaaaaa');", _innerCommand.CommandText);
        }

        [TestMethod]
        public void AccessDenied()
        {
            // Проверяем ситуацию, когда посетитель AccessChecker запрещает доступ.
            _innerCommand.CommandText = "delete from users where id = 299;";
            var context = new DbGovernator.ExecutionContext();
            context.QueryType = "delete";
            context.TableName = "users";
            context.Command = _innerCommand;
            var step = new PrepareCommand();
            step.Context = context;
            var ex = Assert.ThrowsException<Exception>(() =>
            {
                _accessChecker.VisitPreparing(step);
            });
            Assert.AreEqual("Access denied", ex.Message);
        }
    }
}
