// using System.ComponentModel.Design;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using MH.DDD.DataAccessLayer.UnitOfWork;
// using CategoryApi.Domains.Entities;
// using CategoryApi.Infrastructures.DbContexts;

// namespace CategoryApi.Commands.Attribute
// {
//     public class UpdateAttributeByAdminCommand : CommandBase<ServiceResult<string>>
//     {

//         public List<string> Values{get; set;}

//         public string Key{get; set;}

//         public string CategoryId{get; set;}

//         public bool IsSearchable{get; set;}

//         public string Fa_Name{get; set;}

//         public Domains.Entities.Type Type {get; set;}

//         public bool IsVariantable{get; set;}
        
//         public bool IsRequired{get; set;}

//         public bool IsUnique{get; set;}

//         public UpdateAttributeByAdminCommand( List<string> values, string key, string categoryId, bool isSearchable, string fa_Name, Domains.Entities.Type type, bool isVariantable, bool isRequired, bool isUnique)
//         {
//             Values = values;
//             Key = key;
//             CategoryId = categoryId;
//             IsSearchable = isSearchable;
//             Fa_Name = fa_Name;
//             Type = type;
//             IsVariantable = isVariantable;
//             IsRequired = isRequired;
//             IsUnique = isUnique;
//         }

//         public UpdateAttributeByAdminCommand()
//         {
//         }

//         public class UpdateAttributeByAdminCommandHandler : BaseRequestCommandHandler<UpdateAttributeByAdminCommand, ServiceResult<string>>
//         {
//             private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
//             public UpdateAttributeByAdminCommandHandler(ILogger<UpdateAttributeByAdminCommand> logger,IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
//             {
//                 this.unitOfWork = unitOfWork;
//             }

//             protected override async Task<ServiceResult<string>> HandleAsync(UpdateAttributeByAdminCommand command, CancellationToken cancellationToken)
//             {



//                 var sr = ServiceResult.Empty;
//                 var repoAttribute = unitOfWork.GetRepository<AttributeEntity>();
//                 var repoCategory = unitOfWork.GetRepository<CategoryEntity>();


//                 // All Attributes
//                 var attributes = repoAttribute.GetQueryale()
//                                               .Where(x=>x.CategoryId == command.CategoryId)
//                                               .AsNoTracking()
//                                               .ToList();


//                 // searching for Attribute with specified Id
//                 var findAttribute = repoAttribute.GetFirstOrDefault(predicate: x=> x.Key ==command.Key && x.CategoryId == command.CategoryId);
//                 if(findAttribute==null)
//                     return sr.SetError(Errors.AttributeNotFound.GetMessage() , Errors.AttributeNotFound.GetCode())
//                                 .SetMessage(Errors.AttributeNotFound.GetPersionMessage())
//                                 .To<string>();



//                 var AllAttributes = repoAttribute.GetQueryale()
//                                                  .Where(x=>x.CategoryId==command.CategoryId)
//                                                  .AsNoTracking()
//                                                  .ToList();




//                 if(AllAttributes.Any(x=>x.IsVariantable == true && x.CategoryId == command.CategoryId) && command.IsVariantable == true)
//                    return sr.SetError(Errors.OnlyOneAttributeCanBeSetVariantable.GetMessage() , Errors.OnlyOneAttributeCanBeSetVariantable.GetCode())
//                             .SetMessage(Errors.OnlyOneAttributeCanBeSetVariantable.GetPersionMessage())
//                             .To<string>();



                
//                 if(AllAttributes.Any(x=>x.IsUnique == true && x.CategoryId == command.CategoryId) && command.IsUnique == true)
//                    return sr.SetError(Errors.OnlyOneAttributeCanBeSetUnique.GetMessage() , Errors.OnlyOneAttributeCanBeSetUnique.GetCode())
//                             .SetMessage(Errors.OnlyOneAttributeCanBeSetUnique.GetPersionMessage())
//                             .To<string>();



                            
//                 // Update Attribute
//                 findAttribute.Modify();
//                 findAttribute = command.Adapt(findAttribute);
//                 findAttribute.ValidateType();
//                 findAttribute.ValidateVariantUnique(attributes);
//                 findAttribute.ValidateUniqness(attributes);
//                 findAttribute.ValidateVariantable(attributes);
//                 findAttribute.ValidateRequirement();
//                 findAttribute.CheckVariantAttributeIsUniq();
//                 findAttribute.CheckVariantAttributeShouldBeSingleSelectType();
//                 findAttribute.setValue();


                
//                 repoAttribute.Update(findAttribute);



//                 // Save Into Database
//                 unitOfWork.SaveChanges();




//                 return ServiceResult.Create<string>(findAttribute.Id);


                
//             }

//             protected override ServiceResult<string> HandleOnError(System.Exception exp)
//             {
//                 var sr = ServiceResult.Empty;
//                 sr = sr.SetError("UnHandled");
//                 sr.Error.ExteraFields = new KeyValueList<string, string>(){
//                     new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
//                     new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
//                 };

//                 return sr.To<string>();
//             }
//         }

//     }
// }