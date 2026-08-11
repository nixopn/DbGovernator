using DbGovernator;
using DbGovernator.Abstractions;
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
    public sealed class TestAudit
    {
        private Audit _audit;
        private Mock<ILogger> _logger;
        private DbCommand _innerCommand;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger>();
            _audit = new Audit();
            _audit.Logger = _logger.Object;
            _innerCommand = new NpgsqlCommand();
        }

        [TestMethod]
        public void CheckQueryInsert()
        {
            _innerCommand.CommandText = "insert into users(name) values ('aaaaaaaa');";
            var context = new DbGovernator.ExecutionContext();
            context.Command = _innerCommand;
            var step = new PrepareCommand();
            step.Context = context;
            _audit.VisitPreparing(step);
            Assert.AreEqual("insert", step.Context.QueryType);
            Assert.AreEqual("users", step.Context.TableName);
        }


        [TestMethod]
        public void CheckQueryUpdate()
        {
            _innerCommand.CommandText = "update users set name = 'aaaaaaaa' where id=229;";
            var context = new DbGovernator.ExecutionContext();
            context.Command = _innerCommand;
            var step = new PrepareCommand();
            step.Context = context;
            _audit.VisitPreparing(step);
            Assert.AreEqual("update", step.Context.QueryType);
            Assert.AreEqual("users", step.Context.TableName);
        }

        [TestMethod]
        public void CheckQuerySelect()
        {
            _innerCommand.CommandText = "select * from users;";
            var context = new DbGovernator.ExecutionContext();
            context.Command = _innerCommand;
            var step = new PrepareCommand();
            step.Context = context;
            _audit.VisitPreparing(step);
            Assert.AreEqual("select", step.Context.QueryType);
            Assert.AreEqual("users", step.Context.TableName);
        }

        [TestMethod]
        public void CheckQueryDelete()
        {
            _innerCommand.CommandText = "delete from users where id = 297;";
            var context = new DbGovernator.ExecutionContext();
            context.Command = _innerCommand;
            var step = new PrepareCommand();
            step.Context = context;
            _audit.VisitPreparing(step);
            Assert.AreEqual("delete", step.Context.QueryType);
            Assert.AreEqual("users", step.Context.TableName);
        }

        [TestMethod]
        public void CheckQueryWrongCommandFormat()
        {
            _innerCommand.CommandText = "inserawdawdt awdawdawdinawdawdto users(name) values ('aaaaaaaa');";
            var context = new DbGovernator.ExecutionContext();
            context.Command = _innerCommand;
            var step = new PrepareCommand();
            step.Context = context;
            _audit.VisitPreparing(step);
            Assert.AreEqual("unkown", step.Context.QueryType);
            Assert.AreEqual("", step.Context.TableName);
        }

        [TestMethod]
        public void CheckQueryNullCommandFormat()
        {
            _innerCommand.CommandText = null;
            var context = new DbGovernator.ExecutionContext();
            context.Command = _innerCommand;
            var step = new PrepareCommand();
            step.Context = context;
            _audit.VisitPreparing(step);
            Assert.AreEqual("unkown", step.Context.QueryType);
            Assert.AreEqual("", step.Context.TableName);
        }

        [TestMethod]
        public void CheckQueryStringEmptyCommandFormat()
        {
            _innerCommand.CommandText = String.Empty;
            var context = new DbGovernator.ExecutionContext();
            context.Command = _innerCommand;
            var step = new PrepareCommand();
            step.Context = context;
            _audit.VisitPreparing(step);
            Assert.AreEqual("unkown", step.Context.QueryType);
            Assert.AreEqual("", step.Context.TableName);
        }
    }
}
