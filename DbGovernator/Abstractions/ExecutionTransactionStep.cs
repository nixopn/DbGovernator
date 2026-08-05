using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbGovernator.Abstractions
{
    /// <summary>
    /// Абстрактный класс, определяющий шаги работы транзакций.
    /// Все конкретные классы являются этапами работы команды и принимают следующие параметры.
    /// Посетителя, выполняющего свои функции на этом этапе.
    /// Контекст транзакции, принимаемый посетителем.
    /// </summary>
    public abstract class ExecutionTransactionStep
    {
        public TransactionContext Context { get; set; } = new TransactionContext();
        public abstract void AcceptVisitor(ITransactionVisitor visitor, TransactionContext context);
        public abstract Task AcceptVisitorAsync(ITransactionVisitor visitor, TransactionContext context);
    }
}
