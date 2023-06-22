using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CategoryApi.Commands.Attribute
{
    public class AddValueToAttributeCommandValidation: AbstractValidator<AddValueToAttributeCommand>
    {



        public AddValueToAttributeCommandValidation()
        {





            RuleFor(x=>x.Key)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());



            RuleFor(x=>x.Values)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());
        }
        
    }
}