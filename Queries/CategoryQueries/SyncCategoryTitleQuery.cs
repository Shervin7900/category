using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.CategoryQueries
{
    public class SyncCategoryTitleQuery : QueryBase<ServiceResult<IPagedList<GetCategoryDetailSyncDto>>>
    {
        public SyncCategoryTitleQuery(TermFilter tr, DateTime time)
        {
            Tr = tr;
            Time = time;
        }

        public TermFilter Tr { get; set; }
        public DateTime Time { get; set; }



        public class SyncCategoryTitleQueryHandler : BaseRequestQueryHandler<SyncCategoryTitleQuery, ServiceResult<IPagedList<GetCategoryDetailSyncDto>>>
        {
            private readonly IUnitOfWork<CategoryDbContext> _unitOfWork;

            public SyncCategoryTitleQueryHandler(ILogger<SyncCategoryTitleQueryHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this._unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<IPagedList<GetCategoryDetailSyncDto>>> HandleAsync(SyncCategoryTitleQuery query, CancellationToken cancellationToken)
            {


                //logic
                var sr = ServiceResult.Empty;

                var categoryRepository = _unitOfWork.GetRepository<CategoryEntity>();


                // Arrange Search Filter
                query.Tr = query.Tr?.CheckNullDefault();



                // finding Category
                //filter by CreatedAt and ModifiedAt
                var categoryList = categoryRepository.GetPagedList
               (
                   x => new GetCategoryDetailSyncDto(x.Id, x.Title),
                    predicate: x => x.ModifiedAt > query.Time,
                   null,
                   pageIndex: query.Tr.PgNumber,
                   pageSize: query.Tr.PgSize,
                   disableTracking: true
               );




                return sr.To<IPagedList<GetCategoryDetailSyncDto>>(categoryList);


            }

            protected override ServiceResult<IPagedList<GetCategoryDetailSyncDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<IPagedList<GetCategoryDetailSyncDto>>();

            }
        }
    }
}