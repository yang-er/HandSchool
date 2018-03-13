using System.Threading.Tasks;

namespace HandSchool
{
    public interface ISystemEntrance
    {
        string Name { get; }
        string ScriptFileUri { get; }
        bool IsPost { get; }
        string PostValue { get; }
        string StorageFile { get; }
        string LastReport { get; }
        Task Execute();
        void Parse();
    }
}
