namespace Oasis.Helpers
{
    public class Pagination
    {
        public List<object> Tasks { get; set; } = new List<object>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
