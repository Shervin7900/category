using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Commands.Attribute
{
    public class AddValueToAttributeCommand: CommandBase<ServiceResult<string>>
    {
        public List<string> Values{get; set;}

        public string Key{get; set;}


        public AddValueToAttributeCommand(string key, List<string> values)
        {
            Key = key;
            Values = values;
        }

        public AddValueToAttributeCommand()
        {
        }

        public class AddValueToAttributeCommandHandler : BaseRequestCommandHandler<AddValueToAttributeCommand, ServiceResult<string>>
        {

            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            public AddValueToAttributeCommandHandler(Microsoft.Extensions.Logging.ILogger<AddValueToAttributeCommandHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<string>> HandleAsync(AddValueToAttributeCommand command, CancellationToken cancellationToken)
            {



                   var sr = ServiceResult.Empty;
                   var repoAttribute = unitOfWork.GetRepository<AttributeEntity>();
                   var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();



                    if(command.Values.Count() == 0 && command.Values == null)
                        throw sr.SetError(Errors.ValueCannotBeNull.GetMessage() , Errors.ValueCannotBeNull.GetCode())
                                                .SetMessage(Errors.ValueCannotBeNull.GetPersionMessage())
                                                .ToException();


                    var attributeValue = attributeValueRepo.GetQueryale().FirstOrDefault(x=>x.Key == command.Key);
                    if(attributeValue == null)
                        throw sr.SetError(Errors.KeyNotFound.GetMessage() , Errors.KeyNotFound.GetCode())
                                                .SetMessage(Errors.KeyNotFound.GetPersionMessage())
                                                .ToException();


                    
                    var valueOfAttribute = attributeValueRepo.GetQueryale().Where(x=>x.Key == command.Key).ToList();

                    command.Values.Distinct().ToList().ForEach(x=>{
                        var duplicateValue = valueOfAttribute.FirstOrDefault(m=>m.Value == x);
                        if(duplicateValue != null)
                            throw sr.SetError(Errors.AttributeHasDuplicateValue.GetMessage() , Errors.AttributeHasDuplicateValue.GetCode())
                                                .SetMessage(Errors.AttributeHasDuplicateValue.GetPersionMessage())
                                                .ToException();
                        if(x != null && x != String.Empty)
                        {
                            var attributeValue = new AttributeValueEntity(command.Key , x);
                            attributeValueRepo.Insert(attributeValue);
                        }
                        else
                        {
                            throw sr.SetError(Errors.ValueCannotBeNull.GetMessage() , Errors.ValueCannotBeNull.GetCode())
                                                .SetMessage(Errors.ValueCannotBeNull.GetPersionMessage())
                                                .ToException();
                        }
                    });



                    unitOfWork.SaveChanges();

                    return sr.To<string>();


            }
        }
        
    }
}