using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Dtos.Commission;
using CategoryApi.Queries.Commission;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;

namespace CategoryApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class InternalCommissionController : MHBaseController
    {




        private readonly IMediator _mediator;

        public InternalCommissionController(ILogger<InternalCommissionController> logger,
        IUserHelper userHelper, IMediator mediator) : base(logger, userHelper)
        {
            _mediator = mediator;
        }




        /// <summary>
        ///  Get All Commission
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> All Commission </returns>
        [ProducesResponseType(typeof(ServiceResult<GetAllCommissionOutPutDto>), 200)]
        [HttpGet("All")]
        public async Task<ServiceResult<IPagedList<GetAllCommissionOutPutDto>>> GetAllCommission([FromQuery] SearchFilter sf)
        {
            return await _mediator.Send<ServiceResult<IPagedList<GetAllCommissionOutPutDto>>>(new GetAllCommissionQuery(sf));
        }


        /// <summary>
        ///  Get Commissions By CategoryId
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> Commissions By CategoryId </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetCommissionsByCategoryIdDto>>), 200)]
        [HttpPost("GetCommissions")]
        public async Task<ServiceResult<List<GetCommissionsByCategoryIdDto>>> GetCommissionsByCategoryId([FromBody] List<String> CategoryIds)
        {
            return await _mediator.Send<ServiceResult<List<GetCommissionsByCategoryIdDto>>>(new GetCommissionsByCategoryIdQuery(CategoryIds));
        }



    }
}