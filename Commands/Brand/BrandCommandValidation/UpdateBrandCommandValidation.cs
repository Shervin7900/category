using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CategoryApi.Commands.Brand.BrandCommandValidation
{
    public class UpdateBrandCommandValidation : AbstractValidator<UpdateBrandCommand>
    {

        public UpdateBrandCommandValidation()
        {


            RuleFor(x => x.BrandId)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());



            RuleFor(x => x.Fa_Name)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());



            RuleFor(x => x.En_Name)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());


        }


    }
}