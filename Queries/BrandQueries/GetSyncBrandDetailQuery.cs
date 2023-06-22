using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using MH.DDD.DataAccessLayer.UnitOfWork;
using src.CategoryApi.Dtos.BrandDtos;

namespace CategoryApi.Queries.SyncShop
{
    public class GetSyncBrandDetailQuery : QueryBase<ServiceResult<IPagedList<BrandDetailSyncDto>>>
    {
        public GetSyncBrandDetailQuery(TermFilter tr, DateTime time)
        {
            Tr = tr;
            Time = time;
        }

        public TermFilter Tr { get; set; }
        public DateTime Time { get; set; }

        public class GetSyncBrandDetailQueryHandler : BaseRequestQueryHandler<GetSyncBrandDetailQuery, ServiceResult<IPagedList<BrandDetailSyncDto>>>
        {
            private readonly IUnitOfWork<CategoryDbContext> _unitOfWork;
            public GetSyncBrandDetailQueryHandler(Microsoft.Extensions.Logging.ILogger<GetSyncBrandDetailQueryHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                _unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<IPagedList<BrandDetailSyncDto>>> HandleAsync(GetSyncBrandDetailQuery query, CancellationToken cancellationToken)
            {


                var sr = ServiceResult.Empty;
                var BrandRepo = _unitOfWork.GetRepository<BrandEntity>();
                query.Tr = query.Tr.CheckNullDefault();


                var brandEntity = BrandRepo.GetPagedList(
                    x =>
                    x.Adapt<BrandDetailSyncDto>(),
                    predicate: x => x.ModifiedAt > query.Time,
                    null,
                    null,
                    query.Tr.PgNumber,
                    query.Tr.PgSize,
                    true);


                return ServiceResult.Create<IPagedList<BrandDetailSyncDto>>(brandEntity);


            }
        }
    }
}