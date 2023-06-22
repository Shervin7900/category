using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.CategoryQueries
{
    public class GetBreadCrumbQuery: QueryBase<ServiceResult<GetBreadCrumbOutPutDto>>
    {


        public string CategoryId { get; set; }



        public GetBreadCrumbQuery(string categoryId)
        {
            CategoryId = categoryId;
        }



        public GetBreadCrumbQuery()
        {
        }



        public class GetBreadCrumbQueryHandler : BaseRequestQueryHandler<GetBreadCrumbQuery, ServiceResult<GetBreadCrumbOutPutDto>>
        {


            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;


            public GetBreadCrumbQueryHandler(ILogger<GetBreadCrumbQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }


            protected override async Task<ServiceResult<GetBreadCrumbOutPutDto>> HandleAsync(GetBreadCrumbQuery query, CancellationToken cancellationToken)
            {


                var sr = ServiceResult.Empty;
                var repoCategory = unitOfWork.GetRepository<CategoryEntity>();

                var dto = new GetBreadCrumbOutPutDto();

                



                // Searching for Category with specified CategoryId 
                var search = repoCategory.GetQueryale()
                                                  .AsNoTracking()
                                                  .FirstOrDefault(predicate: x => x.Id == query.CategoryId);

                if(search != null)
                {

                    dto.BreadCrumb.Add(search.Adapt<GetBreadCrumbChildrenDto>());

                    var ParentId = search.ParentId;
                    while(ParentId != null)
                    {
                        var parentSearch = repoCategory.GetQueryale()
                                    .FirstOrDefault(x=>x.Id  == ParentId);

                        dto.BreadCrumb.Add(parentSearch.Adapt<GetBreadCrumbChildrenDto>());

                        if(parentSearch.ParentId == null)
                            break;

                        ParentId = parentSearch.ParentId;
                    }

                    var root = dto.BreadCrumb.FirstOrDefault(x=>x.Title == "base Category");
                    dto.BreadCrumb.Remove(root);
                    
                }
                

                return ServiceResult.Create<GetBreadCrumbOutPutDto>(dto);



            }

            protected override ServiceResult<GetBreadCrumbOutPutDto> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<GetBreadCrumbOutPutDto>();
                
            }
        }
        
    }
}