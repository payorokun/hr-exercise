namespace Crud.Domain.Models
{
    public class Quote:BaseEntity
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public string TextLength { get; set; }
    }
    public class BaseEntity{}
}