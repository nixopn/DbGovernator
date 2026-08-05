using DbGovernator.Abstractions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.NDbClasses
{
    /// <summary>
    /// Фабрика ддя источника данных.
    /// Через неё внедряются зависимости.
    /// </summary>
    public class NDbDataSourceFactory : INDbDataSourceFactory
    {
        IEnumerable<IVisitor> _visitors;
        ILogger _logger;
        IConnectionStringProvider _connectionStringProvider;
        IEnumerable<ITransactionVisitor> _transactionVisitors;

        /// <summary>
        /// Конструктор фабрики.
        /// </summary>
        /// <param name="visitors">Список посетителей, получается через DI.</param>
        /// <param name="logger">Логгер, получается через DI.</param>
        /// <param name="connectionStringProvider">Провайдер строки соединения, получается через DI.</param>
        public NDbDataSourceFactory(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// Если нужна транзакция.
        /// </summary>
        /// <param name="visitors">Список посетителей, получается через DI.</param>
        /// <param name="logger">Логгер, получается через DI.</param>
        /// <param name="connectionStringProvider">Провайдер строки соединения, получается через DI.</param>
        /// <param name="transactionVisitors">Список посетителей транзакций, получается через DI.</param>
        public NDbDataSourceFactory(IEnumerable<IVisitor> visitors, ILogger logger, IConnectionStringProvider connectionStringProvider, IEnumerable<ITransactionVisitor> transactionVisitors)
        {
            _visitors = visitors;
            _logger = logger;
            _connectionStringProvider = connectionStringProvider;
            _transactionVisitors = transactionVisitors;
        }
        /// <summary>
        /// Функция для создания источника данных.
        /// </summary>
        /// <returns>
        /// Источник данных.
        /// </returns>
        public DbDataSource Create()
        {
            var dataSource = NpgsqlDataSource.Create(_connectionStringProvider.GetConnectionString());
            var NDataSource = new NDbDataSource(dataSource, _visitors, _logger, _transactionVisitors);
            return NDataSource;
        }
    }
}
