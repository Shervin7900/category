using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace src.CategoryApi.Queries.AttributeQueries
{
    public class GetAllCategoryAttributesByAdminQuery : QueryBase<ServiceResult<List<AttributeDetailsDto>>>
    {
        public string CategoryId {get; set;}

        public GetAllCategoryAttributesByAdminQuery(string categoryId)
        {
            CategoryId = categoryId;
        }

        public GetAllCategoryAttributesByAdminQuery()
        {
        }


        public class GetAllCategoryAttributesByAdminQueryHandler : BaseRequestQueryHandler<GetAllCategoryAttributesByAdminQuery, ServiceResult<List<AttributeDetailsDto>>>
        {


            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;


            public GetAllCategoryAttributesByAdminQueryHandler(ILogger<GetAllCategoryAttributesByAdminQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }


            protected override async Task<ServiceResult<List<AttributeDetailsDto>>> HandleAsync(GetAllCategoryAttributesByAdminQuery query, CancellationToken cancellationToken)
            {


                var sr = ServiceResult.Empty;
                var attributeRepository = unitOfWork.GetRepository<AttributeEntity>();
                var categoryRepository = unitOfWork.GetRepository<CategoryEntity>();
                var categoryAttributeRepo = unitOfWork.GetRepository<CategoryAttributeEntity>();
                var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();




                // check if Attribute Exists 
                var categoryEntity = categoryRepository.GetQueryale()
                                        .Any(x=>x.Id == query.CategoryId);


                if(categoryEntity == false)
                    return sr.SetError(Errors.CategoryNotFound.GetMessage(), Errors.CategoryNotFound.GetCode())
                    .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                    .To<List<AttributeDetailsDto>>();

             
             
                // Get All Attributes in specific CategoryId
                var categoryAttributesId = categoryAttributeRepo.GetQueryale().Where(x => x.CategoryId == query.CategoryId ).Select(x => x.AttributeId ).ToList();
                var attributes = attributeRepository.GetQueryale().Where(x => categoryAttributesId.Contains(x.Id) && x.IsVariantable == false).ToList();


                // Change Values Type From String to List
                
                // attributes.ForEach(x => x.setValues());


                var searchableAttribute = attributes
                                            .Where(x => x.IsSearchableInValues == false )
                                            .Select(x => x.Key)
                                            .ToList();
             

                var Allvalues = attributeValueRepo.GetQueryale()
                                               .Where(x =>  searchableAttribute.Contains(x.Key) )
                                               .ToList()
                                               .GroupBy(x => x.Key)
                                               .Select( x => new {x.Key , values = x.Select(x => x.Value).ToList()})
                                               .ToList();

                // Check If Category has IsInCard Attribute
                var IsInCardAttibutes = attributes.Where(x => x.IsInCard == true).ToList();
                if(IsInCardAttibutes.Distinct().Count() > 1 )
                    return sr.SetError(Errors.OnlyOneAttributeCanBeInCard.GetMessage(), Errors.OnlyOneAttributeCanBeInCard.GetCode())
                        .SetMessage(Errors.OnlyOneAttributeCanBeInCard.GetPersionMessage())
                        .To<List<AttributeDetailsDto>>();

                if(IsInCardAttibutes.Any(x => x.IsVariantable == true))
                    return sr.SetError(Errors.InCardAttributeCantBeVariantable.GetMessage(), Errors.InCardAttributeCantBeVariantable.GetCode())
                        .SetMessage(Errors.InCardAttributeCantBeVariantable.GetPersionMessage())
                        .To<List<AttributeDetailsDto>>();

                // Set Values
                attributes.ForEach( x => 
                {
                    var AttributeValues = Allvalues.FirstOrDefault(s => s.Key == x.Key);
                    if(AttributeValues != null)
                    {
                        x.setValues(AttributeValues.values);
                    }
                    else
                    {
                        x.setValues(new List<string>() {});
                    }
                });



                return ServiceResult.Create<List<AttributeDetailsDto>>(attributes.Adapt<List<AttributeDetailsDto>>());



            }

            protected override ServiceResult<List<AttributeDetailsDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<List<AttributeDetailsDto>>();
                
            }
        }
    }
}