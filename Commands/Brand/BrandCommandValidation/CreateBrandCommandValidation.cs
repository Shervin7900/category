using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CategoryApi.Commands.Brand.BrandCommandValidation
{
    public class CreateBrandCommandValidation : AbstractValidator<CreateBrandCommand>
    {
        public CreateBrandCommandValidation()
        {


            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsNull.GetMessage());



            RuleFor(x => x.Fa_Name)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsNull.GetMessage());



            RuleFor(x => x.En_Name)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsNull.GetMessage());
        }
    }
}