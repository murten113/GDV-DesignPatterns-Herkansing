namespace Commands
{
    public interface ICommand
    {
        /// <summary>
        /// ICommand is an interface that defines the contract for command objects in a command pattern implementation.
        /// </summary>
        void Execute();
        void Undo();
    }
}
