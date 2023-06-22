using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CategoryApi.Commands.Category.CategoryCommandValidation
{
    public class UpdateCategoryCommandValidation : AbstractValidator<UpdateCategoryCommand>
    {


      public UpdateCategoryCommandValidation()
      {

          
            RuleFor(x => x.CategoryId)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());



            RuleFor(x => x.Title)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());



            RuleFor(x => x.Description)
                .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
                .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());


      }  


    }
}