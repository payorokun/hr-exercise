using Crud.Application.Dtos;
using Crud.Domain.Models;
using FluentValidation;

namespace Crud.Application.Validators;

public class QuoteValidator : AbstractValidator<CreateUpdateQuoteDto>
{
    public QuoteValidator()
    {
        RuleFor(l => l.Id).GreaterThan(0);
        RuleFor(l => l.Author).NotEmpty().MaximumLength(100);
        RuleFor(l => l.Text).MaximumLength(500);
    }
}
