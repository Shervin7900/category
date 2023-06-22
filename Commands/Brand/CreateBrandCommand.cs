using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Commands.Brand
{
    public class CreateBrandCommand : CommandBase<ServiceResult<string>>
    {
        public string En_Name{get; set;}

        public string CategoryId{get; set;}

        public string Fa_Name{get; set;}

        public Domains.Entities.Status Status{get; set;}

        public string Description{get; set;}

        public CreateBrandCommand(string en_Name, string categoryId, string fa_Name, Domains.Entities.Status status, string description)
        {
            En_Name = en_Name;
            CategoryId = categoryId;
            Fa_Name = fa_Name;
            Status = status;
            Description = description;
        }

        public CreateBrandCommand()
        {
        }


        public class CreateBrandCommandHandler : BaseRequestCommandHandler<CreateBrandCommand, ServiceResult<string>>
        {
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            public CreateBrandCommandHandler(ILogger<CreateBrandCommand> logger,IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected override async Task<ServiceResult<string>> HandleAsync(CreateBrandCommand command, CancellationToken cancellationToken)
            {
                var sr = ServiceResult.Empty;
                var repoBrand = unitOfWork.GetRepository<BrandEntity>();
                var repoCategory = unitOfWork.GetRepository<CategoryEntity>();




                // Get All Brands
                var brands = repoBrand.GetQueryale()
                                      .Where(x=>x.CategoryId == command.CategoryId)
                                      .AsNoTracking()
                                      .ToList();


                

                // Get BrandEntity
                var categoryEntity = repoCategory.GetFirstOrDefault(predicate :x=>x.Id==command.CategoryId);
                if(categoryEntity==null)
                   return sr.SetError(Errors.CategoryNotFound.GetMessage() , Errors.CategoryNotFound.GetCode())
                                .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                                .To<string>();




                // Get BrandEntity
                var brandEntity = repoBrand.GetFirstOrDefault(predicate :x=>x.En_Name == command.En_Name && x.CategoryId == command.CategoryId && x.IsDeleted ==false);
                if(brandEntity!=null)
                   return sr.SetError(Errors.BrandAlreadyExist.GetMessage() , Errors.BrandAlreadyExist.GetCode())
                                .SetMessage(Errors.BrandAlreadyExist.GetPersionMessage())
                                .To<string>();

                



                // Insert Brand
                brandEntity = command.Adapt<BrandEntity>();
                brandEntity.IsVisible();
                brandEntity.Active();
                repoBrand.Insert(brandEntity);



                // Saving To Database
                unitOfWork.SaveChanges();


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