# Commands & CommandHandlers


## Command 

Commands are messages with the intent to change the state of the sysytem .
They are a way to execute behavior. 



command usually contains a set of properties as DTO which is quantified in constructor of it .



## CommandHandler 

commandHandler is space of implementation of method's logic . 



# Implementation


A folder containing two files (Command & CommandHandler) must be created for each command and its Handler.

## NamingPattern   


 **Verb** + **object** | **entity** + **command**



# Example
	
	
	Verb : Create
	Object : Test

    FolderName         :  CreateTestCommand
	CommandName        :  CreateTestCommand.cs	
	CommandHandlerName :  CreateTestCommandHandler.cs


#

	public class UpdateTestCommand : CommandBase<ServiceResult<string>>
    {

        public string Name { get; set; }
        public string Phone { get; set; }


        public UpdateTestCommand(string name, string phone)
        {
            Name = name;
            Phone = phone;
        }

        public UpdateTestCommand()
        {

        }

        public class UpdateTestCommandHandler : BaseRequestCommandHandler<UpdateTestCommand, ServiceResult<string>>
        {

            public UpdateTestCommandHandler(ILogger<UpdateTestCommand> logger) : base(logger)
            {

            }

            protected override async Task<ServiceResult<string>> HandleAsync(UpdateTestCommand command, CancellationToken cancellationToken)
            {


                // Do logic
                var sResult = new ServiceResult().To<string>("successfully done");
                return sResult;


            }

            protected override ServiceResult<string> HandleOnError(System.Exception exp)
            {
                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");
                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<string>();
            }
        }
    }