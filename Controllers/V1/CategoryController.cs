using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Commands.Category;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Queries.CategoryQueries;

namespace CategoryApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CategoryController : MHBaseController
    {




        private readonly IMediator _mediator;

        public CategoryController(ILogger<CategoryController> logger,
        IUserHelper userHelper, IMediator mediator) : base(logger, userHelper)
        {
            _mediator = mediator;
        }





        /// <summary>
        ///  Register a new Category
        /// </summary>
        /// <remarks>
        ///  Title and Description is Required
        ///  اگر پرنت آیدی نال باشد به روت اضافه میشود
        /// </remarks>
        /// <response code="404"> if Parent is not found </response>  
        /// <response code="400"> if hierarchid is invalid </response>  
        /// <response code="404"> if category is not found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> categoryId </returns>
        [ProducesResponseType(typeof(ServiceResult<string>), 200)]    
        [HttpPost("Admin/Create")]
        [Authorize]
        public async Task<ServiceResult<string>> CreateCategory(CreateCategoryDto modelDto)
        {
            return await _mediator.Send<ServiceResult<string>>(modelDto.Adapt<CreateCategoryCommand>());
        }





        /// <summary>
        ///  Update a Category
        /// </summary>
        /// <remarks>
        ///  CategoryId and Title and Description is Required
        /// </remarks>
        /// <response code="404"> if category is not found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> Id Of The Updated Category </returns>
        [ProducesResponseType(typeof(ServiceResult<string>), 200)]    
        [HttpPut("Admin/Update")]
        [Authorize]
        public async Task<ServiceResult<string>> UpdateCategory(UpdateCategoryDto modelDto)
        {
            return await _mediator.Send<ServiceResult<string>>(modelDto.Adapt<UpdateCategoryCommand>());
        }





        /// <summary>
        ///  Get Category Info By Admin
        /// </summary>
        /// <response code="404"> if category is not found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> CategoryInfo Of That Category </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetCategoryInfoByAdminDto>>), 200)]    
        [HttpGet("Admin/tree")]
        [Authorize]
        public async Task<ServiceResult<GetCategoryInfoByAdminDto>> GetCategoryInfoByAdmin([FromQuery]string parentId)
        {
            return await _mediator.Send<ServiceResult<GetCategoryInfoByAdminDto>>(new GetCategoryInfoByAdminQuery(parentId));
        }  





        /// <summary>
        ///  Get Category Info By User
        /// </summary>
        /// <response code="404"> if category is not found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> CategoryInfo Of That Category </returns>
        [ProducesResponseType(typeof(ServiceResult<GetCategoryInfoByUserDto>), 200)]    
        [HttpGet("Menu")]
        public async Task<ServiceResult<GetCategoryInfoByUserDto>> GetCategoryInfoByUser()
        {
            return await _mediator.Send<ServiceResult<GetCategoryInfoByUserDto>>(new GetMenuQuery());
        }  



        
        
        // /// <summary>
        // ///  Get Category Filters
        // /// </summary>
        // /// <returns> Filters model for specific category </returns>
        [ProducesResponseType(typeof(ServiceResult<GetCustomerFiltersDto>), 200)]    
        [HttpGet("Filters/{CategoryId}")]
        public async Task<ServiceResult<List<GetCustomerFiltersDto>>> GetCustomerFilters(string CategoryId)
        {
            return await _mediator.Send<ServiceResult<List<GetCustomerFiltersDto>>>(new GetCategoryFiltersQuery(CategoryId));
        }  



        /// <summary>
        ///  Get BreadCrumb Category
        /// </summary>
        /// <remarks>
        ///  CategoryId is Required
        /// </remarks>
        /// <returns> BreadCrumb </returns>
        [ProducesResponseType(typeof(ServiceResult<GetBreadCrumbOutPutDto>), 200)]
        [HttpGet("BreadCrumb/{CategoryId}")]
        public async Task<ServiceResult<GetBreadCrumbOutPutDto>> BreadCrumb([FromRoute] string CategoryId)
        {
            return await _mediator.Send<ServiceResult<GetBreadCrumbOutPutDto>>(new GetBreadCrumbQuery(CategoryId));
        }


    }
}