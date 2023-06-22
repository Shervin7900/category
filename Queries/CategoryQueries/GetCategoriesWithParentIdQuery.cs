using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AggregatedDtos;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;
using src.CategoryApi.Dtos.BrandDtos;

namespace CategoryApi.Queries.CategoryQueries
{
    public class GetCategoriesWithParentIdQuery : QueryBase<ServiceResult<List<string>>>
    {
        public List<string> ParentCategoryIds { get; set; }

        public GetCategoriesWithParentIdQuery(List<string> parentCategoryIds)
        {
            ParentCategoryIds = parentCategoryIds;
        }
        

        public class GetCategoriesWithParentIdQueryHandler : BaseRequestQueryHandler<GetCategoriesWithParentIdQuery, ServiceResult<List<string>>>
        {
            private readonly IUnitOfWork<CategoryDbContext> _unitOfWork;

            public GetCategoriesWithParentIdQueryHandler(Microsoft.Extensions.Logging.ILogger<GetCategoriesWithParentIdQueryHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this._unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<List<string>>> HandleAsync(GetCategoriesWithParentIdQuery query, CancellationToken cancellationToken)
            {

                // Used In Search Priority -> Get Category Ids Of This Parent Category Id

                var sr = ServiceResult.Empty;
                var categoryRepository = _unitOfWork.GetRepository<CategoryEntity>();


                var allCategoryIdsWithThisParentId = categoryRepository
                                                                .GetQueryale()
                                                                .Where(x=> query.ParentCategoryIds.Contains(x.ParentId))
                                                                .Select(x=>x.Id)
                                                                .ToList();


                return sr.To<List<string>>(allCategoryIdsWithThisParentId);


            }

            protected override ServiceResult<List<string>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<List<string>>();

            }
        }
    }
}