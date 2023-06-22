using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Commands.Commission;
using CategoryApi.Dtos.Commission;
using CategoryApi.Queries.Commission;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;

namespace CategoryApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class CommissionController: MHBaseController
    {




        private readonly IMediator _mediator;

        public CommissionController(ILogger<CategoryController> logger,
        IUserHelper userHelper, IMediator mediator) : base(logger, userHelper)
        {
            _mediator = mediator;
        }





        /// <summary>
        ///  Update a Commission
        /// </summary>
        /// <remarks>
        ///  Id And CategoryId And TotalCommission And AffiliatorCommission is Required
        /// </remarks>
        /// <response code="404"> if commission is not found  </response>  
        /// <response code="404"> if category is not found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> commissionId </returns>
        [ProducesResponseType(typeof(ServiceResult<string>), 200)]    
        [HttpPut("Update")]
        [Authorize]
        public async Task<ServiceResult<string>> UpdateCommission([FromBody]UpdateCommissionInputDto modelDto)
        {
            return await _mediator.Send<ServiceResult<string>>(new UpdateCommissionCommand( modelDto.AffiliatorCommission  ,modelDto.AnotherShopCommission , modelDto.Id , modelDto.BooxellCommission));
        }



        /// <summary>
        ///  Get All Commission
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> All Commission </returns>
        [ProducesResponseType(typeof(ServiceResult<string>), 200)]    
        [HttpGet("All")]
        [Authorize]
        public async Task<ServiceResult<IPagedList<GetAllCommissionOutPutDto>>> GetAllCommission([FromQuery] SearchFilter sf)
        {
            return await _mediator.Send<ServiceResult<IPagedList<GetAllCommissionOutPutDto>>>(new GetAllCommissionQuery(sf));
        }



        /// <summary>
        ///  Get Commission Detail By Category Id
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> Commission Detail </returns>
        [ProducesResponseType(typeof(ServiceResult<GetCommissionsByCategoryIdDto>), 200)]    
        [HttpGet("Detail/{CategoryId}")]
        [Authorize]
        public async Task<ServiceResult<GetCommissionOfACategoryIdOutPutDto>> GetCommissionDetailByCategoryId([FromRoute] string CategoryId)
        {
            return await _mediator.Send<ServiceResult<GetCommissionOfACategoryIdOutPutDto>>(new GetCommissionOfACategoryIdQuery(CategoryId));
        }


        
    }
}