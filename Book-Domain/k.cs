using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Domain
{


    public class Book
    {
        public int id { get; set; }
        public string title { get; set; }
        public int sourceBookId { get; set; }
        public bool index { get; set; }
        public bool hasPhysicalEdition { get; set; }
        public int canonicalId { get; set; }
        public DateTime lastUpdate { get; set; }
        public string description { get; set; }
        public string htmlDescription { get; set; }
        public int PublisherID { get; set; }
        public string publisherSlug { get; set; }
        public float price { get; set; }
        public int numberOfPages { get; set; }
        public float rating { get; set; }
        public List<Rate> rates { get; set; } = new List<Rate>();
        public List<Ratedetail> rateDetails { get; set; } = new List<Ratedetail>();
        public List<Type> types { get; set; } = new List<Type>();
        public string sticker { get; set; }
        public int beforeOffPrice { get; set; }
        public bool isRtl { get; set; }
        public bool showOverlay { get; set; }
        public int PhysicalPrice { get; set; }
        public int physicalBeforeOffPrice { get; set; }
        public string ISBN { get; set; }
        public string publishDate { get; set; }
        public int destination { get; set; }
        public string type { get; set; }
        public string refId { get; set; }
        public string coverUri { get; set; }
        public string shareUri { get; set; }
        public string shareText { get; set; }
        public string publisher { get; set; }
        public List<Author> authors { get; set; } = new List<Author>();
        public List<File> files { get; set; } = new List<File>();
        public List<Label> labels { get; set; } = new List<Label>();
        public List<Category> categories { get; set; } = new List<Category>();
        public bool subscriptionAvailable { get; set; }
        public int state { get; set; }
        public bool encrypted { get; set; }
        public float currencyPrice { get; set; }
        public float currencyBeforeOffPrice { get; set; }
    }

    public class Rate
    {
        public float value { get; set; }
        public int count { get; set; }
    }

    public class Ratedetail
    {
        public int id { get; set; }
        public string title { get; set; }
        public float point { get; set; }
    }

    public class Type
    {
        public int id { get; set; }
        public string name { get; set; }
        public float price { get; set; }
        public float beforeOffPrice { get; set; }
    }

    public class Author
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int type { get; set; }
        public string slug { get; set; }
    }

    public class File
    {
        public int id { get; set; }
        public int size { get; set; }
        public int type { get; set; }
        public int bookId { get; set; }
        public int sequenceNo { get; set; }
    }

    public class Label
    {
        public int tagID { get; set; }
        public string tag { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public int parent { get; set; }
        public string title { get; set; }
        public string audioTitle { get; set; }
        public string slug { get; set; }
        public string audioSlug { get; set; }
        public bool hasAudioSection { get; set; }
        public int firstAudioVisibleId { get; set; }
    }

    public class Comment
    {
        public int id { get; set; }
        public int accountId { get; set; }
        public bool verifiedAccount { get; set; }
        public string nickname { get; set; }
        public string profileImageUri { get; set; }
        public int bookId { get; set; }
        public string bookCoverUri { get; set; }
        public string bookType { get; set; }
        public string bookName { get; set; }
        public string creationDate { get; set; }
        public float rate { get; set; }
        public object[] latestReplies { get; set; }
        public int repliesCount { get; set; }
        public int likeCount { get; set; }
        public int dislikeCount { get; set; }
        public string comment { get; set; }
        public bool isLiked { get; set; }
        public bool isDisliked { get; set; }
        public List<Ratedetail> rateDetails { get; set; } = new List<Ratedetail>();
        public int recommendation { get; set; }
    }

    public class Relatedbook
    {
        public int type { get; set; }
        public string title { get; set; }
        public int backgroundStyle { get; set; }
        public Bookdata bookData { get; set; }
        public Destination destination { get; set; }
        public Backgroundconfig backgroundConfig { get; set; }
    }

    public class Bookdata
    {
        public List<Book> books { get; set; } = new List<Book>();
        public int layout { get; set; }
        public bool showPrice { get; set; }
    }

    public class Correspondingbook
    {
        public string title { get; set; }
        public string color { get; set; }
        public string icon { get; set; }
        public Destination destination { get; set; }
    }

    public class Destination
    {
        public int type { get; set; }
        public int order { get; set; }
        public int navigationPage { get; set; }
        public int operationType { get; set; }
        public int bookId { get; set; }
    }


    public class Filters
    {
        public List<List> list { get; set; } = new List<List>();
        public string refId { get; set; }
    }

    public class List
    {
        public int type { get; set; }
        public int value { get; set; }
    }

    public class Backgroundconfig
    {
        public int style { get; set; }
    }

    public class Quote
    {
        public string id { get; set; }
        public Data data { get; set; }
        public int likeCount { get; set; }
        public int bookId { get; set; }
        public int accountId { get; set; }
        public int commentCount { get; set; }
        public string creationDate { get; set; }
        public DateTime date { get; set; }
        public bool showComment { get; set; }
        public string coverUri { get; set; }
        public string bookName { get; set; }
        public string authorName { get; set; }
        public string publisherName { get; set; }
        public string profileImageUri { get; set; }
        public string nickname { get; set; }
        public string description { get; set; }
    }

    public class Data
    {
        public string quote { get; set; }
        public int startAtomId { get; set; }
        public int endAtomId { get; set; }
        public int chapterIndex { get; set; }
        public int endOffset { get; set; }
        public int startOffset { get; set; }
    }

}
