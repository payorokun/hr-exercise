namespace Crud.Application.Dtos;
public record CreateUpdateQuoteDto
{
    public int Id { get; set; }
    public string Author { get; set; }
    public string Text { get; set; }
}
