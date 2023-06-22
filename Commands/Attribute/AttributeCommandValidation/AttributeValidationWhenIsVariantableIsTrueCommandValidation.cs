using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CategoryApi.Commands.Attribute.AttributeCommandValidation
{
    public class AttributeValidationWhenIsVariantableIsTrueCommandValidation : AbstractValidator<AttributeValidationWhenIsVariantableisTrueCommand>
    {
        public AttributeValidationWhenIsVariantableIsTrueCommandValidation()
        {
            


            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsNull.GetMessage());



            RuleFor(x => x.BrandId)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsNull.GetMessage());



        }
    }
}