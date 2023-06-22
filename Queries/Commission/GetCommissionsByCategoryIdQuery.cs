using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.Commission;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.Commission
{
    public class GetCommissionsByCategoryIdQuery : QueryBase<ServiceResult<List<GetCommissionsByCategoryIdDto>>>
    {
        public List<String> CategoryIds { get; set; }

        public GetCommissionsByCategoryIdQuery()
        {
        }

        public GetCommissionsByCategoryIdQuery(List<string> categoryIds)
        {
            CategoryIds = categoryIds;
        }

        public class GetCommissionsByCategoryIdQueryHandler : BaseRequestQueryHandler<GetCommissionsByCategoryIdQuery, ServiceResult<List<GetCommissionsByCategoryIdDto>>>
        {

            private readonly IUnitOfWork<CategoryDbContext> _unitOfWork;

            public GetCommissionsByCategoryIdQueryHandler(ILogger<GetCommissionsByCategoryIdQueryHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {

                _unitOfWork = unitOfWork;

            }

            protected override async Task<ServiceResult<List<GetCommissionsByCategoryIdDto>>> HandleAsync(GetCommissionsByCategoryIdQuery query, CancellationToken cancellationToken)
            {

                var sr = ServiceResult.Empty;

                var commissionRepo = _unitOfWork.GetRepository<CommissionEntity>();

                var commissionList = commissionRepo.GetQueryale().Where(x => query.CategoryIds.Distinct().Contains(x.CategoryId)).ToList();


                if (query.CategoryIds.Distinct().Count() != commissionList.Count())
                    return sr.SetError(Errors.CategoryNotFound.GetMessage(), Errors.CategoryNotFound.GetCode())
                            .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                            .To<List<GetCommissionsByCategoryIdDto>>();

                foreach(var item in commissionList)
                {
                    item.SetTotalCommission(item.BooxellCommission + item.AffiliatorCommission + item.AnotherShopCommission);
                }

                var AdaptedCommissionList = commissionList.Adapt<List<GetCommissionsByCategoryIdDto>>();

                return ServiceResult.Create<List<GetCommissionsByCategoryIdDto>>(AdaptedCommissionList);



            }

            protected override ServiceResult<List<GetCommissionsByCategoryIdDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<List<GetCommissionsByCategoryIdDto>>();

            }
        }

    }
}