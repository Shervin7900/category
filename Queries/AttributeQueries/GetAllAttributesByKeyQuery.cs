using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.AttributeQueries
{
    public class GetAllAttributesByKeyQuery : QueryBase<ServiceResult<IPagedList<GetAllAttributesByKeyDto>>>
    {
        public TermFilter tf { get; set; }
        public List<string> Keys { get; set; }

        public GetAllAttributesByKeyQuery(TermFilter tf, List<string> keys)
        {
            this.tf = tf;
            Keys = keys;
        }

        public GetAllAttributesByKeyQuery()
        {
        }

        public class GetAllAttributesByKeyQueryHandler : BaseRequestQueryHandler<GetAllAttributesByKeyQuery, ServiceResult<IPagedList<GetAllAttributesByKeyDto>>>
        {

            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;

            public GetAllAttributesByKeyQueryHandler(ILogger<GetAllAttributesByKeyQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }

            protected override async Task<ServiceResult<IPagedList<GetAllAttributesByKeyDto>>> HandleAsync(GetAllAttributesByKeyQuery query, CancellationToken cancellationToken)
            {



                var sr = ServiceResult.Empty;
                
                // Term Filter
                query.tf = query.tf?.CheckNullDefault();

                var attributeRepo = unitOfWork.GetRepository<AttributeEntity>();



                // attributes with specific Keys
                var attributeList = attributeRepo.GetPagedList
                (
                    x=>
                    query.Keys.Contains(x.Key),
                    x => x.OrderBy(y => y.CreatedAt),
                    null,
                    pageIndex: query.tf.PgNumber,
                    pageSize: query.tf.PgSize,
                    disableTracking: true
                );


                var AdaptedAttributeDetails = attributeList.Adapt<IPagedList<GetAllAttributesByKeyDto>>();


                var attributeDetails = AdaptedAttributeDetails.Items
                .Select(x=>new GetAllAttributesByKeyDto(x.Key , x.Fa_Name , x.IsSearchable , x.Type 
                                                       ,x.IsVariantable , x.IsRequired , x.IsUnique ,x.IsSearchableInValues,x.IsInCard)).ToList();

                


                return ServiceResult.Create<IPagedList<GetAllAttributesByKeyDto>>(AdaptedAttributeDetails);



            }

            protected override ServiceResult<IPagedList<GetAllAttributesByKeyDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<IPagedList<GetAllAttributesByKeyDto>>();

            }
        }
    }
}