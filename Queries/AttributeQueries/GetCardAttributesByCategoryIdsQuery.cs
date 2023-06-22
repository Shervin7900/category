using System.IO;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.AttributeQueries
{
    public class GetCardAttributesByCategoryIdsQuery : QueryBase<ServiceResult<List<GetCardAttributesDtos>>>
    {
        public GetCardAttributesByCategoryIdsQuery(List<string> categoryIds)
        {
            this.categoryIds = categoryIds;
        }

        public List<string> categoryIds { get; set; }


        public class GetCardAttributesByCategoryIdsQueryHandler : BaseRequestQueryHandler<GetCardAttributesByCategoryIdsQuery, ServiceResult<List<GetCardAttributesDtos>>>
        {

            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;

            public GetCardAttributesByCategoryIdsQueryHandler(ILogger<GetCardAttributesByCategoryIdsQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }

            protected override async Task<ServiceResult<List<GetCardAttributesDtos>>> HandleAsync(GetCardAttributesByCategoryIdsQuery query, CancellationToken cancellationToken)
            {


                // TODO: Warning : 
                // Should Refactor Model and Request to DB

                
                var sr = ServiceResult.Empty;

                var attributeRepo = unitOfWork.GetRepository<AttributeEntity>();
                var categoryAttributeRepo = unitOfWork.GetRepository<CategoryAttributeEntity>();

                var categoryAttributes = categoryAttributeRepo.GetQueryale()
                                                        .Where(x => query.categoryIds.Contains(x.CategoryId))
                                                        .Select(x => new {x.CategoryId,x.AttributeId})
                                                        .ToList();
                var w = attributeRepo.GetQueryale()
                                              .Where(x => x.IsVisibled && !x.IsDeleted && x.IsInCard && !x.IsVariantable)
                                              .Select(x => new{x.Id,x.Key,x.Fa_Name})
                                              .ToList();

                var attributes = attributeRepo.GetQueryale()
                                              .Where(x => categoryAttributes.Select(y => y.AttributeId).Contains(x.Id) && x.IsVisibled && !x.IsDeleted && x.IsInCard && !x.IsVariantable)
                                              .Select(x => new{x.Id,x.Key,x.Fa_Name})
                                              .ToList();



                // if(categoryAttributes.Distinct().Count() != attributes.Distinct().Count())
                //     return sr.SetError(Errors.AttributeNotFound.GetMessage() , Errors.AttributeNotFound.GetCode())
                //         .SetMessage(Errors.AttributeNotFound.GetPersionMessage())
                //         .To<List<GetCardAttributesDtos>>();

                var result = new List<GetCardAttributesDtos>();
                query.categoryIds.ForEach
                (
                    categoryId =>
                    {
                        var attIds = categoryAttributes.Where(x => x.CategoryId == categoryId).Select(x => x.AttributeId).ToList();
                        if(attIds.Count() != 0 )
                        {
                            var att = attributes.FirstOrDefault(x => attIds.Contains(x.Id));
                            if(att != null)
                                result.Add(new GetCardAttributesDtos(categoryId,att.Key,att.Fa_Name));
                        }

                    }
                );
                
                return sr.To<List<GetCardAttributesDtos>>(result);

            }

            protected override ServiceResult<List<GetCardAttributesDtos>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<List<GetCardAttributesDtos>>();

            }
        }
    }
}