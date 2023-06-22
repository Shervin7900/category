using System.ComponentModel.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;
using CategoryApi.Domains.Entities;
using MH.DDD.DataAccessLayer.Repository;
using CategoryApi.Infrastructures.CommissionOption;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Dtos.FileStorageServices;
using CategoryApi.Dtos.Enums;

namespace CategoryApi.Commands.Category
{
    public class CreateCategoryCommand : CommandBase<ServiceResult<string>>
    {
        public string Title{get; set;}

        public string ParentId{get; set;}
        
        public string Description{get; set;}

        public string Icon {get; set;}

        public UploadImageDto UploadImageDto {get; set;}

        public bool IsAcceptProduct{get; set;}

        public CreateCategoryCommand(string title, string parentId, string description, string icon, UploadImageDto uploadImageDto, bool isAcceptProduct)
        {
            Title = title;
            ParentId = parentId;
            Description = description;
            Icon = icon;
            UploadImageDto = uploadImageDto;
            IsAcceptProduct = isAcceptProduct;
        }

        public CreateCategoryCommand()
        {
        }





        public class CreateCategoryCommandHandler : BaseRequestCommandHandler<CreateCategoryCommand, ServiceResult<string>>
        {

            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            private readonly CommissionOption commissionOption;
            private readonly IFileStorageService _fileStorageService;

            public CreateCategoryCommandHandler(ILogger<CreateCategoryCommand> logger, IUnitOfWork<CategoryDbContext> unitOfWork, CommissionOption commissionOption, IFileStorageService fileStorageService) : base(logger)
            {

                this.unitOfWork = unitOfWork;
                this.commissionOption = commissionOption;
                _fileStorageService = fileStorageService;
            }


            protected override async Task<ServiceResult<string>> HandleAsync(CreateCategoryCommand command, CancellationToken cancellationToken)
            {



                var sr = ServiceResult.Empty;
                var repoCategory = unitOfWork.GetRepository<CategoryEntity>();
                var repoBrand = unitOfWork.GetRepository<BrandEntity>();
                var repoCommission = unitOfWork.GetRepository<CommissionEntity>();
                CategoryEntity categoryEntity = null;




                var rootEntity = repoCategory.GetFirstOrDefault(predicate : x=>x.ParentId == null);


                
                if(command.ParentId == null)
                {
                    command.ParentId = rootEntity.Id;
                }





                // Validate NameCategory
                if(repoCategory.GetQueryale().Any(x=> x.Title == command.Title && x.ParentId == command.ParentId))


                return sr.SetError(Errors.CategoryAlreadyExist.GetMessage() , Errors.CategoryAlreadyExist.GetCode())
                            .SetMessage(Errors.CategoryAlreadyExist.GetPersionMessage())
                            .To<string>();   





                
                // Get CategoryEntity
                if(!repoCategory.GetQueryale().Any( x => x.Id == command.ParentId))


                   return sr.SetError(Errors.ParentNotFound.GetMessage() , Errors.ParentNotFound.GetCode())
                                .SetMessage(Errors.ParentNotFound.GetPersionMessage())
                                .To<string>();
                

                
                if(command.ParentId!=null)
                {
                    // var iconId = Guid.NewGuid().ToString();
                    var imageId = Guid.NewGuid().ToString();
                    
                    // For Upload Icon In Future    

                    // // Upload Category Icon
                    // var uploadIcon = await _fileStorageService.Upload(new UploadFileDto(iconId , command.UploadIconDto.FileContent , command.UploadIconDto.Extention , DirectoryTypeEnum.category));

                    // if(uploadIcon.HasError == true)
                    //     throw uploadIcon.ToException();

                    
                    // Upload Category Image
                    var uploadImage = await _fileStorageService.Upload(new UploadFileDto(imageId , command.UploadImageDto.FileContent , command.UploadImageDto.Extention , DirectoryTypeEnum.category));

                    if(uploadImage.HasError == true)
                        throw uploadImage.ToException();


                    

                    categoryEntity = CreateCategoryWhenParentIdIsNotNull(command, sr, repoCategory , _fileStorageService , command.Icon , imageId);

                }
                else
                {

                    // var iconId = Guid.NewGuid().ToString();
                    var imageId = Guid.NewGuid().ToString();
                    
                    // For Upload Icon In Future    
                    
                    // // Upload Category Icon
                    // var uploadIcon = await _fileStorageService.Upload(new UploadFileDto(iconId , command.UploadIconDto.FileContent , command.UploadIconDto.Extention , DirectoryTypeEnum.category));

                    // if(uploadIcon.HasError == true)
                    //     throw uploadIcon.ToException();

                    
                    // Upload Category Image
                    var uploadImage = await _fileStorageService.Upload(new UploadFileDto(imageId , command.UploadImageDto.FileContent , command.UploadImageDto.Extention , DirectoryTypeEnum.category));

                    if(uploadImage.HasError == true)
                        throw uploadImage.ToException();


                        

                    categoryEntity = CreateCategoryWhenParentIdIsNull(command, repoCategory , _fileStorageService , command.Icon , imageId);
                    
                }

                var affiliateCommission = commissionOption.commissions.FirstOrDefault(x=>x.CommissionType == "AffiliatorCommission");
                var booxellCommission = commissionOption.commissions.FirstOrDefault(x=>x.CommissionType == "BooxellCommission");
                var anotherShopCommission = commissionOption.commissions.FirstOrDefault(x=>x.CommissionType == "AnotherShopCommission");


                // Save Into Database
                // Insert Category
                // Insert Default Brand (other)
                repoCategory.Insert(categoryEntity);
                repoBrand.Insert(new BrandEntity("other" , categoryEntity.Id , "متفرقه" , Domains.Entities.Status.Active , ""));

                // Insert Commission
                var commission = new CommissionEntity(categoryEntity.Id ,affiliateCommission.CommissionPercentage + anotherShopCommission.CommissionPercentage + booxellCommission.CommissionPercentage ,  affiliateCommission.CommissionPercentage , anotherShopCommission.CommissionPercentage , booxellCommission.CommissionPercentage);
                repoCommission.Insert(commission);
                
                unitOfWork.SaveChanges();
            


                return ServiceResult.Create<string>(categoryEntity.Id);
                


            }


