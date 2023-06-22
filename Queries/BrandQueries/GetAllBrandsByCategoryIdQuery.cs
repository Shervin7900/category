using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.BrandDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.Core.Expressions;
using MH.DDD.DataAccessLayer.PagedList;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.BrandQueries
{
    public class GetAllBrandsByCategoryIdQuery : QueryBase<ServiceResult<IPagedList<GetBrandOutPutDto>>>
    {
        
        public TermFilter tf {get;set;}
        public string CategoryId{get; set;}


        public GetAllBrandsByCategoryIdQuery(TermFilter tf, string categoryId)
        {
            this.tf = tf;
            CategoryId = categoryId;
        }


        public GetAllBrandsByCategoryIdQuery()
        {
        }


         public class GetAllBrandsByCategoryIdQueryHandler : BaseRequestQueryHandler<GetAllBrandsByCategoryIdQuery, ServiceResult<IPagedList<GetBrandOutPutDto>>>
        {


            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;


            public GetAllBrandsByCategoryIdQueryHandler(ILogger<GetAllBrandsByCategoryIdQueryHandler> logger,IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }


            protected override async Task<ServiceResult<IPagedList<GetBrandOutPutDto>>> HandleAsync(GetAllBrandsByCategoryIdQuery query, CancellationToken cancellationToken)
            {
            

               var sr = ServiceResult.Empty;
               var repoBrand = unitOfWork.GetRepository<BrandEntity>();
               var repoCategory = unitOfWork.GetRepository<CategoryEntity>();



               // Initializing Page List
               query.tf = query.tf.CheckNullDefault();



               // finding Category
               var CategoryEntity = repoCategory.GetQueryale()
                                .Any(x=>x.Id == query.CategoryId);



               if(CategoryEntity == false)
                    return sr.SetError(Errors.CategoryNotFound.GetMessage() , Errors.CategoryNotFound.GetCode())
                            .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                            .To<IPagedList<GetBrandOutPutDto>>();



               // Page List Of Brands in A  Specific Category
               var PagedListOfBrand = repoBrand.GetPagedList(x=>x.CategoryId.Equals(query.CategoryId) && x.IsDeleted != true , null  ,null , query.tf.PgNumber ,query.tf.PgSize , false);




               return ServiceResult.Create<IPagedList<GetBrandOutPutDto>>(PagedListOfBrand.Adapt<IPagedList<GetBrandOutPutDto>>());

            }

            protected override ServiceResult<IPagedList<GetBrandOutPutDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<IPagedList<GetBrandOutPutDto>>();

            }
        }
    }
}