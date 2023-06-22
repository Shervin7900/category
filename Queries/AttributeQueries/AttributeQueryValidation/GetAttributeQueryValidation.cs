using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CategoryApi.Queries.AttributeQueries.AttributeQueryValidation
{
    public class GetAttributeQueryValidation : AbstractValidator<GetAttributesQuery>
    {
        public GetAttributeQueryValidation()
        {
            RuleFor(x=>x.CategoryId)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());
        }
    }
}