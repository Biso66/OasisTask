namespace Oasis.Models
{
    public class Work
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; } = false;
        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}
