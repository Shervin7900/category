using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Dtos.AttributeDtos;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Queries.Seller
{
    public class GetAllAttributesByIsVariantableBySellerQuery: QueryBase<ServiceResult<List<GetAllAttributesDto>>>
    {
        public GetAllAttributesByIsVariantableBySellerQuery(bool isVariantabale, string categoryId)
        {
            IsVariantabale = isVariantabale;
            this.categoryId = categoryId;
        }

        public bool IsVariantabale { get; set; }
        public string categoryId { get; set; }


        public class GetAllAttributesByIsVariantableBySellerQueryHandler : BaseRequestQueryHandler<GetAllAttributesByIsVariantableBySellerQuery, ServiceResult<List<GetAllAttributesDto>>>
        {

            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;

            public GetAllAttributesByIsVariantableBySellerQueryHandler(ILogger<GetAllAttributesByIsVariantableBySellerQueryHandler> logger, IUnitOfWork<CategoryDbContext> _unitOfWork) : base(logger)
            {

                unitOfWork = _unitOfWork;

            }

            protected override async Task<ServiceResult<List<GetAllAttributesDto>>> HandleAsync(GetAllAttributesByIsVariantableBySellerQuery query, CancellationToken cancellationToken)
            {


                var sr = ServiceResult.Empty;
                var attributeRepo = unitOfWork.GetRepository<AttributeEntity>();
                var categoryRepo = unitOfWork.GetRepository<CategoryEntity>();
                var categoryAttributeRepo = unitOfWork.GetRepository<CategoryAttributeEntity>();
                var attributeValueRepo = unitOfWork.GetRepository<AttributeValueEntity>();




                // Validate Category
                bool isCategoryExists = categoryRepo.GetQueryale().Any(x => x.Id == query.categoryId);

                if(!isCategoryExists)
                    return sr.SetError(Errors.CategoryNotFound.GetMessage(), Errors.CategoryNotFound.GetCode())
                                 .SetMessage(Errors.CategoryNotFound.GetPersionMessage())
                                 .To<List<GetAllAttributesDto>>();



                // finding the ProductAttributes 
                var categoryAttributesId = categoryAttributeRepo.GetQueryale().Where(x => x.CategoryId == query.categoryId ).Select(x => x.AttributeId ).ToList();
                var attributes = attributeRepo.GetQueryale().AsNoTracking().Where(x => categoryAttributesId.Contains(x.Id) && x.IsVariantable == query.IsVariantabale ).ToList();


                // Check If Category has IsInCard Attribute
                var IsInCardAttibutes = attributes.Where(x => x.IsInCard == true).ToList();
                if(IsInCardAttibutes.Distinct().Count() > 1 )
                    return sr.SetError(Errors.OnlyOneAttributeCanBeInCard.GetMessage(), Errors.OnlyOneAttributeCanBeInCard.GetCode())
                        .SetMessage(Errors.OnlyOneAttributeCanBeInCard.GetPersionMessage())
                        .To<List<GetAllAttributesDto>>();

                if(query.IsVariantabale)
                    if(IsInCardAttibutes.Any(x => x.IsVariantable == true))
                        return sr.SetError(Errors.InCardAttributeCantBeVariantable.GetMessage(), Errors.InCardAttributeCantBeVariantable.GetCode())
                            .SetMessage(Errors.InCardAttributeCantBeVariantable.GetPersionMessage())
                            .To<List<GetAllAttributesDto>>();
                        

                // attributes.ForEach(X => X.setValues());
                var searchableAttribute = attributes
                                            .Where(x => x.IsSearchableInValues == false )
                                            .Select(x => x.Key)
                                            .ToList();
             


                var Allvalues = attributeValueRepo.GetQueryale()
                                               .Where(x =>  searchableAttribute.Contains(x.Key) )
                                               .ToList()
                                               .GroupBy(x => x.Key)
                                               .Select( x => new {x.Key , values = x.Select(x => x.Value).ToList()})
                                               .ToList();

                
                // Set Values
                attributes.ForEach( x => 
                {
                    var AttributeValues = Allvalues.FirstOrDefault(s => s.Key == x.Key);
                    if(AttributeValues != null)
                    {
                        x.setValues(AttributeValues.values);
                    }
                    else
                    {
                        x.setValues(new List<string>() {});
                    }
                });

                return ServiceResult.Create<List<GetAllAttributesDto>>(attributes.Adapt<List<GetAllAttributesDto>>());

            }

            protected override ServiceResult<List<GetAllAttributesDto>> HandleOnError(System.Exception exp)
            {

                var sr = ServiceResult.Empty;
                sr = sr.SetError("UnHandled");

                sr.Error.ExteraFields = new KeyValueList<string, string>(){
                    new KeyValuePair<string, string>(nameof(exp.Message),exp.Message),
                    new KeyValuePair<string, string>(nameof(exp.Source),exp.Source),
                    new KeyValuePair<string, string>(nameof(exp.InnerException),exp.InnerException?.Message ?? ""),
                };

                return sr.To<List<GetAllAttributesDto>>();

            }
        }
        
    }
}