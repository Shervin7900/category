using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Commands.Attribute;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Queries.AttributeQueries;
using CategoryApi.Queries.Seller;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using src.CategoryApi.Queries.AttributeQueries;

namespace CategoryApi.Controllers.V1
{


    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class AttributeController : MHBaseController
    {



        private readonly IMediator _mediator;

        public AttributeController(ILogger<AttributeController> logger,
        IUserHelper userHelper, IMediator mediator) : base(logger, userHelper)
        {
            _mediator = mediator;
        }






        /// <summary>
        ///  Register a new Attribute
        /// </summary>
        /// <remarks>
        /// Value, Name, IsVariantable, IsSearchable, IsRequired are Required
        ///
        // /// اگر نوع ورودی باشد، باید منحصر به فرد باشد و ارزش ها نباید خالی باشد و باید الزامی باشد
        ///
        // /// اگر نوع تکی باشد، باید تغییر پذیر باشد و ارزش ها نباید خالی باشد و حداقل یک ارزش باید داشته باشد و باید الزامی  باشد
        ///
        // /// اگر نوع چندتایی باشد، باید تغییر پذیر باشد و ارزش ها نباید خالی باشد و حداقل یک ارزش باید داشته باشد
        ///
        // /// دو تغییر پذیر ،دو منحصر به فرد ، دو تغییر پذیر و منحصر به فرد نداریم
        ///
        // /// نام های ویژگی ها نباید یکسان باشد
        /// </remarks>
        /// <response code="409"> if Attribute Already Exists  </response>  
        /// <response code="400"> if more than one Attribute set to variantable  </response>  
        /// <response code="404"> if Category Not Found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> A newly created AttributeId </returns>
        [ProducesResponseType(typeof(ServiceResult<string>), 200)]
        [HttpPost("Admin/Create")]
        [Authorize]
        public async Task<ServiceResult<string>> create(CreateAttributeDto modelDto)
        {
            return await _mediator.Send<ServiceResult<string>>(modelDto.Adapt<CreateAttributeCommand>());
        }




        /// <summary>
        ///  Assign a Registered Attribute to Category
        /// </summary>
        /// <remarks>
        /// Value, Name, IsVariantable, IsSearchable, IsRequired are Required
        ///
        // /// اگر نوع ورودی باشد، باید منحصر به فرد باشد و ارزش ها نباید خالی باشد و باید الزامی باشد
        ///
        // /// اگر نوع تکی باشد، باید تغییر پذیر باشد و ارزش ها نباید خالی باشد و حداقل یک ارزش باید داشته باشد و باید الزامی  باشد
        ///
        // /// اگر نوع چندتایی باشد، باید تغییر پذیر باشد و ارزش ها نباید خالی باشد و حداقل یک ارزش باید داشته باشد
        ///
        // /// دو تغییر پذیر ،دو منحصر به فرد ، دو تغییر پذیر و منحصر به فرد نداریم
        ///
        // /// نام های ویژگی ها نباید یکسان باشد
        /// </remarks>
        /// <response code="409"> if Attribute Already Exists  </response>  
        /// <response code="400"> if more than one Attribute set to variantable  </response>  
        /// <response code="404"> if Category Not Found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> A newly created AttributeId </returns>
        [HttpPut("Admin/Assign/{attributeKey}/{categoryId}")]
        [Authorize]
        public async Task<ServiceResult<string>> assign(string attributeKey , string categoryId)
        {
            return await _mediator.Send<ServiceResult<string>>(new AssignAttributeCommand(attributeKey , categoryId));
        }






        // /// <summary>
        // ///  Update a new Attribute By Admin
        // /// </summary>
        // /// <remarks>
        // ///  AttributeId, CategoryId, Value, Name, IsSearchable, IsVariantable, IsRequired are Required
        // /// 
        // /// اگر نوع ورودی باشد، باید منحصر به فرد باشد و ارزش ها نباید خالی باشد و باید الزامی باشد
        // /// 
        // /// اگر نوع تکی باشد، باید تغییر پذیر باشد و ارزش ها نباید خالی باشد و حداقل یک ارزش باید داشته باشد و باید الزامی  باشد
        // /// 
        // /// اگر نوع چندتایی باشد، باید تغییر پذیر باشد و ارزش ها نباید خالی باشد و حداقل یک ارزش باید داشته باشد 
        // /// 
        // /// دو تغییر پذیر ،دو منحصر به فرد ، دو تغییر پذیر و منحصر به فرد نداریم 
        // ///
        // /// نام های ویژگی ها نباید یکسان باشد
        // /// </remarks>
        // /// <response code="404"> if Attribute Not Found  </response>  
        // /// <response code="400"> if more than one Attribute set to variantable  </response>  
        // /// <response code="404"> if Category Not Found  </response>  
        // /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        // /// <returns> A newly created AttributeId </returns>
        // [ProducesResponseType(typeof(ServiceResult<string>), 200)]
        // [HttpPut("Admin/Update")]
        // [Authorize]
        // public async Task<ServiceResult<string>> UpdateAttribute(UpdateAttributeDto modelDto)
        // {
        //     return await _mediator.Send<ServiceResult<string>>(modelDto.Adapt<UpdateAttributeByAdminCommand>());
        // }








        /// <summary>
        ///  Get All Attribute of Category By Admin
        /// </summary>
        /// <remarks>
        ///  CategoryId is Required
        /// </remarks>
        /// <response code="404"> if Category Not Found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> Attributes of A Category </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetAttributesDto>>), 200)]
        [HttpGet("Admin/All/{categoryId}")]
        [Authorize]
        public async Task<ServiceResult<List<GetAttributesDto>>> GetAttributesByAdmin([FromRoute] string categoryId)
        {
            return await _mediator.Send<ServiceResult<List<GetAttributesDto>>>(new GetAttributesQuery(categoryId));
        }


        /// <summary>
        ///  Get Values of Attribute By admin
        /// </summary>
        /// <remarks>
        ///  Key is Required
        /// </remarks>
        /// <returns> Value of Attribute </returns>
        [ProducesResponseType(typeof(ServiceResult<IPagedList<string>>), 200)]
        [HttpGet("Admin/Values/{Key}")]
        [Authorize]
        public async Task<ServiceResult<IPagedList<string>>> GetAttributeValue([FromRoute] string key , [FromQuery] TermFilter termFilter)
        {
            return await _mediator.Send<ServiceResult<IPagedList<string>>>(new GetAttributeValues(key , termFilter));
        }


        /// <summary>
        ///  Get Values of Attribute By customer
        /// </summary>
        /// <remarks>
        ///  Key is Required
        /// </remarks>
        /// <returns> Value of Attribute </returns>
        [ProducesResponseType(typeof(ServiceResult<IPagedList<string>>), 200)]
        [HttpGet("Values/{Key}")]
        public async Task<ServiceResult<IPagedList<string>>> GetAttributeValueByCustomer([FromRoute] string key , [FromQuery] TermFilter termFilter)
        {
            return await _mediator.Send<ServiceResult<IPagedList<string>>>(new GetAttributeValues(key , termFilter));
        }


        /// <summary>
        ///  Get all Attributes By Admin
        /// </summary>
        /// <returns> all Attributes exist in System </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetAttributesDto>>), 200)]
        [HttpGet("Admin/GetAll")]
        [Authorize]
        public async Task<ServiceResult<List<GetAllAttributesDto>>> GetAttributesByAdmin()
        {
            return await _mediator.Send<ServiceResult<List<GetAllAttributesDto>>>(new GetAllAttributesByAdminQuery());
        }


        /// <summary>
        ///  Get All Attributes By Admin ( IsVariantable is False )
        /// </summary>
        /// <remarks>
        ///  CategoryId is Required
        /// </remarks>
        /// <param name ="categoryId"> Id of Category </param>
        /// <response code="404"> if Category Not Found  </response>  
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> Attributes of A Category </returns>
        [ProducesResponseType(typeof(ServiceResult<List<AttributeDetailsDto>>), 200)]
        [HttpGet("Admin/All/CreateProduct/{categoryId}")]
        [Authorize]
        public async Task<ServiceResult<List<AttributeDetailsDto>>> GetAllAttributesByAdmin([FromRoute] string categoryId)
        {
            return await _mediator.Send<ServiceResult<List<AttributeDetailsDto>>>(new GetAllCategoryAttributesByAdminQuery(categoryId));
        }







        /// <summary>
        ///  Get All ProductAttributes By Admin
        /// </summary>
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> All IsVariant False Attributes for ProductAttribute  </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetAllAttributesDto>>), 200)]
        [HttpGet("Admin/All/ProductAttribute/{categoryId}")]
        [Authorize]
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
        [Authorize]
        public async Task<ServiceResult<List<GetAllAttributesDto>>> GetAllVariantAttribute([FromRoute] string categoryId)
        {
            bool isVariantable = true;
            return await _mediator.Send<ServiceResult<List<GetAllAttributesDto>>>(new GetAllAttributesByIsVariantebaleQuery(isVariantable, categoryId));
        }



        /// <summary>
        ///  Get All VariantAttributes By Seller
        /// </summary>
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> All IsVariant True Attributes for ProductAttribute  </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetAllAttributesDto>>), 200)]
        [HttpGet("Seller/All/VariantAttribute/{categoryId}")]
        [Authorize]
        public async Task<ServiceResult<List<GetAllAttributesDto>>> GetAllVariantAttributeSeller([FromRoute] string categoryId)
        {
            bool isVariantable = true;
            return await _mediator.Send<ServiceResult<List<GetAllAttributesDto>>>(new GetAllAttributesByIsVariantableBySellerQuery(isVariantable, categoryId));
        }



        /// <summary>
        ///  Add Value To Attribute
        /// </summary>
        /// <response code="405"> if type of request is wrong or input data are wrong  </response>  
        /// <returns> All IsVariant True Attributes for ProductAttribute  </returns>
        [ProducesResponseType(typeof(ServiceResult<List<GetAllAttributesDto>>), 200)]
        [HttpPost("AddValue/{Key}")]
        [Authorize]
        public async Task<ServiceResult<string>> AddAttributeValue([FromRoute] string Key , [FromBody] AddValueToAttributeInputDto model)
        {
            return await _mediator.Send<ServiceResult<string>>(new AddValueToAttributeCommand(Key, model.Values));
        }



    }
}