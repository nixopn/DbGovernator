using DbGovernator.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    /// <summary>
    /// Контекст для посетителей.
    /// </summary>
    public class ExecutionContext
    {
        /// <summary>
        /// Для выяснения сырого запроса.
        /// </summary>
        public DbCommand Command { get; set; }

        /// <summary>
        /// Время, которое засекается перед началом выполнения команды.
        /// </summary>
        public DateTime Before { get; set; }

        /// <summary>
        /// Время, которое засекается после выполнения команды.
        /// </summary>
        public DateTime After { get; set; }

        /// <summary>
        /// Промежуток между началом и концом выполнения команды.
        /// </summary>
        public TimeSpan Duration => After - Before;

        /// <summary>
        /// Хранит результат функции исполняющей запрос.
        /// </summary>
        public object? Result { get; set; }

        /// <summary>
        /// Если резльутат int, то записывается сюда.
        /// </summary>
        public int AffectedRows { get; set; }

        /// <summary>
        /// Имя таблицы.
        /// </summary>
        public string? TableName { get; set; }

        /// <summary>
        /// Тип запроса
        /// </summary>
        public string? QueryType { get; set; }

        /// <summary>
        /// Делегат для передачи функции исполняющей запрос.
        /// </summary>
        public Func<object?> ExecutionFunction { get; set; }

        /// <summary>
        /// Хранит информацию о том, была ли у конкретного посетителя ошибка.
        /// </summary>
        public Dictionary<IVisitor, bool> HadError { get; set; } = new();
    }
}