            private static CategoryEntity CreateCategoryWhenParentIdIsNull(CreateCategoryCommand command, IRepository<CategoryEntity> repoCategory, IFileStorageService _fileStorageService, string iconId, string imageId)
            {

                CategoryEntity categoryEntity;
                // Searching for the Root
                var gettingTheRoot = HierarchyId.GetRoot();


                




                // Searching for Child Of The Root
                var child = repoCategory.GetQueryale().Where(x => x.HierarchyId.GetAncestor(1) == gettingTheRoot).OrderBy(x => x.HierarchyId).LastOrDefault();



                // Generating a new Category Entity
                categoryEntity = new CategoryEntity(gettingTheRoot.GetDescendant(child != null ? child.HierarchyId : null, null), command.Title, command.Description, command.IsAcceptProduct, command.ParentId, iconId, imageId);




                // Insert Category
                return categoryEntity;

            }


            private static CategoryEntity CreateCategoryWhenParentIdIsNotNull(CreateCategoryCommand command, ServiceResult sr, IRepository<CategoryEntity> repoCategory, IFileStorageService _fileStorageService, string iconId, string imageId)
            {

                CategoryEntity categoryEntity;
                var searchForParent = repoCategory.GetQueryale().FirstOrDefault(x => x.Id == command.ParentId);
                if (searchForParent == null)
                    throw sr.SetError(Errors.ParentNotFound.GetMessage(), Errors.ParentNotFound.GetCode())
                                .SetMessage(Errors.ParentNotFound.GetPersionMessage())
                                .ToException();




                // Searching for Child Of The Parent                
                var child = repoCategory.GetQueryale().Where(x => x.HierarchyId.GetAncestor(1) == searchForParent.HierarchyId).OrderBy(x => x.HierarchyId).LastOrDefault();





                // Generating A new Category Entity
                categoryEntity = new CategoryEntity(searchForParent?.HierarchyId.GetDescendant(child != null ? child.HierarchyId : null, null), command.Title, command.Description, command.IsAcceptProduct, command.ParentId, iconId, imageId);



                // Insert Category
                return categoryEntity;

            }


            protected override ServiceResult<string> HandleOnError(System.Exception exp)
            {    
            
                var sr = ServiceResult.Empty;
                sr = sr.SetError(exp.Message);

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<string>();

            }


        }
    }
}