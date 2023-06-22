using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MH.DDD.DataAccessLayer.UnitOfWork;
using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using CategoryApi.Messages.Category;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Dtos.FileStorageServices;
using CategoryApi.Dtos.Enums;

namespace CategoryApi.Commands.Category
{
    public class UpdateCategoryCommand : CommandBase<ServiceResult<string>>
    {
        public string CategoryId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Icon {get; set;}

        public UploadImageDto UploadImageDto {get; set;}

        public bool IsAcceptProduct { get; set; }

        public UpdateCategoryCommand(string categoryId, string title, string description, string icon, UploadImageDto uploadImageDto, bool isAcceptProduct)
        {
            CategoryId = categoryId;
            Title = title;
            Description = description;
            Icon = icon;
            UploadImageDto = uploadImageDto;
            IsAcceptProduct = isAcceptProduct;
        }

        public UpdateCategoryCommand()
        {
        }



        public class UpdateCategoryCommandHandler : BaseRequestCommandHandler<UpdateCategoryCommand, ServiceResult<string>>
        {
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            private readonly IFileStorageService _fileStorageService;
            INotifyService _notifyService;
            public UpdateCategoryCommandHandler(ILogger<CreateCategoryCommand> logger, IUnitOfWork<CategoryDbContext> unitOfWork, INotifyService notifyService, IFileStorageService fileStorageService) : base(logger)
            {
                this.unitOfWork = unitOfWork;
                _notifyService = notifyService;
                _fileStorageService = fileStorageService;
            }

            protected override async Task<ServiceResult<string>> HandleAsync(UpdateCategoryCommand command, CancellationToken cancellationToken)
            {



                var sr = ServiceResult.Empty;
                var repoCategory = unitOfWork.GetRepository<CategoryEntity>();


                // var iconId = Guid.NewGuid().ToString();
                var imageId = Guid.NewGuid().ToString();





                // Serching for Category with given CategoryId
                var findCategory = repoCategory.GetFirstOrDefault(predicate: x => x.Id == command.CategoryId);
                if (findCategory == null)
                    return sr.SetError(Errors.CategoryNotFound.GetMessage(), Errors.CategoryNotFound.GetCode())
                                 .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                                 .To<string>();

                


                // Validate NameCategory
                if (repoCategory.GetQueryale().Any(x => x.Title == command.Title && x.Id != command.CategoryId && x.ParentId == findCategory.ParentId))
                    return sr.SetError(Errors.CategoryAlreadyExist.GetMessage(), Errors.CategoryAlreadyExist.GetCode())
                                 .SetMessage(Errors.CategoryAlreadyExist.GetPersionMessage())
                                 .To<string>();





                
                // For Upload Icon In Future    
                
                // if(command.UploadIconDto != null && command.UploadIconDto.FileContent != null)
                // {

                //     // Delete Category Previous Icon
                //     var deleteIcon = await _fileStorageService.DeleteFile(new DeleteFileInputDto(DirectoryTypeEnum.category.ToString() , findCategory.Icon + ".jpg"));

                //     if(deleteIcon.HasError == true)
                //         throw deleteIcon.ToException();


                //     // Upload Category Icon
                //     var uploadIcon = await _fileStorageService.Upload(new UploadFileDto(iconId , command.UploadIconDto.FileContent , command.UploadIconDto.Extention , DirectoryTypeEnum.category));

                //     if(uploadIcon.HasError == true)
                //         throw uploadIcon.ToException();


                //     findCategory.setIcon(iconId);

                // }

                if(command.UploadImageDto != null && command.UploadImageDto.FileContent != null)
                {
                    
                    // Delete Previous Category Image
                    var deleteImage = await _fileStorageService.DeleteFile(new DeleteFileInputDto(DirectoryTypeEnum.category.ToString() , findCategory.Image + ".jpg"));

                    if(deleteImage.HasError == true)
                        throw deleteImage.ToException();


                    // Upload Category Image
                    var uploadImage = await _fileStorageService.Upload(new UploadFileDto(imageId , command.UploadImageDto.FileContent , command.UploadImageDto.Extention , DirectoryTypeEnum.category));

                    if(uploadImage.HasError == true)
                            throw uploadImage.ToException();

                            
                    findCategory.setImage(imageId);

                }



                // Update Category
                findCategory.Modify();
                findCategory = command.Adapt(findCategory);
                findCategory.setIcon(command.Icon);
                repoCategory.Update(findCategory);




                // Save Into Database
                unitOfWork.SaveChanges();

                //send event 
                var inputeEvent = new GetCategoryDetailDto(findCategory.Id, findCategory.Title);
                await _notifyService.SendAsync<CategoryUpdatedEvent>(new CategoryUpdatedEvent(inputeEvent));




                return ServiceResult.Create<string>(findCategory.Id);


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