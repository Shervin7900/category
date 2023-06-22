using MH.DDD.DataAccessLayer.UnitOfWork;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.Core.ImagePattern;
using CategoryApi.Dtos.Enums;

namespace CategoryApi.Queries.CategoryQueries
{
    public class GetCategoryInfoByAdminQuery : QueryBase<ServiceResult<GetCategoryInfoByAdminDto>>
    {


        public string ParentId { get; set; }



        public GetCategoryInfoByAdminQuery(string parentId)
        {
            ParentId = parentId;
        }



        public GetCategoryInfoByAdminQuery()
        {
        }



        public class GetCategoryInfoByAdminQueryHandler : BaseRequestQueryHandler<GetCategoryInfoByAdminQuery, ServiceResult<GetCategoryInfoByAdminDto>>
        {


            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            private ImagePatternOption imagePatternOption { get; set; }


            public GetCategoryInfoByAdminQueryHandler(ILogger<GetCategoryInfoByAdminQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork, ImagePatternOption imagePatternOption) : base(logger)
            {

                unitOfWork = _unitOfWork;
                this.imagePatternOption = imagePatternOption;
            }


            protected override async Task<ServiceResult<GetCategoryInfoByAdminDto>> HandleAsync(GetCategoryInfoByAdminQuery query, CancellationToken cancellationToken)
            {


                var sr = ServiceResult.Empty;
                var repoCategory = unitOfWork.GetRepository<CategoryEntity>();



                // Finding the Root
                var rootEntity = repoCategory.GetQueryale().FirstOrDefault(x => x.ParentId == null);



                if (query.ParentId == null)
                {
                    query.ParentId = rootEntity.Id;
                }



                // Searching for Category with specified CategoryId 
                var searchForParent = repoCategory.GetQueryale()
                                                  .AsNoTracking()
                                                  .FirstOrDefault(predicate: x => x.Id == query.ParentId);


                if (searchForParent == null)
                    return sr.SetError(Errors.ParentNotFound.GetMessage(), Errors.ParentNotFound.GetCode())
                    .SetMessage(Errors.ParentNotFound.GetPersionMessage())
                    .To<GetCategoryInfoByAdminDto>();



                // Searching for Child Of The Parent             
                var children = repoCategory.GetQueryale()
                                           .Include(c => c.Children)
                                           .Where(x => x.HierarchyId.GetAncestor(1) == searchForParent.HierarchyId)
                                           .OrderBy(x => x.HierarchyId)
                                           .AsNoTracking()
                                           .ToList();



                var childrenAdapter = children.Adapt<List<GetCategoryInfoByAdminChildrenDto>>();

                childrenAdapter.ForEach(x=>{
                    // if(x.Icon != null && x.Icon != String.Empty)
                    //     x.Icon = imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Icon);
                    if(x.Image != null && x.Image != String.Empty)
                    x.Image = imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Image);
                });
                

                


                var parentAdapter = searchForParent.Adapt<GetCategoryInfoByAdminDto>();



                parentAdapter.Children = childrenAdapter;




                return ServiceResult.Create<GetCategoryInfoByAdminDto>(parentAdapter);



            }

            protected override ServiceResult<GetCategoryInfoByAdminDto> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<GetCategoryInfoByAdminDto>();
                
            }
        }
    }
}