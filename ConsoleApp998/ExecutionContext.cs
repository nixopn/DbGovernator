using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    // Контекст для посетителей
    public class ExecutionContext
    {
        public DbCommand Command { get; set; } // Для выяснения сырого запроса

        // Свойства для выяснения длительности запроса
        public DateTime Before { get; set; }
        public DateTime After { get; set; }
        public TimeSpan Duration => After - Before;

        // Хранит результат функции исполняющей запрос
        public object? Result { get; set; }
        // Если резльутат int, то записывается сюда
        public int affectedRows { get; set; }

        // Имя таблицы
        public string? tableName { get; set; }
        // Тип запроса
        public string? queryType { get; set; }

        // Делегат для передачи функции исполняющей запрос
        public Delegate executionFunction { get; set; }
    }
}
