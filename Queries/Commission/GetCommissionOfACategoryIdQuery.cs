using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.Commission;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.Commission
{
    public class GetCommissionOfACategoryIdQuery: QueryBase<ServiceResult<GetCommissionOfACategoryIdOutPutDto>>
    {
        public string CategoryId { get; set; }

        public GetCommissionOfACategoryIdQuery()
        {
        }

        public GetCommissionOfACategoryIdQuery(string categoryId)
        {
            CategoryId = categoryId;
        }

        public class GetCommissionOfACategoryIdQueryHandler : BaseRequestQueryHandler<GetCommissionOfACategoryIdQuery, ServiceResult<GetCommissionOfACategoryIdOutPutDto>>
        {

            private readonly IUnitOfWork<CategoryDbContext> _unitOfWork;

            public GetCommissionOfACategoryIdQueryHandler(ILogger<GetCommissionOfACategoryIdQueryHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {

                _unitOfWork = unitOfWork;

            }

            protected override async Task<ServiceResult<GetCommissionOfACategoryIdOutPutDto>> HandleAsync(GetCommissionOfACategoryIdQuery query, CancellationToken cancellationToken)
            {

                var sr = ServiceResult.Empty;

                var commissionRepo = _unitOfWork.GetRepository<CommissionEntity>();


                // commission Entity
                var commissionEntity = commissionRepo.GetQueryale().FirstOrDefault(x => x.CategoryId == query.CategoryId);


                if (commissionEntity == null)
                    return sr.SetError(Errors.CommissionNotFound.GetMessage(), Errors.CommissionNotFound.GetCode())
                            .SetMessage(Errors.CommissionNotFound.GetPersionMessage())
                            .To<GetCommissionOfACategoryIdOutPutDto>();

                commissionEntity.SetTotalCommission(commissionEntity.BooxellCommission + commissionEntity.AffiliatorCommission + commissionEntity.AnotherShopCommission);

                var adaptedCommission = commissionEntity.Adapt<GetCommissionOfACategoryIdOutPutDto>();


                return ServiceResult.Create<GetCommissionOfACategoryIdOutPutDto>(adaptedCommission);



            }

            protected override ServiceResult<GetCommissionOfACategoryIdOutPutDto> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<GetCommissionOfACategoryIdOutPutDto>();

            }
        }
        
    }
}