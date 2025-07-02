using System.Collections.Generic;
using Commands;

namespace Managers
{
    public class CommandInvoker
    {
        private readonly Stack<ICommand> _commandHistory = new();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commandHistory.Push(command);
        }

        public void UndoLast()
        {
            if (_commandHistory.Count > 0)
            {
                ICommand lastCommand = _commandHistory.Pop();
                lastCommand.Undo();
            }
        }

        public void ClearHistory()
        {
            _commandHistory.Clear();
        }
    }
}
