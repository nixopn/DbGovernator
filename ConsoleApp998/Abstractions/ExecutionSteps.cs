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

    // Абстрактный класс, определяющий шаги выполнения команд.
    // Все конкретные классы являются этапами работы команды и принимают следующие параметры.
    // Посетителя, выполняющего свои функции на этом этапе.
    // Контекст исполнения, принимается посетителем.
    public abstract class ExecutionStep
    {
        public abstract void AcceptVisitor(IVisitor visitor, ExecutionContext context);
        public abstract Task AcceptVisitorAsync(IVisitor visitor, ExecutionContext context);
    }

}


