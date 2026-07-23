using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;





namespace DbGovernator.Abstractions
{
    /// <summary>
    /// Абстрактный класс, определяющий шаги выполнения команд.
    /// Все конкретные классы являются этапами работы команды и принимают следующие параметры.
    /// Посетителя, выполняющего свои функции на этом этапе.
    /// Контекст исполнения, принимаемый посетителем.
    /// </summary>
    public abstract class ExecutionStep
    {
        public ExecutionContext Context { get; set; } = new ExecutionContext();
        public abstract void AcceptVisitor(IVisitor visitor, ExecutionContext context);
        public abstract Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context);
    }

}


