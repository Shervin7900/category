using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Commands.Attribute
{
    public class CreateAttributeCommand  : CommandBase<ServiceResult<string>>
    {
    public List<string> Values{get; set;}

        public string Key{get; set;}

        public bool IsSearchable{get; set;}

        public bool IsVariantable{get; set;}

        public string Fa_Name{get; set;}
        
        public bool IsRequired{get; set;}

        public bool IsUnique{get; set;}

        public bool IsSearchableInValues{get; set;}
        public bool IsInCard{get; set;}

        public Domains.Entities.Type Type {get; set;}


        public CreateAttributeCommand(List<string> values, string key, bool isSearchable, bool isVariantable, string fa_Name, bool isRequired, bool isUnique, Domains.Entities.Type type, bool isInCard)
        {
            Values = values;
            Key = key;
            IsSearchable = isSearchable;
            IsVariantable = isVariantable;
            Fa_Name = fa_Name;
            IsRequired = isRequired;
            IsUnique = isUnique;
            Type = type;
            IsInCard = isInCard;
        }

        public CreateAttributeCommand()
        {
        }

        public class CreateAttributeCommandHandler : BaseRequestCommandHandler<CreateAttributeCommand, ServiceResult<string>>
        {

            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            public CreateAttributeCommandHandler(Microsoft.Extensions.Logging.ILogger<CreateAttributeCommandHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<string>> HandleAsync(CreateAttributeCommand command, CancellationToken cancellationToken)
            {



                   var sr = ServiceResult.Empty;
                   var repoAttribute = unitOfWork.GetRepository<AttributeEntity>();
                   var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();



                    if(command.Values == null)
                        throw sr.SetError(Errors.ValueCannotBeNull.GetMessage() , Errors.ValueCannotBeNull.GetCode())
                                                .SetMessage(Errors.ValueCannotBeNull.GetPersionMessage())
                                                .ToException();




                    if(command.Type != Domains.Entities.Type.Input)
                    {

                        if(command.Values.Distinct().Count() != command.Values.Count )
                            return sr.SetError(Errors.AttributeHasDuplicateValue.GetMessage() , Errors.AttributeHasDuplicateValue.GetCode())
                                                .SetMessage(Errors.AttributeHasDuplicateValue.GetPersionMessage())
                                                .To<string>();
                                                
                    }

                


                    // check key exist 
                    var isKeyExist = repoAttribute.GetQueryale().Any(x=>x.Key == command.Key);
                    if(isKeyExist)
                        return sr.SetError(Errors.DuplicateKeyExists.GetMessage() , Errors.DuplicateKeyExists.GetCode())
                            .SetMessage(Errors.DuplicateKeyExists.GetPersionMessage())
                            .To<string>();




                    //  Register and validate Attribute                
                    var newAttribute = command.Adapt<AttributeEntity>();
                    newAttribute.CheckVariantAttributeShouldBeSingleSelectType();
                    newAttribute.ValidateType();
                    newAttribute.CheckIsUniqShouldBeInputType();
                    newAttribute.ValidateRequirement();
                    newAttribute.CheckVariantAttributeIsUniq();
                    newAttribute.validateIsInCardAttribute();


                    // Insert Attribute
                    repoAttribute.Insert(newAttribute);

                    // Insert Attribute Value
                    var attributeValues = new List<AttributeValueEntity>();
                    command.Values.ForEach(x => 
                    {
                        attributeValues.Add(new AttributeValueEntity(newAttribute.Key , x));
                    });
                    attributeValueRepo.Insert(attributeValues);



                    unitOfWork.SaveChanges();

                    return sr.To<string>(newAttribute.Key);


            }
        }

    }
}