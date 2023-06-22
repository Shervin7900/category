namespace CategoryApi.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : MHBaseController
    {




            public HomeController(ILogger<HomeController> logger, IUserHelper userHelper) : base(logger, userHelper){}






            /// <summary>
            ///  HealthCheck Api  
            /// </summary>
            [HttpGet]
            [AllowAnonymous]
            public async Task<ActionResult> Ping()
                => await Task.FromResult(Ok());






            /// <summary>
            ///  Get Errors List 
            /// </summary>
            /// <remarks>
            ///  get list of used ErrorMessage whit h their ErrorCode in this Microservice.
            /// </remarks>
            /// <returns> return list of used ErrorMessage whith their ErrorCode in this Microservice </returns>
            [ProducesResponseType(typeof(List<ErrorDto>), 200)]   
            [HttpGet("ErrorsList")]
            public List<ErrorDto> GetErrors() 
                =>  ErrorMessagesExtentions.GetErrors();



    }


}