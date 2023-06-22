using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CategoryApi.Commands.Commission.CommissionCommandValidation
{
    public class UpdateCommissionCommandValidation: AbstractValidator<UpdateCommissionCommand>
    {
        public UpdateCommissionCommandValidation()
        {
            


            RuleFor(x => x.Id)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsNull.GetMessage());


        }
        
    }
}