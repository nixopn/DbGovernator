using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator
{
    /// <summary>
    /// Контекст для посетителей транзакций.
    /// </summary>
    public class TransactionContext
    {
        /// <summary>
        /// Сохранена ли отменена команда:
        /// True - сохранена.
        /// False  - отменена.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Время, которое засекается в начале транзакции.
        /// </summary>
        public DateTime Before { get; set; }

        /// <summary>
        /// Время, которое засекается в конце транзакции.
        /// </summary>
        public DateTime After { get; set; }

        /// <summary>
        /// Промежуток между началом и концом транзакции.
        /// </summary>
        public TimeSpan Duration => After - Before;
    }
}
