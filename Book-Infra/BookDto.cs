using Book_Domain;

namespace Book_Infra
{
    public class BookDto
    {
        public Book book { get; set; }
        public List<Comment> comments { get; set; } = new List<Comment>();
        public int commentsCount { get; set; }
        public List<Relatedbook> relatedBooks { get; set; } = new List<Relatedbook>();
        public string shareText { get; set; }
        public List<Quote> quotes { get; set; } = new List<Quote>();
        public int quotesCount { get; set; }
        public bool hideOffCoupon { get; set; }
    }
}
