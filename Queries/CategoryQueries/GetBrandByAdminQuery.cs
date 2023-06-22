using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;
using src.CategoryApi.Dtos.BrandDtos;

namespace src.CategoryApi.Queries.CategoryQueries
{
    public class GetAllBrandsByAdminQuery : QueryBase<ServiceResult<List<BrandDetailDto>>>
    {
        public string CategoryId{get; set;}

        public GetAllBrandsByAdminQuery(string categoryId)
        {
            CategoryId = categoryId;
        }

        public GetAllBrandsByAdminQuery()
        {
        }

        public class GetAllBrandsByAdminQueryHandler : BaseRequestQueryHandler<GetAllBrandsByAdminQuery, ServiceResult<List<BrandDetailDto>>>
        {


            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;


            public GetAllBrandsByAdminQueryHandler(ILogger<GetAllBrandsByAdminQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }


            protected override async Task<ServiceResult<List<BrandDetailDto>>> HandleAsync(GetAllBrandsByAdminQuery query, CancellationToken cancellationToken)
            {


                var sr = ServiceResult.Empty;
                var brandRepository = unitOfWork.GetRepository<BrandEntity>();
                var categoryRepository = unitOfWork.GetRepository<CategoryEntity>();




                var brands = brandRepository.GetQueryale()
                                            .Where(x=>x.CategoryId == query.CategoryId)
                                            .AsNoTracking()
                                            .ToList();




                var categoryEntity = categoryRepository.GetQueryale()
                                        .Any(x=>x.Id == query.CategoryId);


                if(categoryEntity == null)
                    return sr.SetError(Errors.CategoryNotFound.GetMessage(), Errors.CategoryNotFound.GetCode())
                    .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                    .To<List<BrandDetailDto>>();



                


                return ServiceResult.Create<List<BrandDetailDto>>(brands.Adapt<List<BrandDetailDto>>());

            }

            protected override ServiceResult<List<BrandDetailDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<List<BrandDetailDto>>();
                
            }
        }
    }
}