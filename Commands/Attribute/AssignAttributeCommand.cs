using System.Linq;
using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Commands.Attribute
{
    public class AssignAttributeCommand : CommandBase<ServiceResult<string>>
    {


        public string AttributeKey { get; set; }
        public string CategoryId { get; set; }
        

        public AssignAttributeCommand(string attributeKey, string categoryId)
        {
            AttributeKey = attributeKey;
            CategoryId = categoryId;
        }

        public class AssignAttributeCommandHandler : BaseRequestCommandHandler<AssignAttributeCommand, ServiceResult<string>>
        {
            
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;

            public AssignAttributeCommandHandler(Microsoft.Extensions.Logging.ILogger<AssignAttributeCommandHandler> logger, IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected async override Task<ServiceResult<string>> HandleAsync(AssignAttributeCommand command, CancellationToken cancellationToken)
            {



                var sr = ServiceResult.Empty;
                var repoAttribute = unitOfWork.GetRepository<AttributeEntity>();
                var repoCategory = unitOfWork.GetRepository<CategoryEntity>();
                var repoCategoryAttribute = unitOfWork.GetRepository<CategoryAttributeEntity>();



                // validate category
                if(!repoCategory.GetQueryale().Any( x => x.Id == command.CategoryId ))
                    return sr.SetError(Errors.CategoryNotFound.GetMessage() , Errors.CategoryNotFound.GetCode() )
                             .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                             .To<string>();



                // get new attribute
                var attribute = repoAttribute.GetQueryale().FirstOrDefault(x => x.Key == command.AttributeKey);
                
                
                if(attribute == null)
                    return sr.SetError(Errors.AttributeNotFound.GetMessage() , Errors.AttributeNotFound.GetCode() )
                             .SetMessage(Errors.AttributeNotFound.GetPersionMessage())
                             .To<string>();
                    


                // get all attribute category
                var CategoryAttributesId = repoCategoryAttribute.GetQueryale().Where(x => x.CategoryId == command.CategoryId).Select(x => x.AttributeId ).ToList();
                var AllAttributes = repoAttribute.GetQueryale().Where(x => CategoryAttributesId.Contains(x.Id)).ToList();


                var isExistAttribute = AllAttributes.Any(x => x.Key == command.AttributeKey);
                if(isExistAttribute)
                    return sr.SetError(Errors.AttributeExists.GetMessage() , Errors.AttributeExists.GetCode() )
                             .SetMessage(Errors.AttributeExists.GetPersionMessage())
                             .To<string>();


                
                if(AllAttributes.Any(x => x.IsVariantable) && attribute.IsVariantable)
                    return sr.SetError(Errors.OnlyOneAttributeCanBeSetVariantable.GetMessage() , Errors.OnlyOneAttributeCanBeSetVariantable.GetCode() )
                             .SetMessage(Errors.OnlyOneAttributeCanBeSetVariantable.GetPersionMessage())
                             .To<string>();
         
         
         
                if(AllAttributes.Any(x => x.IsUnique) && attribute.IsUnique)
                    return sr.SetError(Errors.OnlyOneAttributeCanBeSetUnique.GetMessage() , Errors.OnlyOneAttributeCanBeSetUnique.GetCode() )
                             .SetMessage(Errors.OnlyOneAttributeCanBeSetUnique.GetPersionMessage())
                             .To<string>();
                


                attribute.ValidateUniqness(AllAttributes);
                attribute.ValidateVariantable(AllAttributes);
                // validate and insert 

                
                repoCategoryAttribute.Insert(new CategoryAttributeEntity(attribute.Id , command.CategoryId));
                unitOfWork.SaveChanges();

                return sr.To<string>(attribute.Key);
            }
        }


    }
}