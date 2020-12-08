namespace QueenOfHearts.ExecutionService
{
    public interface IStateManager
    {
        IState ObtainState();
        void Initialize();
    }
}