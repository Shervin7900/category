using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using CategoryApi.Messages.Brand;
using MH.DDD.DataAccessLayer.UnitOfWork;
using src.CategoryApi.Dtos.BrandDtos;

namespace CategoryApi.Commands.Brand
{
    public class UpdateBrandCommand : CommandBase<ServiceResult<string>>
    {


        public string En_Name { get; set; }

        public string BrandId { get; set; }

        public string Fa_Name { get; set; }

        public string Description { get; set; }

        public UpdateBrandCommand(string en_Name, string brandId, string fa_Name, string description)
        {

            En_Name = en_Name;
            BrandId = brandId;
            Fa_Name = fa_Name;
            Description = description;
        }

        public UpdateBrandCommand()
        {
        }


        public class UpdateBrandCommandHandler : BaseRequestCommandHandler<UpdateBrandCommand, ServiceResult<string>>
        {

            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            private INotifyService _notifyService;

            public UpdateBrandCommandHandler(ILogger<UpdateBrandCommand> logger, IUnitOfWork<CategoryDbContext> unitOfWork, INotifyService notifyService) : base(logger)
            {

                this.unitOfWork = unitOfWork;
                _notifyService = notifyService;
            }



            protected override async Task<ServiceResult<string>> HandleAsync(UpdateBrandCommand command, CancellationToken cancellationToken)
            {


                var sr = ServiceResult.Empty;
                var repoBrand = unitOfWork.GetRepository<BrandEntity>();
                var repoCategory = unitOfWork.GetRepository<CategoryEntity>();


                // Get BrandEntity
                var brandEntity = repoBrand.GetFirstOrDefault(predicate: x => x.Id == command.BrandId);
                if (brandEntity == null)
                    return sr.SetError(Errors.BrandNotFound.GetMessage(), Errors.BrandNotFound.GetCode())
                                 .SetMessage(Errors.BrandNotFound.GetPersionMessage())
                                 .To<string>();

                // Get All Brands include this category
                var brands = repoBrand.GetQueryale()
                                      .Where(x => x.CategoryId == brandEntity.CategoryId)
                                      .AsNoTracking()
                                      .ToList();

                var duplicateTitle = brands.Any(x => x.En_Name == command.En_Name);
                if (duplicateTitle)
                    return sr.SetError(Errors.TitleAlreadyExists.GetMessage(), Errors.TitleAlreadyExists.GetCode())
                                .SetMessage(Errors.TitleAlreadyExists.GetPersionMessage())
                                .To<string>();




                // Update Brand
                brandEntity.Modify();
                brandEntity = command.Adapt(brandEntity);
                repoBrand.Update(brandEntity);



                // Saving To Database
                unitOfWork.SaveChanges();

                //send event to search
                var inputEvent = new BrandDetailDto(brandEntity.Id, brandEntity.Fa_Name);
                await _notifyService.SendAsync<BrandUpdatedEvent>(new BrandUpdatedEvent(inputEvent));

                return ServiceResult.Create<string>(brandEntity.Id);


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