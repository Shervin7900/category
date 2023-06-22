using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AggregatedDtos;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;
using src.CategoryApi.Dtos.BrandDtos;

namespace CategoryApi.Queries.CategoryQueries
{
    public class CategoryAttributeBrandDetailsQuery : QueryBase<ServiceResult<CategoryAttributeBrandDetailDto>>
    {
        public CategoryAttributeBrandDetailInputDto dto { get; set; }

        public CategoryAttributeBrandDetailsQuery(CategoryAttributeBrandDetailInputDto dto)
        {
            this.dto = dto;
        }

        public class CategoryAttributeBrandDetailsQueryHandler : BaseRequestQueryHandler<CategoryAttributeBrandDetailsQuery, ServiceResult<CategoryAttributeBrandDetailDto>>
        {
            private readonly IUnitOfWork<CategoryDbContext> _unitOfWork;

            public CategoryAttributeBrandDetailsQueryHandler(Microsoft.Extensions.Logging.ILogger<CategoryAttributeBrandDetailsQueryHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this._unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<CategoryAttributeBrandDetailDto>> HandleAsync(CategoryAttributeBrandDetailsQuery query, CancellationToken cancellationToken)
            {

                var sr = ServiceResult.Empty;

                var brandRepository = _unitOfWork.GetRepository<BrandEntity>();
                var categoryRepository = _unitOfWork.GetRepository<CategoryEntity>();
                var attributeRepository = _unitOfWork.GetRepository<AttributeEntity>();



                // Get Brands
                var brandDtos = brandRepository.GetQueryale()
                                        .Where(x => query.dto.BrandsId.Distinct().Contains(x.Id))
                                        .Select(x => x.Adapt<BrandDetailDto>())
                                        .ToList();


                // Get Categories
                var categoryDtos = categoryRepository.GetQueryale()
                                        .Where(x => query.dto.CategoriesId.Distinct().Contains(x.Id))
                                        .Select(x => x.Adapt<GetCategoryDetailDto>())
                                        .ToList();


                // Get Attributes
                var attributeDtos = attributeRepository.GetQueryale()
                                        .Where(x => query.dto.Keys.Distinct().Contains(x.Key))
                                        .Select(x => x.Adapt<GetAttributeDetailWithoutValues>())
                                        .ToList();


                return sr.To<CategoryAttributeBrandDetailDto>(new CategoryAttributeBrandDetailDto(brandDtos, categoryDtos, attributeDtos));


            }

            protected override ServiceResult<CategoryAttributeBrandDetailDto> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<CategoryAttributeBrandDetailDto>();

            }
        }
    }
}