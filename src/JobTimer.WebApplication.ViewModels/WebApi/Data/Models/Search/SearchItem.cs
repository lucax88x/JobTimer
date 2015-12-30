namespace JobTimer.WebApplication.ViewModels.WebApi.Data.Models.Search
{
    public enum SearchItemType
    {
        Page = 0
    }
    public class SearchItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public SearchItemType Type { get; set; }
        public string Id { get; set; }
    }
}
