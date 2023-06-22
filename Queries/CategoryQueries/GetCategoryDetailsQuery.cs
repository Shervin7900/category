using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.CategoryQueries
{
    public class GetCategoryDetailsQuery : QueryBase<ServiceResult<List<CategoryAttributeDetailDto>>>
    {


        public List<string> Keys { get; set; }

        public GetCategoryDetailsQuery(List<string> keys)
        {
            Keys = keys;
        }

        public GetCategoryDetailsQuery()
        {
        }



        public class GetBrandFaNameQueryHandler : BaseRequestQueryHandler<GetCategoryDetailsQuery, ServiceResult<List<CategoryAttributeDetailDto>>>
        {


            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;


            public GetBrandFaNameQueryHandler(ILogger<GetBrandFaNameQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }


            protected override async Task<ServiceResult<List<CategoryAttributeDetailDto>>> HandleAsync(GetCategoryDetailsQuery query, CancellationToken cancellationToken)
            {


                var sr = ServiceResult.Empty;
                var repoCategory = unitOfWork.GetRepository<CategoryEntity>();
                var repoAttribute = unitOfWork.GetRepository<AttributeEntity>();


                var attributeEntity = repoAttribute
                                    .GetQueryale()
                                    .Where(x=>query.Keys.Contains(x.Key))
                                    .Select(x=>new CategoryAttributeDetailDto(x.Key , x.Fa_Name,x.IsInCard))
                                    .ToList();


                return ServiceResult.Create<List<CategoryAttributeDetailDto>>(attributeEntity);



            }

            protected override ServiceResult<List<CategoryAttributeDetailDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<List<CategoryAttributeDetailDto>>();
                
            }
        }
        
    }
}