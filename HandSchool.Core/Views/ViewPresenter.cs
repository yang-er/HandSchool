namespace HandSchool.Views
{
    public interface IViewPresenter
    {
        IViewPage[] GetAllPages();

        int PageCount { get; }
    }
}
