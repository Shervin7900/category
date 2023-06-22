using CategoryApi.Commands.Attribute;
using CategoryApi.Commands.Category;
using CategoryApi.Dtos.AggregatedDtos;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Dtos.Sync;
using CategoryApi.Queries.CategoryQueries;
using MH.DDD.DataAccessLayer.PagedList;
using CategoryApi.Queries.SyncShop;
using src.CategoryApi.Dtos.BrandDtos;
using CategoryApi.Queries.AttributeQueries;

namespace src.CategoryApi.Controllers.V1
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class InternalCategoryController : MHBaseController
    {


        private readonly IMediator _mediator;



        public InternalCategoryController(ILogger<InternalCategoryController> logger,
        IUserHelper userHelper, IMediator mediator) : base(logger, userHelper)
        {
            _mediator = mediator;
        }








        /// <summary>
        /// Validate Category Dependencies (Attributes , Brand) When IsVariantable is false
        /// </summary>
        /// <remarks>
        ///  CategoryId ,BrandId , Attributes are Required
        ///
        /// ویژگی ها نباید تغییر پذیر باشند
        ///
        ///اگر ویژگی الزامی باشد،نباید مقدار ها خالی باشد
        ///
        /// اگر منحصر به فرد باشد، باید از نوع ورودی و دارای مقدار باشد
        ///
        /// اگر نوع ورودی و تکی و چندتایی باشد، باید مقدار داشته باشد
        /// </remarks>
        /// <response code="404"> if category is not found  </response>
        /// <response code="404"> if brand is not found  </response>    
        /// <response code="409"> if attribute type is input and Values is not null  </response>  
        /// <response code="409"> if attribute type is Single and Values is null or count is 0 </response>  
        /// <response code="409"> if attribute type is MultiSelect and Values is null or count is 0 </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> true or false </returns>
        [ProducesResponseType(typeof(ServiceResult<AttributeValidationResponseModelDto>), 200)]
        [HttpPost("Admin/Create/ProductAtt/Validate")]
        public async Task<ServiceResult<AttributeValidationResponseModelDto>> AttributeValidationWhenIsVariantableisFalse(AttributeValidationDto modelDto)
        {
            return await _mediator.Send<ServiceResult<AttributeValidationResponseModelDto>>(modelDto.Adapt<AttributeValidationWhenIsVariantableisFalseCommand>());
        }






        /// <summary>
        /// Validate Category Dependencies (Attributes , Brand) When IsVariantable is true
        /// </summary>
        /// <remarks>
        ///  CategoryId ,BrandId , Attributes are Required
        ///
        /// ویژگی ها باید تغییر پذیر باشند
        ///
        ///اگر ویژگی الزامی باشد،نباید مقدار ها خالی باشد
        ///
        /// اگر منحصر به فرد باشد، باید از نوع ورودی و دارای مقدار باشد
        ///
        /// اگر نوع ورودی و تکی و چندتایی باشد، باید مقدار داشته باشد
        /// </remarks>
        /// <response code="404"> if category is not found  </response>
        /// <response code="404"> if brand is not found  </response>    
        /// <response code="409"> if attribute type is input and Values is not null  </response>  
        /// <response code="409"> if attribute type is Single and Values is null or count is 0 </response>  
        /// <response code="409"> if attribute type is MultiSelect and Values is null or count is 0 </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> true or false </returns>
        [ProducesResponseType(typeof(ServiceResult<bool>), 200)]
        [HttpPost("Admin/Create/ProductVariant/Validate")]
        public async Task<ServiceResult<bool>> AttributeValidationWhenIsVariantableisTrue(AttributeValidationDto modelDto)
        {
            return await _mediator.Send<ServiceResult<bool>>(modelDto.Adapt<AttributeValidationWhenIsVariantableisTrueCommand>());
        }















        /// <summary>
        /// Validate CategoryIds And BrandIds
        /// </summary>
        /// <remarks>
        ///  CategoryIds ,BrandIds are Required
        /// </remarks>
        /// <response code="404"> if category  is not found  </response>
        /// <response code="404"> if brand is not found  </response>    
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> true or false </returns>
        [ProducesResponseType(typeof(ServiceResult<bool>), 200)]
        [HttpPost("BasketAgg/ValidateCategoriesAndBrands")]
        public async Task<ServiceResult<bool>> ValidateListOfCategoriesAndBrands(ValidateCategoriesAndBrandsDto modelDto)
        {
            return await _mediator.Send<ServiceResult<bool>>(new ValidateListOfCategoriesAndBrandsCommand(modelDto.CategoryIds, modelDto.BrandIds));
        }












        /// <summary>
        ///  Get List Key And Attribute Fa_Name
        /// </summary>
        /// <remarks>
        ///  List Of Keys are Required
        /// </remarks>
        /// <returns> list of Key And Fa_Name attribute </returns>
        [ProducesResponseType(typeof(ServiceResult<CategoryAttributeDetailDto>), 200)]
        [HttpPost("Get/BasketAggregator/Attribute")]
        public async Task<ServiceResult<List<CategoryAttributeDetailDto>>> CategoryDetail([FromBody] List<string> Keys)
        {
            return await _mediator.Send<ServiceResult<List<CategoryAttributeDetailDto>>>(new GetCategoryDetailsQuery(Keys));
        }




        /// <summary>
        ///  Get CategoriesInfo , BrandsInfo , AttributesInfo
        ///
        ///  This Model Used For Getting Multi Brands info , Category info , Attribute info
        ///
        /// </summary>
        /// <returns> list of Brands , Categories , Attributes </returns>
        [ProducesResponseType(typeof(ServiceResult<CategoryAttributeBrandDetailDto>), 200)]
        [HttpPost("Get/Categories/Brands/Attributes/Details")]
        public async Task<ServiceResult<CategoryAttributeBrandDetailDto>> CategoryAttributeBrandDetails([FromBody] CategoryAttributeBrandDetailInputDto dto)
        {
            return await _mediator.Send<ServiceResult<CategoryAttributeBrandDetailDto>>(new CategoryAttributeBrandDetailsQuery(dto));
        }





        /// <summary>
        ///  Get CategoriesInfo , BrandsInfo , AttributesInfo
        ///
        ///  This Model Used For Getting Multi Brands info , Category info , Attribute info
        ///
        /// </summary>
        /// <returns> list of Brands , Categories , Attributes </returns>
        [ProducesResponseType(typeof(ServiceResult<SyncCategoryAttributeBrandDetailDto>), 200)]
        [HttpPost("Get/Categories/Attributes/Details/Sync")]
        public async Task<ServiceResult<SyncCategoryAttributeBrandDetailDto>> SyncCategoryAttributeDetails([FromBody] SyncCategoryAttributeBrandDetailInputDto dto)
        {
            var result = await _mediator.Send<ServiceResult<SyncCategoryAttributeBrandDetailDto>>(new SyncCategoryAttributeBrandDetailsQuery(dto));
            return result;
        }





        /// <summary>
        ///  Get time and termFilter
        ///
        ///  This Model Used For sync Category Title
        ///
        /// </summary>
        /// <returns> list of  Categories Title </returns>
        [ProducesResponseType(typeof(ServiceResult<IPagedList<GetCategoryDetailSyncDto>>), 200)]
        [HttpPost("Get/Categories/Title/Sync")]
        public async Task<ServiceResult<IPagedList<GetCategoryDetailSyncDto>>> SyncCategoryTitle([FromBody] InputSyncDto input)
        {
            var result = await _mediator.Send<ServiceResult<IPagedList<GetCategoryDetailSyncDto>>>(new SyncCategoryTitleQuery(input.Tr, input.Time));
            return result;
        }



        /// <summary>
        ///  Get time and termFilter
        ///
        ///  This Model Used For sync brand Title
        ///
        /// </summary>
        /// <returns> list of  brand Title </returns>
        [ProducesResponseType(typeof(ServiceResult<IPagedList<BrandDetailSyncDto>>), 200)]
        [HttpPost("Get/Brand/Title/Sync")]
        public async Task<ServiceResult<IPagedList<BrandDetailSyncDto>>> SyncBrandTitle([FromBody] InputSyncDto input)
        {
            var result = await _mediator.Send<ServiceResult<IPagedList<BrandDetailSyncDto>>>(new GetSyncBrandDetailQuery(input.Tr, input.Time));
            return result;
        }



        /// <summary>
        ///  Get BreadCrumb Category
        /// </summary>
        /// <remarks>
        ///  CategoryId is Required
        /// </remarks>
        /// <returns> BreadCrumb </returns>
        [ProducesResponseType(typeof(ServiceResult<CategoryDetailDto>), 200)]
        [HttpGet("BreadCrumb/{CategoryId}")]
        public async Task<ServiceResult<GetBreadCrumbOutPutDto>> BreadCrumb([FromRoute] string CategoryId)
        {
            return await _mediator.Send<ServiceResult<GetBreadCrumbOutPutDto>>(new GetBreadCrumbQuery(CategoryId));
        }


        /// <summary>
        ///  Get IsInCard attributes By ListCategoryId
        /// </summary>
        /// <returns> list of  Categories Title </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetCardAttributesDtos>>), 200)]
        [HttpPost("Get/CardAttributes")]
        public async Task<ServiceResult<List<GetCardAttributesDtos>>> GetCardAttributesByCategoryIds([FromBody] List<string> categoryIds)
        {
            var result = await _mediator.Send<ServiceResult<List<GetCardAttributesDtos>>>(new GetCardAttributesByCategoryIdsQuery(categoryIds));
            return result;
        }



        /// <summary>
        ///  Get Categories With Parent Id
        /// </summary>
        /// <returns> list of  Category Ids </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetCardAttributesDtos>>), 200)]
        [HttpPost("Get/Search/CategoryIds")]
        public async Task<ServiceResult<List<string>>> GetCategoryIdsWithParentId([FromBody] List<string> ParentIds)
        {
            var result = await _mediator.Send<ServiceResult<List<string>>>(new GetCategoriesWithParentIdQuery(ParentIds));
            return result;
        }


    }
}