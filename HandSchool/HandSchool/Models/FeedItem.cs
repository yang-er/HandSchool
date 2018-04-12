namespace HandSchool
{
    namespace Models
    {
        public class FeedItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Link { get; set; }
            public string Comments { get; set; }
            public string PubDate { get; set; }
            public string Creator { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
        }
    }

    public interface IFeedEntrance : ISystemEntrance { }
}
