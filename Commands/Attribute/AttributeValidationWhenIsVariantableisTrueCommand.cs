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
    public class AttributeValidationWhenIsVariantableisTrueCommand : CommandBase<ServiceResult<bool>>
    {
        
        public string CategoryId{get; set;}

        public string BrandId{get; set;}

        public List<GetAllAttributesForCategoryValidationDto> Attributes {get; set;}

        public AttributeValidationWhenIsVariantableisTrueCommand(string categoryId, string brandId, List<GetAllAttributesForCategoryValidationDto> attributes)
        {
            CategoryId = categoryId;
            BrandId = brandId;
            Attributes = attributes;
        }

        public AttributeValidationWhenIsVariantableisTrueCommand()
        {
        }




        public class AttributeValidationWhenIsVariantableisTrueCommandHandler : BaseRequestCommandHandler<AttributeValidationWhenIsVariantableisTrueCommand, ServiceResult<Boolean>>
        {
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            public AttributeValidationWhenIsVariantableisTrueCommandHandler(ILogger<AttributeValidationWhenIsVariantableisTrueCommand> logger,IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected override async Task<ServiceResult<bool>> HandleAsync(AttributeValidationWhenIsVariantableisTrueCommand command, CancellationToken cancellationToken)
            {



                var sr = ServiceResult.Empty;
                var categoryRepository = unitOfWork.GetRepository<CategoryEntity>();
                var brandRepository = unitOfWork.GetRepository<BrandEntity>();
                var attributeRepository = unitOfWork.GetRepository<AttributeEntity>();
                var categoryAttributeRepo = unitOfWork.GetRepository<CategoryAttributeEntity>();
                var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();


                


                // Check if Category Exists
                if(!categoryRepository.GetQueryale()
                        .Any(x=> x.Id == command.CategoryId))

                        
                   return sr.SetError(Errors.CategoryNotFound.GetMessage() , Errors.CategoryNotFound.GetCode())
                                .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                                .To<Boolean>();                             




                // Check if Brand Exists
                if(!brandRepository.GetQueryale()
                            .Any(x=>x.Id == command.BrandId))


                    return sr.SetError(Errors.BrandNotFound.GetMessage() , Errors.BrandNotFound.GetCode())
                                .SetMessage(Errors.BrandNotFound.GetPersionMessage())
                                .To<Boolean>();



                
                // finding the specific Category List
                var categoryAttributesId = categoryAttributeRepo.GetQueryale().Where(x => x.CategoryId == command.CategoryId).Select(x => x.AttributeId ).ToList();
                var attributes = attributeRepository.GetQueryale().AsNoTracking().Where(x => categoryAttributesId.Contains(x.Id) && x.IsVariantable == true && x.IsRequired == true).ToList();


                // Check If Category has IsInCard Attribute
                var IsInCardAttibutes = attributes.Where(x => x.IsInCard == true).ToList();
                if(IsInCardAttibutes.Distinct().Count() > 1 )
                    return sr.SetError(Errors.OnlyOneAttributeCanBeInCard.GetMessage(), Errors.OnlyOneAttributeCanBeInCard.GetCode())
                             .SetMessage(Errors.OnlyOneAttributeCanBeInCard.GetPersionMessage())
                             .To<Boolean>();


               if(IsInCardAttibutes.Any(x => x.IsVariantable == true))
                    return sr.SetError(Errors.InCardAttributeCantBeVariantable.GetMessage(), Errors.InCardAttributeCantBeVariantable.GetCode())
                        .SetMessage(Errors.InCardAttributeCantBeVariantable.GetPersionMessage())
                        .To<Boolean>();



                var isValid = true;
            
                if(command.Attributes.Count() != attributes.Count())
                {   
                    isValid = false;
                    return sr.SetError(Errors.ValidationFailed.GetMessage(), Errors.ValidationFailed.GetCode())
                                    .SetMessage(Errors.ValidationFailed.GetPersionMessage())
                                    .To<Boolean>();         
                }

                command.Attributes.ForEach(x =>
                {

                    // TODO: Here should check in DB
                    var attributesWithSameKey = attributes.FirstOrDefault(d => d.Key == x.Key);
                    if (attributesWithSameKey == null)
                    {
                        isValid = false;
                        return;
                    }


                    if(x.Values == null || x.Values.Count() == 0 )
                    {
                        isValid = false;
                        return;
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
                                    // TODO: Here should check in DB
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



                    if (isValid == false)
                        return ;         


                });


                if(isValid == false)
                    return sr.SetError(Errors.ValidationFailed.GetMessage() , Errors.ValidationFailed.GetCode())
                                .SetMessage(Errors.ValidationFailed.GetPersionMessage())
                                .To<bool>();




                var brands = brandRepository.GetQueryale()
                                            .Where(x=>x.CategoryId == command.CategoryId)
                                            .AsNoTracking()
                                            .ToList();
                




                return ServiceResult.Create<bool>(true);


            }
             protected override ServiceResult<bool> HandleOnError(System.Exception exp)
            {
                var sr = ServiceResult.Empty;
                sr = sr.SetError(exp.Message);
                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<bool>();
            }
        }
    }
}