using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CategoryApi.Domains.Entities;
using CategoryApi.Infrastructures.DbContexts;
using MH.DDD.DataAccessLayer.UnitOfWork;

namespace CategoryApi.Commands.Commission
{
    public class UpdateCommissionCommand: CommandBase<ServiceResult<string>>
    {
        public string Id{get; set;}

        public int AffiliatorCommission{get; set;}

        public int AnotherShopCommission{get; set;}

        public int BooxellCommission{get; set;}

        public UpdateCommissionCommand( int affiliatorCommission, int anotherShopCommission, string id, int booxellCommission)
        {
            AffiliatorCommission = affiliatorCommission;
            AnotherShopCommission = anotherShopCommission;
            Id = id;
            BooxellCommission = booxellCommission;
        }

        public UpdateCommissionCommand()
        {
        }



        public class UpdateCommissionCommandHandler : BaseRequestCommandHandler<UpdateCommissionCommand, ServiceResult<string>>
        {
            private readonly IUnitOfWork<CategoryDbContext> unitOfWork;
            INotifyService _notifyService;
            public UpdateCommissionCommandHandler(ILogger<UpdateCommissionCommand> logger, IUnitOfWork<CategoryDbContext> unitOfWork, INotifyService notifyService) : base(logger)
            {
                this.unitOfWork = unitOfWork;
                _notifyService = notifyService;
            }

            protected override async Task<ServiceResult<string>> HandleAsync(UpdateCommissionCommand command, CancellationToken cancellationToken)
            {



                var sr = ServiceResult.Empty;
                var repoCommission = unitOfWork.GetRepository<CommissionEntity>();
                var repoCategory = unitOfWork.GetRepository<CategoryEntity>();








                // Serching for Commission with given Id
                var findCommission = repoCommission.GetFirstOrDefault(predicate: x => x.Id == command.Id);
                if (findCommission == null)
                    return sr.SetError(Errors.CommissionNotFound.GetMessage(), Errors.CommissionNotFound.GetCode())
                                 .SetMessage(Errors.CommissionNotFound.GetPersionMessage())
                                 .To<string>();


                

                if( 100 < command.BooxellCommission + command.AffiliatorCommission + command.AnotherShopCommission)
                    return sr.SetError(Errors.CommissionValidationFailed.GetMessage(), Errors.CommissionValidationFailed.GetCode())
                                 .SetMessage(Errors.CommissionValidationFailed.GetPersionMessage())
                                 .To<string>();





                // Update Commission
                findCommission.Modify();
                findCommission = command.Adapt(findCommission);
                repoCommission.Update(findCommission);




                // Save Into Database
                unitOfWork.SaveChanges();





                return ServiceResult.Create<string>(findCommission.Id);


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