using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Commands.Category
{
    public class ValidateListOfCategoriesAndBrandsCommand : CommandBase<ServiceResult<bool>>
    {
        public List<string> CategoryIds{get; set;}

        public List<string> BrandIds{get; set;}

        public ValidateListOfCategoriesAndBrandsCommand(List<string> categoryIds, List<string> brandIds)
        {
            CategoryIds = categoryIds;
            BrandIds = brandIds;
        }

        public ValidateListOfCategoriesAndBrandsCommand()
        {
        }



        public class ValidateListOfCategoriesAndBrandsCommandHandler : BaseRequestCommandHandler<ValidateListOfCategoriesAndBrandsCommand, ServiceResult<bool>>
        {
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            public ValidateListOfCategoriesAndBrandsCommandHandler(ILogger<CreateCategoryCommand> logger,IUnitOfWork<CategoryDbContext> unitOfWork) : base(logger)
            {
                this.unitOfWork = unitOfWork;
            }

            protected override async Task<ServiceResult<bool>> HandleAsync(ValidateListOfCategoriesAndBrandsCommand command, CancellationToken cancellationToken)
            {



                var sr = ServiceResult.Empty;
                var CategoryRepository = unitOfWork.GetRepository<CategoryEntity>();
                var BrandRepository = unitOfWork.GetRepository<BrandEntity>();




                var BrandsList = BrandRepository
                                            .GetQueryale()
                                            .Where(x=>command.BrandIds.Contains(x.Id) && command.CategoryIds.Contains(x.CategoryId))
                                            .Distinct()
                                            .Count();


                bool isValid = true;

                if(BrandsList == 0)
                    isValid = false;















                // command.BrandIds.ForEach(d=>{

                //     if(!ListOfBrandIds.Any(x=>command.CategoryIds.Contains(x.CategoryId) && x.Id == d))
                //     {
                //         isValid = false;
                //     }
                //     // if(!ListOfBrandIds.Any(x=>command.CategoryIds.Contains(x.CategoryId)))
                //     // {
                //     //     isValid = false;
                //     // }
                //     // if(!ListOfBrandIds.Select(x=>x.Id).Contains(d))
                //     // {
                //     //     isValid =  false; 
                //     // }

                // });

                
                return ServiceResult.Create<bool>(isValid);


            }
             protected override ServiceResult<bool> HandleOnError(System.Exception exp)
            {
                var sr = ServiceResult.Empty;
                sr = sr.SetError(exp.Message);
                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<bool>();
            }
        }
    }
}