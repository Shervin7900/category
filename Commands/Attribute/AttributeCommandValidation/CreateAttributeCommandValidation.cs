using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CategoryApi.Commands.Attribute.AttributeCommandValidation
{
    public class CreateAttributeCommandValidation : AbstractValidator<CreateAttributeCommand>
    {



        public CreateAttributeCommandValidation()
        {





            RuleFor(x=>x.Key)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());



            RuleFor(x=>x.Fa_Name)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());




        }



        
    }
}