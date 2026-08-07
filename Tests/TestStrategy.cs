using DbGovernator.Abstractions;
using DbGovernator.Realisations;
using LinqToDB;
using Moq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using DbGovernator;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Tests
{
    [TestClass]
    public sealed class TestStrategy
    {
        private ExecutionStrategy _executionStrategy;
        private Mock<ILogger> _logger;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger>();
            _executionStrategy = new ExecutionStrategy();
            _executionStrategy.Logger = _logger.Object;
        }

        [TestMethod]
        public void HasDbExceptionWhenIsNpgsqlException()
        {
            var ex = new NpgsqlException("Test Db exception");
            var result = _executionStrategy.HasDbException(ex);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasDbExceptionWhenIsLinqToDblException()
        {
            var ex = new LinqToDBException("Test Db exception");
            var result = _executionStrategy.HasDbException(ex);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasDbExceptionWhenIsNotDbException()
        {
            var ex = new NullReferenceException("Test Db exception");
            var result = _executionStrategy.HasDbException(ex);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasDbExceptionWhenIsInnerDblException()
        {
            var ex = new NpgsqlException("Test Db exception");
            var outEx = new NullReferenceException("Outer exception", ex);
            var result = _executionStrategy.HasDbException(outEx);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VisitExecutionSuccessIntResult()
        {
            var context = new DbGovernator.ExecutionContext();
            int expectedResult = 298;
            context.ExecutionFunction = () => expectedResult;
            var step = new ExecutionSt();
            step.Context = context;
            _executionStrategy.VisitExecution(step);
            Assert.AreEqual(expectedResult, context.Result);
            Assert.AreEqual(expectedResult, context.AffectedRows);
        }

        [TestMethod]
        public void VisitExecutionSuccessObjectResult()
        {
            var context = new DbGovernator.ExecutionContext();
            string obj = "Object result";
            context.ExecutionFunction = () => obj;
            var step = new ExecutionSt();
            step.Context = context;
            _executionStrategy.VisitExecution(step);
            Assert.AreEqual(obj, context.Result);
        }

        [TestMethod]
        public void VisitExecutionSuccessTaskResult()
        {
            var context = new DbGovernator.ExecutionContext();
            var taskResult = Task.FromResult(298);
            context.ExecutionFunction = () => taskResult;
            var step = new ExecutionSt();
            step.Context = context;
            _executionStrategy.VisitExecution(step);
            Assert.AreEqual(taskResult.Result, context.Result);
            Assert.AreEqual(taskResult.Result, context.AffectedRows);
        }

        [TestMethod]
        public void VisitExecutionRetryOnDbExceptionSuccess()
        {
            var context = new DbGovernator.ExecutionContext();
            int expectedResult = 298;
            int cnt = 0;
            context.ExecutionFunction = () =>
            {
                cnt++;
                if (cnt < 8)
                {
                    throw new NpgsqlException("Test exception");
                }
                return 298;
            };
            var step = new ExecutionSt();
            step.Context = context;
            _executionStrategy.VisitExecution(step);
            Assert.AreEqual(expectedResult, context.Result);
            Assert.AreEqual(expectedResult, context.AffectedRows);
        }

        [TestMethod]
        public void VisitExecutionRetryOnDbExceptionFail()
        {
            var context = new DbGovernator.ExecutionContext();
            int cnt = 0;
            context.ExecutionFunction = () =>
            {
                cnt++;
                if (cnt < 29)
                {
                    throw new NpgsqlException("Test exception");
                }
                return 298;
            };
            var step = new ExecutionSt();
            step.Context = context;
            var ex = Assert.ThrowsException<Exception>(() =>
            {
                _executionStrategy.VisitExecution(step);
            });
            Assert.AreEqual("Service is temporary unavailable", ex.Message);
        }

        [TestMethod]
        public void VisitExecutionRetryOnNonDbExceptionFail()
        {
            var context = new DbGovernator.ExecutionContext();
            context.ExecutionFunction = () =>
            {
                throw new NullReferenceException("Test exception");
            };
            var step = new ExecutionSt();
            step.Context = context;
            var ex = Assert.ThrowsException<Exception>(() =>
            {
                _executionStrategy.VisitExecution(step);
            });
            Assert.AreEqual("Not an sql exception", ex.Message);
        }
    }
}
