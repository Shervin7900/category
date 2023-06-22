using System.Linq.Expressions;
using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.AttributeQueries
{
    public class GetAttributeValues : QueryBase<ServiceResult<IPagedList<string>>>
    {
        public TermFilter tf { get; set; }
        public string Key { get; set; }

        public GetAttributeValues(string Key , TermFilter Tf)
        {
            this.tf = Tf;
            this.Key = Key ;
        }



        public class GetAttributeValuesHandler : BaseRequestQueryHandler<GetAttributeValues, ServiceResult<IPagedList<string>>>
        {
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            public GetAttributeValuesHandler(Microsoft.Extensions.Logging.ILogger<GetAttributeValuesHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<IPagedList<string>>> HandleAsync(GetAttributeValues query, CancellationToken cancellationToken)
            {

                query.tf = query.tf.CheckNullDefault();
                var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();
                

                Expression<Func<AttributeValueEntity, bool>> expression = query.tf.SearchTerm == null ? x => true : x => x.Value.Contains(query.tf.SearchTerm);     


                var values = attributeValueRepo
                                .GetQueryale()
                                .Where(x => x.Key == query.Key)
                                .Where(expression)
                                .OrderBy(c => c.CreatedAt)
                                .Select(x => x.Value)
                                .ToPagedList(query.tf.PgNumber, query.tf.PgSize);

                return ServiceResult.Create<IPagedList<string>>(values);
            }

            protected override ServiceResult<IPagedList<string>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<IPagedList<string>>();
                
            }
        }
    }
}