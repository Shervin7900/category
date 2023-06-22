using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.CategoryQueries
{
    public class GetCategoryFiltersQuery : QueryBase<ServiceResult<List<GetCustomerFiltersDto>>>
    {
        public string categoryId;

        public GetCategoryFiltersQuery(string categoryId)
        {
            this.categoryId = categoryId;
        }

        public class GetCategoryFiltersQueryHandler : BaseRequestQueryHandler<GetCategoryFiltersQuery, ServiceResult<List<GetCustomerFiltersDto>>>
        {
            
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;

            public GetCategoryFiltersQueryHandler(Microsoft.Extensions.Logging.ILogger<GetCategoryFiltersQueryHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<List<GetCustomerFiltersDto>>> HandleAsync(GetCategoryFiltersQuery query, CancellationToken cancellationToken)
            {
                


                var sr = ServiceResult.Empty;
                var attributesRepo = unitOfWork.GetRepository<AttributeEntity>();
                var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();
                var categoryAttributeRepo = unitOfWork.GetRepository<CategoryAttributeEntity>();
                
                

                // Get Category Attribute
                var categoryAttributesId = categoryAttributeRepo.GetQueryale().Where(x => x.CategoryId == query.categoryId ).Select(x => x.AttributeId ).ToList();
                var attributes = attributesRepo.GetQueryale().AsNoTracking().Where(x => categoryAttributesId.Contains(x.Id) && x.IsSearchable == true ).ToList();


                var searchableAttribute = attributes
                                            .Where(x => x.IsSearchableInValues == false )
                                            .Select(x => x.Key)
                                            .ToList();
             
             
                // Get SearchAbleInVariant Variant Attributes
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


                return sr.To<List<GetCustomerFiltersDto>>(attributes.Adapt<List<GetCustomerFiltersDto>>());  

            }
        }
    }
}