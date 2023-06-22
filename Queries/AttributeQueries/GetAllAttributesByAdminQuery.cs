using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.AttributeQueries
{
    public class GetAllAttributesByAdminQuery : QueryBase<ServiceResult<List<GetAllAttributesDto>>>
    {


        public class GetAllAttributesByAdminHandler : BaseRequestQueryHandler<GetAllAttributesByAdminQuery, ServiceResult<List<GetAllAttributesDto>>>
        {
            
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;

            public GetAllAttributesByAdminHandler(Microsoft.Extensions.Logging.ILogger<GetAllAttributesByAdminHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<List<GetAllAttributesDto>>> HandleAsync(GetAllAttributesByAdminQuery query, CancellationToken cancellationToken)
            {

                var sr = ServiceResult.Empty;
                var attributesRepo = unitOfWork.GetRepository<AttributeEntity>();
                var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();



                var allAttributes =  attributesRepo.GetQueryale().AsNoTracking().ToList();
               

               
                var searchableAttribute = allAttributes
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
                allAttributes.ForEach( x => 
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



                return sr.To<List<GetAllAttributesDto>>(allAttributes.Adapt<List<GetAllAttributesDto>>());   

            }

        }

    }
}