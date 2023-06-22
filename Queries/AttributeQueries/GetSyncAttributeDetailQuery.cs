using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using MH.DDD.DataAccessLayer.UnitOfWork;
using src.CategoryApi.Dtos.BrandDtos;

namespace CategoryApi.Queries.SyncShop
{
    public class GetSyncAttributeDetailQuery : QueryBase<ServiceResult<IPagedList<GetAttributeDetailWithoutValuesSyncDto>>>
    {
        public GetSyncAttributeDetailQuery(TermFilter tr, DateTime time)
        {
            Tr = tr;
            Time = time;
        }

        public TermFilter Tr { get; set; }
        public DateTime Time { get; set; }

        public class GetSyncAttributeDetailQueryHandler : BaseRequestQueryHandler<GetSyncAttributeDetailQuery, ServiceResult<IPagedList<GetAttributeDetailWithoutValuesSyncDto>>>
        {
            private readonly IUnitOfWork<CategoryDbContext> _unitOfWork;
            public GetSyncAttributeDetailQueryHandler(Microsoft.Extensions.Logging.ILogger<GetSyncAttributeDetailQueryHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                _unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<IPagedList<GetAttributeDetailWithoutValuesSyncDto>>> HandleAsync(GetSyncAttributeDetailQuery query, CancellationToken cancellationToken)
            {


                var sr = ServiceResult.Empty;
                var AttributeRepo = _unitOfWork.GetRepository<AttributeEntity>();
                query.Tr = query.Tr.CheckNullDefault();


                var attributeEntity = AttributeRepo.GetPagedList(
                    x =>
                    x.Adapt<GetAttributeDetailWithoutValuesSyncDto>(),
                    predicate: x => 
                    x.ModifiedAt > query.Time ,
                    null,
                    null,
                    query.Tr.PgNumber,
                    query.Tr.PgSize,
                    true);


                return ServiceResult.Create<IPagedList<GetAttributeDetailWithoutValuesSyncDto>>(attributeEntity);


            }
        }
    }
}