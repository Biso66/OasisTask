using Oasis.Models;

namespace Oasis.DTOS
{
    public class WorkDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; } = false;
        public int UserId { get; set; }
    }
}
