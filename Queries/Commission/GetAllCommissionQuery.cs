using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.Commission;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.Commission
{
    public class GetAllCommissionQuery: QueryBase<ServiceResult<IPagedList<GetAllCommissionOutPutDto>>>
    {
        public SearchFilter sf { get; set; }

        public GetAllCommissionQuery(SearchFilter sf)
        {
            this.sf = sf;
        }

        public GetAllCommissionQuery()
        {
        }

        public class GetAllCommissionQueryHandler : BaseRequestQueryHandler<GetAllCommissionQuery, ServiceResult<IPagedList<GetAllCommissionOutPutDto>>>
        {

            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;

            public GetAllCommissionQueryHandler(ILogger<GetAllCommissionQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }

            protected override async Task<ServiceResult<IPagedList<GetAllCommissionOutPutDto>>> HandleAsync(GetAllCommissionQuery query, CancellationToken cancellationToken)
            {



                var sr = ServiceResult.Empty;
                
                // Search Filter
                query.sf = query.sf.CheckNullDefault();

                var commissionRepo = unitOfWork.GetRepository<CommissionEntity>();

                var commissions = query.sf.Translate<CommissionEntity>();
                commissions = commissions != null ? commissions : x => true;



                // commissions
                var commissionList = commissionRepo.GetPagedList
                (
                    commissions,
                    null,
                    pageIndex: query.sf.PgNumber,
                    pageSize: query.sf.PgSize,
                    disableTracking: true
                );



                foreach(var item in commissionList.Items)
                {
                    item.SetTotalCommission(item.BooxellCommission + item.AffiliatorCommission + item.AnotherShopCommission);
                }

                


                var AdaptedCommissionList = commissionList.Adapt<IPagedList<GetAllCommissionOutPutDto>>();


                return ServiceResult.Create<IPagedList<GetAllCommissionOutPutDto>>(AdaptedCommissionList);



            }

            protected override ServiceResult<IPagedList<GetAllCommissionOutPutDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<IPagedList<GetAllCommissionOutPutDto>>();

            }
        }
        
    }
}