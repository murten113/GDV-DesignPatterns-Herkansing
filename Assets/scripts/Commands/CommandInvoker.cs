using System.Collections.Generic;
using Commands;

namespace Managers
{

    /// <summary>
    /// CommandInvoker is a class that manages the execution and undoing of commands in a command pattern implementation.
    /// </summary>
    public class CommandInvoker
    {

        private readonly Stack<ICommand> _commandHistory = new();

        /// <summary>
        /// Constructor for CommandInvoker.
        /// </summary>
        /// <param name="command"></param>
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commandHistory.Push(command);
        }


        /// <summary>
        /// Undoes the last executed command if there is one in the history.
        /// </summary>
        public void UndoLast()
        {
            if (_commandHistory.Count > 0)
            {
                ICommand lastCommand = _commandHistory.Pop();
                lastCommand.Undo();
            }
        }


        /// <summary>
        /// Clears the command history, removing all executed commands.
        /// </summary>
        public void ClearHistory()
        {
            _commandHistory.Clear();
        }
    }
}
