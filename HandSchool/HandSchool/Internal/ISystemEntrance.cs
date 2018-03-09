namespace HandSchool.Internal
{
    public interface ISystemEntrance
    {
        string Name { get; }
        string ScriptFileUri { get; }
        bool IsPost { get; }
        string PostValue { get; }
        string StorageFile { get; }
        string LastReport { get; }
        void Execute();
        void Parse();
    }
}
