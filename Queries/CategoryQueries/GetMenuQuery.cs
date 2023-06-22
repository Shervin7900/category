using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MH.DDD.DataAccessLayer.UnitOfWork;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.CategoryDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.Core.ImagePattern;
using CategoryApi.Dtos.Enums;

namespace CategoryApi.Queries.CategoryQueries
{
    public class GetMenuQuery : QueryBase<ServiceResult<GetCategoryInfoByUserDto>>
    {

        public string CategoryId { get; set; }

        public GetMenuQuery()
        {
        }


        public class GetMenuQueryHandler : BaseRequestQueryHandler<GetMenuQuery, ServiceResult<GetCategoryInfoByUserDto>>
        {
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            private ImagePatternOption imagePatternOption { get; set; }

            public GetMenuQueryHandler(ILogger<GetMenuQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork, ImagePatternOption imagePatternOption) : base(logger)
            {
                unitOfWork = _unitOfWork;
                this.imagePatternOption = imagePatternOption;
            }

            protected override async Task<ServiceResult<GetCategoryInfoByUserDto>> HandleAsync(GetMenuQuery query, CancellationToken cancellationToken)
            {
            
               var sr = ServiceResult.Empty;
               var repoCategory = unitOfWork.GetRepository<CategoryEntity>();


                // finding the Root
                var rootEntity = repoCategory.GetQueryale()
                            .FirstOrDefault(x=>x.ParentId  == null);



                // Searching for Category with specified CategoryId 
                var searchForParent = repoCategory.GetQueryale()
                    .FirstOrDefault(predicate : x=>x.Id == rootEntity.Id);


                                                    
                if(searchForParent==null)
                        return sr.SetError(Errors.ParentNotFound.GetMessage() , Errors.ParentNotFound.GetCode())
                        .SetMessage(Errors.ParentNotFound.GetPersionMessage())
                        .To<GetCategoryInfoByUserDto>();




                    // Searching for Child Of The Parent                
                    var children = repoCategory.GetQueryale()
                    .Where(x=>x.HierarchyId.GetAncestor(1)==searchForParent.HierarchyId)
                    .OrderBy(x=>x.HierarchyId).ToList();

                    children.ForEach(x=>{
                        // if(x.Icon != null && x.Icon != String.Empty)
                        //     x.setIcon(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Icon));
                        if(x.Image != null && x.Image != String.Empty)
                            x.setImage(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Image));
                    });



                    var childrenOfParentChildren = repoCategory.GetQueryale()
                    .Where(x=>x.HierarchyId.GetAncestor(2)==searchForParent.HierarchyId)
                    .OrderBy(x=>x.HierarchyId).ToList();

                    childrenOfParentChildren.ForEach(x=>{
                        // if(x.Icon != null && x.Icon != String.Empty)
                        //     x.setIcon(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Icon));
                        if(x.Image != null && x.Image != String.Empty)
                            x.setImage(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Image));
                    });


                    
                    var childrenOfParentChildrenWithLevel3 = repoCategory.GetQueryale()
                    .Where(x=>x.HierarchyId.GetAncestor(3)==searchForParent.HierarchyId)
                    .OrderBy(x=>x.HierarchyId).ToList();

                    childrenOfParentChildrenWithLevel3.ForEach(x=>{
                        // if(x.Icon != null && x.Icon != String.Empty)
                        //     x.setIcon(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Icon));
                        if(x.Image != null && x.Image != String.Empty)
                            x.setImage(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Image));
                    });


                    var childrenOfParentChildrenWithLevel4 = repoCategory.GetQueryale()
                    .Where(x=>x.HierarchyId.GetAncestor(4)==searchForParent.HierarchyId)
                    .OrderBy(x=>x.HierarchyId).ToList();

                    childrenOfParentChildrenWithLevel4.ForEach(x=>{
                        // if(x.Icon != null && x.Icon != String.Empty)
                        //     x.setIcon(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Icon));
                        if(x.Image != null && x.Image != String.Empty)
                            x.setImage(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Image));
                    });

                    
                    var childrenOfParentChildrenWithLevel5 = repoCategory.GetQueryale()
                    .Where(x=>x.HierarchyId.GetAncestor(5)==searchForParent.HierarchyId)
                    .OrderBy(x=>x.HierarchyId).ToList();

                    childrenOfParentChildrenWithLevel5.ForEach(x=>{
                        // if(x.Icon != null && x.Icon != String.Empty)
                        //     x.setIcon(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Icon));
                        if(x.Image != null && x.Image != String.Empty)
                            x.setImage(imagePatternOption.getUrl(DirectoryTypeEnum.category.ToString().ToLower() , x.Image));
                    });



                return ServiceResult.Create<GetCategoryInfoByUserDto>(searchForParent.Adapt<GetCategoryInfoByUserDto>());

            }

            protected override ServiceResult<GetCategoryInfoByUserDto> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<GetCategoryInfoByUserDto>();
                
            }
        }
    }
}