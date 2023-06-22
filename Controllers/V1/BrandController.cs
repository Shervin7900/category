using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Commands.Brand;
using CategoryApi.Dtos.BrandDtos;
using CategoryApi.Queries.BrandQueries;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using src.CategoryApi.Dtos.BrandDtos;
using src.CategoryApi.Queries.CategoryQueries;

namespace CategoryApi.Controllers.V1
{



    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BrandController : MHBaseController
    {




        private readonly IMediator _mediator;

        public BrandController(ILogger<BrandController> logger,
        IUserHelper userHelper, IMediator mediator) : base(logger, userHelper)
        {
            _mediator = mediator;
        }





        /// <summary>
        ///  Register a new Brand
        /// </summary>
        /// <remarks>
        ///  CategoryId, Fa_Name, Status, Description  are Required
        /// </remarks>
        /// <response code="409"> if Brand Already Exists  </response>  
        /// <response code="404"> if Category Not Found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> A newly created BrandId </returns>
        [ProducesResponseType(typeof(ServiceResult<string>), 200)]
        [HttpPost("Admin/Create")]
        [Authorize]
        public async Task<ServiceResult<string>> CreateBrand(CreateBrandDto modelDto)
        {
            return await _mediator.Send<ServiceResult<string>>(modelDto.Adapt<CreateBrandCommand>());
        }





        /// <summary>
        ///  Update a Brand
        /// </summary>
        /// <remarks>
        ///  BrandId, CategoryId, Fa_Name, Status, Description  are Required
        /// </remarks>
        /// <response code="404"> if Brand Not Found  </response>  
        /// <response code="404"> if Category Not Found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> Updated BrandId </returns>
        [ProducesResponseType(typeof(ServiceResult<string>), 200)]
        [HttpPut("Admin/Update")]
        [Authorize]
        public async Task<ServiceResult<string>> UpdateBrand(UpdateBrandDto modelDto)
        {
            return await _mediator.Send<ServiceResult<string>>(modelDto.Adapt<UpdateBrandCommand>());
        }





        /// <summary>
        ///  Get All Brands In A Category
        /// </summary>
        /// <remarks>
        ///  CategoryId, Pagenumber, PageSize are Required
        /// </remarks>
        /// <param name ="categoryId"> Id of Category </param>
        /// <response code="404"> if Category Not Found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> Updated BrandId </returns>
        [ProducesResponseType(typeof(ServiceResult<IPagedList<GetBrandOutPutDto>>), 200)]
        [HttpGet("Admin/{categoryId}")]
        [Authorize]
        public async Task<ServiceResult<IPagedList<GetBrandOutPutDto>>> GetBrands([FromQuery] TermFilter tf, string categoryId)
        {
            return await _mediator.Send<ServiceResult<IPagedList<GetBrandOutPutDto>>>(new GetAllBrandsByCategoryIdQuery(tf, categoryId));
        }







        /// <summary>
        ///  Get Brand Info By Admin for Create Product
        /// </summary>
        /// <remarks>
        ///  CategoryId is Required
        /// </remarks>
        /// <param name ="categoryId"> Id of Category </param>
        /// <response code="404"> if category is not found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> Brands Of That Category </returns>
        [ProducesResponseType(typeof(ServiceResult<List<BrandDetailDto>>), 200)]
        [HttpGet("Admin/All/CreateProduct/{categoryId}")]
        [Authorize]
        public async Task<ServiceResult<List<BrandDetailDto>>> GetAllBrandsByAdmin([FromRoute] string categoryId)
        {
            return await _mediator.Send<ServiceResult<List<BrandDetailDto>>>(new GetAllBrandsByAdminQuery(categoryId));
        }





    }
}