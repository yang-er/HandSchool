namespace HandSchool.Internal
{
    interface ISystemEntrance
    {
        ISchoolSystem Parent { get; }
        string Name { get; }
        string ScriptFileUri { get; }
        bool IsPost { get; }
        string PostValue { get; }
        void Execute();
    }
}
