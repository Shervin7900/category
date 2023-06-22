using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Commands.Attribute;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Dtos.Sync;
using CategoryApi.Queries.AttributeQueries;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using CategoryApi.Queries.SyncShop;
using src.CategoryApi.Queries.AttributeQueries;

namespace CategoryApi.Controllers.V1
{


    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class InternalAttributeController : MHBaseController
    {



        private readonly IMediator _mediator;

        public InternalAttributeController(ILogger<InternalAttributeController> logger,
        IUserHelper userHelper, IMediator mediator) : base(logger, userHelper)
        {
            _mediator = mediator;
        }



        /// <summary>
        ///  Get All ProductAttributes By Admin
        /// </summary>
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> All IsVariant False Attributes for ProductAttribute  </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetAllAttributesDto>>), 200)]
        [HttpGet("Admin/All/ProductAttribute/{categoryId}")]
        public async Task<ServiceResult<List<GetAllAttributesDto>>> GetAllProductAttribute([FromRoute] string categoryId)
        {
            bool isVariantable = false;
            return await _mediator.Send<ServiceResult<List<GetAllAttributesDto>>>(new GetAllAttributesByIsVariantebaleQuery(isVariantable, categoryId));
        }





        /// <summary>
        ///  Get All VariantAttributes By Admin
        /// </summary>
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> All IsVariant True Attributes for ProductAttribute  </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetAllAttributesDto>>), 200)]
        [HttpGet("Admin/All/VariantAttribute/{categoryId}")]
        public async Task<ServiceResult<List<GetAllAttributesDto>>> GetAllVariantAttribute([FromRoute] string categoryId)
        {
            bool isVariantable = true;
            return await _mediator.Send<ServiceResult<List<GetAllAttributesDto>>>(new GetAllAttributesByIsVariantebaleQuery(isVariantable, categoryId));
        }


        /// <summary>
        ///  Get All Attributes By Key
        /// </summary>
        /// <returns> Attribute Details </returns>
        [ProducesResponseType(typeof(ServiceResult<IPagedList<GetAllAttributesByKeyDto>>), 200)]
        [HttpPost("Sync/GetAll")]
        public async Task<ServiceResult<IPagedList<GetAllAttributesByKeyDto>>> GetAllAttributesByKey([FromQuery] TermFilter tf, List<string> Keys)
        {
            return await _mediator.Send<ServiceResult<IPagedList<GetAllAttributesByKeyDto>>>(new GetAllAttributesByKeyQuery(tf, Keys));
        }



        /// <summary>
        ///  Get  Attributes By modifiedAt
        /// </summary>
        /// <returns> Attribute Details </returns>
        [ProducesResponseType(typeof(ServiceResult<IPagedList<GetAttributeDetailWithoutValuesSyncDto>>), 200)]
        [HttpPost("Get/Attribute/Title/Sync")]
        public async Task<ServiceResult<IPagedList<GetAttributeDetailWithoutValuesSyncDto>>> SyncAttributeDetails([FromBody] InputSyncDto input)
        {
            return await _mediator.Send<ServiceResult<IPagedList<GetAttributeDetailWithoutValuesSyncDto>>>(new GetSyncAttributeDetailQuery(input.Tr, input.Time));
        }
    }
}