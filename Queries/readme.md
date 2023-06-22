# Query & QueryHandler


## Command 

Queries never modify the database. A query returns a DTO that does not encapsulate any domain knowledge.


## QueryHandler 

QueryHandler is space of implementation of method's logic . 



# Implementation


A folder containing two files (Query & QueryHandler) must be created for each query and its Handler.

## NamingPattern   


 **Verb** + **object** | **entity** + **Query**



# Example
	
	
	Verb : Create
	Object : Test

    FolderName          :  CreateTestQueries
	CommandName         :  CreateTestQuery.cs	
	CommandHandlerName  :  CreateTestQueryHandler.cs



#


	    public class GetAllTestQuery : QueryBase<ServiceResult<TestDto>>
		{
			public GetAllTestQuery()
			{
				
			}


			public class GetAllTestQueryHandler : BaseRequestQueryHandler<GetAllTestQuery, ServiceResult<TestDto>>
			{
				public GetAllTestQueryHandler(ILogger<GetAllTestQueryHandler> logger) : base(logger)
				{

				}

				protected override async Task<ServiceResult<TestDto>> HandleAsync(GetAllTestQuery query, CancellationToken cancellationToken)
				{
				
					// Do logic
					var sResult = new ServiceResult().To<TestDto>(new TestDto(){ Name = "test" , Phone = "09336664455" });
					return sResult ;

				}

				protected override ServiceResult<TestDto> HandleOnError(System.Exception exp)
				{
					var sr = ServiceResult.Empty;
					sr = sr.SetError("UnHandled");
					sr.Error.ExteraFields = new KeyValueList<string, string>(){
						new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
						new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
					};

					return sr.To<TestDto>();
				}
			}

			
		}