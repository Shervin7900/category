using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CategoryApi.Commands.Attribute.AttributeCommandValidation
{
    public class AttributeValidationWhenIsVariantableIsFalseCommandValidation : AbstractValidator<AttributeValidationWhenIsVariantableisFalseCommand>
    {
        public AttributeValidationWhenIsVariantableIsFalseCommandValidation()
        {



            RuleFor(x=>x.CategoryId)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());



            RuleFor(x=>x.BrandId)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());



        }
    }
}