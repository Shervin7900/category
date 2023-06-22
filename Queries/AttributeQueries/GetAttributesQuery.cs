using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.AttributeQueries
{
    public class GetAttributesQuery : QueryBase<ServiceResult<List<GetAttributesDto>>>
    {
        public string CategoryId{get; set;}

        public GetAttributesQuery(string categoryId)
        {
            CategoryId = categoryId;
        }

        public GetAttributesQuery()
        {
        }


        public class GetAttributesQueryHandler : BaseRequestQueryHandler<GetAttributesQuery, ServiceResult<List<GetAttributesDto>>>
        {

            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;

            public GetAttributesQueryHandler(ILogger<GetAttributesQueryHandler> logger,IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }

            protected override async Task<ServiceResult<List<GetAttributesDto>>> HandleAsync(GetAttributesQuery query, CancellationToken cancellationToken)
            {
            
               var sr = ServiceResult.Empty;
               var AttributeRepo = unitOfWork.GetRepository<AttributeEntity>();
               var categoryAttributeRepo = unitOfWork.GetRepository<CategoryAttributeEntity>();
               var categoryRepo = unitOfWork.GetRepository<CategoryEntity>();
               var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();




                //finding the specific Category
                var CategoryEntity = categoryRepo.GetQueryale().AsNoTracking()
                                  .Any(x=>x.Id == query.CategoryId);

                                                 
                if(CategoryEntity == false)
                    return sr.SetError(Errors.CategoryNotFound.GetMessage() , Errors.CategoryNotFound.GetCode())
                    .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                    .To<List<GetAttributesDto>>();



                // finding the specific Category List
                var categoryAttributesId = categoryAttributeRepo.GetQueryale().Where(x => x.CategoryId == query.CategoryId).Select(x => x.AttributeId ).ToList();
                var attributes = AttributeRepo.GetQueryale().AsNoTracking().Where(x => categoryAttributesId.Contains(x.Id)).ToList();


                // Check If Category has IsInCard Attribute
                var IsInCardAttibutes = attributes.Where(x => x.IsInCard == true).ToList();
                if(IsInCardAttibutes.Distinct().Count() > 1 )
                    return sr.SetError(Errors.OnlyOneAttributeCanBeInCard.GetMessage(), Errors.OnlyOneAttributeCanBeInCard.GetCode())
                        .SetMessage(Errors.OnlyOneAttributeCanBeInCard.GetPersionMessage())
                        .To<List<GetAttributesDto>>();


                if(IsInCardAttibutes.Any(x => x.IsVariantable == true))
                    return sr.SetError(Errors.InCardAttributeCantBeVariantable.GetMessage(), Errors.InCardAttributeCantBeVariantable.GetCode())
                        .SetMessage(Errors.InCardAttributeCantBeVariantable.GetPersionMessage())
                        .To<List<GetAttributesDto>>();




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



                return ServiceResult.Create<List<GetAttributesDto>>(attributes.Adapt<List<GetAttributesDto>>());


            }

            protected override ServiceResult<List<GetAttributesDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<List<GetAttributesDto>>();
                
            }
        }
    }
}