# Controllers

all the controllers for the app go here 

# Implementation


First of all must create folder which show version Name and then create controller class
    eg : V1 or V2  

Eeach Controller must extend from  **MHBaseController**

Eeach Controller must has versioning tag in its folder 

Eeach Controller must inject **Logger** and pass it to **MHBaseController**

Eeach Service in controller should contains swagger Documentation

Eeach Service in controller should contains response model

Eeach Service in controller should contains authorize tag and it be in 2 state


1. just empty Authorize tag
2. Authorize tag with Policy


Swagger Documentation should follow below rules 

1.  summary 
2.  remark if is require
3.  return description
4.  error response code


## NamingPattern

	CategoryApiController.cs



## Example


	[ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TestController : MHBaseController
    {



        // inject dependencies 
        private readonly IMediator _mediator;
        
        public TestController(ILogger<TestController> logger, IUserHelper userHelper, IMediator mediator) : base(logger, userHelper)
        {
            _mediator = mediator;
        }






        /// <summary>
        ///  update a new test.
        /// </summary>
        /// <remarks>
        ///  remarks
        /// </remarks>
        /// <response code="404"> if entity is not found  </response>    
        /// <response code="400">If an error happen</response>
        /// <returns> return description </returns>
        [ProducesResponseType(typeof(ServiceResult<string>), 200)]    
        [HttpPost("testCommand")]
        public async Task<ServiceResult<string>> TestCommand(TestDto testDto)
        {
                
             var result = await _mediator.Send<ServiceResult<string>>(testDto.Adapt<UpdateTestCommand>());
             return result;

        }





        /// <summary>
        ///  update a new test.
        /// </summary>
        /// <remarks>
        ///  remarks
        /// </remarks>
        /// <response code="404"> if entity is not found  </response>    
        /// <response code="400">If an error happen</response>
        /// <returns> return description </returns>
        [ProducesResponseType(typeof(ServiceResult<string>), 200)]    
        [HttpPost("TestQuery")]
        public async Task<ServiceResult<TestDto>> TestQuery()
        {

             var result = await _mediator.Send<ServiceResult<TestDto>>(new GetAllTestQuery());
             return result;

        }








    }