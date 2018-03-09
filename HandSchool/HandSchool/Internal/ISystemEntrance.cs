namespace HandSchool.Internal
{
    public interface ISystemEntrance
    {
        string Name { get; }
        string ScriptFileUri { get; }
        bool IsPost { get; }
        string PostValue { get; }
        void Execute();
    }
}
