// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using FluentValidation;

// namespace CategoryApi.Commands.Attribute.AttributeCommandValidation
// {
//     public class UpdateAttributeCommandValidation : AbstractValidator<UpdateAttributeByAdminCommand>
//     {



//         public UpdateAttributeCommandValidation()
//         {



//             RuleFor(x=>x.Key)
//                 .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
//                 .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());



//             RuleFor(x=>x.Fa_Name)
//                 .NotNull().WithMessage(Errors.RequiredItemIsNull.GetMessage())
//                 .NotEmpty().WithMessage(Errors.RequiredItemIsEmpty.GetMessage());




//         }




        
//     }
// }