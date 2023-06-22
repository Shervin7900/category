using System.Net.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;
using Type = CategoryApi.Domains.Entities.Type;

namespace CategoryApi.Commands.Attribute
{
    public class AttributeValidationWhenIsVariantableisFalseCommand : CommandBase<ServiceResult<AttributeValidationResponseModelDto>>
    {

        public string CategoryId { get; set; }

        public string BrandId { get; set; }

        public List<GetAllAttributesForCategoryValidationDto> Attributes { get; set; }

        public AttributeValidationWhenIsVariantableisFalseCommand(string categoryId, string brandId, List<GetAllAttributesForCategoryValidationDto> attributes)
        {
            CategoryId = categoryId;
            BrandId = brandId;
            Attributes = attributes;
        }

        public AttributeValidationWhenIsVariantableisFalseCommand()
        {
        }




        public class AttributeValidationWhenIsVariantableisFalseCommandHandler : BaseRequestCommandHandler<AttributeValidationWhenIsVariantableisFalseCommand, ServiceResult<AttributeValidationResponseModelDto>>
        {
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            public AttributeValidationWhenIsVariantableisFalseCommandHandler(ILogger<AttributeValidationWhenIsVariantableisFalseCommand> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected override async Task<ServiceResult<AttributeValidationResponseModelDto>> HandleAsync(AttributeValidationWhenIsVariantableisFalseCommand command, CancellationToken cancellationToken)
            {



                var sr = ServiceResult.Empty;
                var categoryRepository = unitOfWork.GetRepository<CategoryEntity>();
                var brandRepository = unitOfWork.GetRepository<BrandEntity>();
                var attributeRepository = unitOfWork.GetRepository<AttributeEntity>();
                var categoryAttributeRepo = unitOfWork.GetRepository<CategoryAttributeEntity>();
                var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();




                // Check if Category Exists
                if (!categoryRepository.GetQueryale().Any(x => x.Id == command.CategoryId))
                    return sr.SetError(Errors.CategoryNotFound.GetMessage(), Errors.CategoryNotFound.GetCode())
                                 .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                                 .To<AttributeValidationResponseModelDto>();




                // Check if Brand Exists
                if (!brandRepository.GetQueryale().Any(x => x.Id == command.BrandId))
                    return sr.SetError(Errors.BrandNotFound.GetMessage(), Errors.BrandNotFound.GetCode())
                                .SetMessage(Errors.BrandNotFound.GetPersionMessage())
                                .To<AttributeValidationResponseModelDto>();




                // finding the specific Category List
                var categoryAttributesId = categoryAttributeRepo.GetQueryale().Where(x => x.CategoryId == command.CategoryId).Select(x => x.AttributeId ).ToList();
                var attributes = attributeRepository.GetQueryale().AsNoTracking().Where(x => categoryAttributesId.Contains(x.Id) && x.IsVariantable == false).ToList();




                // Check If Category has IsInCard Attribute
                var IsInCardAttibutes = attributes.Where(x => x.IsInCard == true).ToList();
                if(IsInCardAttibutes.Distinct().Count() > 1 )
                    return sr.SetError(Errors.OnlyOneAttributeCanBeInCard.GetMessage(), Errors.OnlyOneAttributeCanBeInCard.GetCode())
                             .SetMessage(Errors.OnlyOneAttributeCanBeInCard.GetPersionMessage())
                             .To<AttributeValidationResponseModelDto>();

               if(IsInCardAttibutes.Any(x => x.IsVariantable == true))
                    return sr.SetError(Errors.InCardAttributeCantBeVariantable.GetMessage(), Errors.InCardAttributeCantBeVariantable.GetCode())
                        .SetMessage(Errors.InCardAttributeCantBeVariantable.GetPersionMessage())
                        .To<AttributeValidationResponseModelDto>();



                var isValid = true;


                if(command.Attributes.DistinctBy(s => s.Key).Count() != attributes.Count())
                {   
                    isValid = false;
                    return sr.SetError(Errors.ValidationFailed.GetMessage(), Errors.ValidationFailed.GetCode())
                                    .SetMessage(Errors.ValidationFailed.GetPersionMessage())
                                    .To<AttributeValidationResponseModelDto>();         
                }

                
                string uniqueKey = String.Empty;



                command.Attributes.ForEach(x =>
                {


                    var attributesWithSameKey = attributes.FirstOrDefault(d => d.Key == x.Key);
                    if (attributesWithSameKey == null)
                    {
                        isValid = false;
                        return;
                    }


                    if(attributesWithSameKey.IsUnique == true)
                        uniqueKey = attributesWithSameKey.Key;


                    // check if Attribute is Required and Value is null or Has No Value
                    if (attributesWithSameKey.IsRequired == true && ( x.Values.Count() == 0 || x.Values == null ))
                    { 
                        isValid = false;
                        return;
                    }


                    // check if Attribute is Unique
                    if (attributesWithSameKey.IsUnique == true && x.Values.Count() != 1)
                    {
                        isValid = false;
                        return;
                  
                    }


                    // Input
                    if (attributesWithSameKey.Type == Domains.Entities.Type.Input)
                    {

                        if(attributesWithSameKey.IsRequired && ( x.Values.Count() != 1 || x.Values.Count() > 1 ) )
                        {
                            isValid = false;
                            return;
                        }


                        if(!attributesWithSameKey.IsRequired && x.Values.Count() > 1 )
                        {
                            isValid = false;
                            return;
                        }

                    }


                    // Single
                    if(attributesWithSameKey.Type == Type.Single)
                    {



                        if(attributesWithSameKey.IsRequired)
                        {

                            if(x.Values.Count() != 1)
                            {
                                isValid = false;
                                return;
                            }
                            
                            // TODO: Here should check in DB

                            if(attributeValueRepo.GetQueryale().Where(d => d.Key == x.Key &&  x.Values.Distinct().Contains(d.Value)).Count() != x.Values.Distinct().Count())
                            {
                                isValid = false;
                                return;
                            }

                        }
                        else
                        {
                            switch(x.Values.Count())
                            {
                                
                                case 0 :
                                  break;

                                case 1 :
                                
                                    if(attributeValueRepo.GetQueryale().Where(d => d.Key == x.Key &&  x.Values.Distinct().Contains(d.Value)).Count() != x.Values.Distinct().Count())
                                    {
                                        isValid = false;
                                        return;
                                    }
                                  break ;

                                default:
                                    isValid = false;
                                    return;
                            }    
                           
                        }
                        


                    }


                    // Multi-Select
                    if(attributesWithSameKey.Type == Type.MultiSelect)
                    {

                        if(attributesWithSameKey.IsRequired && x.Values.Count() > 0)
                        {
                            
                            // TODO: Should be check
                            if(x.Values.Distinct().Count() != x.Values.Count()) 
                            {
                                isValid = false;
                                return;
                            }

                            if(attributeValueRepo.GetQueryale().Where(d => d.Key == x.Key &&  x.Values.Distinct().Contains(d.Value)).Count() != x.Values.Distinct().Count())
                            {
                                isValid = false;
                                return;
                            }
                        }else
                        {
                            if(!attributesWithSameKey.IsRequired)
                            {


                                switch(x.Values.Count())
                                {

                                    case > 0 :

                                        // TODO: Should be check
                                        if(x.Values.Distinct().Count() != x.Values.Count()) 
                                        {
                                            isValid = false;
                                            return;
                                        }

                                        if(attributeValueRepo.GetQueryale().Where(d => d.Key == x.Key &&  x.Values.Distinct().Contains(d.Value)).Count() != x.Values.Distinct().Count())
                                        {
                                            isValid = false;
                                            return;
                                        }
                                        break;

                                    case 0 : 
                                        break;

                                }

                            
                                
                            }
    
                        }

                        


                    }





                    if (isValid == false)
                        return ;


                });
                

                if (isValid == false)
                    return sr.SetError(Errors.ValidationFailed.GetMessage(), Errors.ValidationFailed.GetCode())
                                .SetMessage(Errors.ValidationFailed.GetPersionMessage())
                                .To<AttributeValidationResponseModelDto>();

                
                var validate = ResponseModel(isValid , uniqueKey == String.Empty ? null : uniqueKey);


                return ServiceResult.Create<AttributeValidationResponseModelDto>(validate);



            }


            public AttributeValidationResponseModelDto ResponseModel(bool isValid , string? isUniqueKey)
            {
                
                var validate = new AttributeValidationResponseModelDto();
                validate.isValid = isValid.Adapt(validate.isValid);
                validate.UniqKey = isUniqueKey.Adapt(validate.UniqKey);
                return validate;
                
            }




            protected override ServiceResult<AttributeValidationResponseModelDto> HandleOnError(System.Exception exp)
            {
                var sr = ServiceResult.Empty;
                sr = sr.SetError(exp.Message);
                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<AttributeValidationResponseModelDto>();
            }
        }
    }
}