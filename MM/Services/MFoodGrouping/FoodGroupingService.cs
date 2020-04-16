using Common;
using Helpers;
using MM.Entities;
using MM.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MM.Services.MFoodGrouping
{
    public interface IFoodGroupingService :  IServiceScoped
    {
        Task<int> Count(FoodGroupingFilter FoodGroupingFilter);
        Task<List<FoodGrouping>> List(FoodGroupingFilter FoodGroupingFilter);
        Task<FoodGrouping> Get(long Id);
        Task<FoodGrouping> Create(FoodGrouping FoodGrouping);
        Task<FoodGrouping> Update(FoodGrouping FoodGrouping);
        Task<FoodGrouping> Delete(FoodGrouping FoodGrouping);
        Task<List<FoodGrouping>> BulkDelete(List<FoodGrouping> FoodGroupings);
        Task<List<FoodGrouping>> Import(List<FoodGrouping> FoodGroupings);
    }

    public class FoodGroupingService : IFoodGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IFoodGroupingValidator FoodGroupingValidator;

        public FoodGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IFoodGroupingValidator FoodGroupingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.FoodGroupingValidator = FoodGroupingValidator;
        }
        public async Task<int> Count(FoodGroupingFilter FoodGroupingFilter)
        {
            try
            {
                int result = await UOW.FoodGroupingRepository.Count(FoodGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<FoodGrouping>> List(FoodGroupingFilter FoodGroupingFilter)
        {
            try
            {
                List<FoodGrouping> FoodGroupings = await UOW.FoodGroupingRepository.List(FoodGroupingFilter);
                return FoodGroupings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<FoodGrouping> Get(long Id)
        {
            FoodGrouping FoodGrouping = await UOW.FoodGroupingRepository.Get(Id);
            if (FoodGrouping == null)
                return null;
            return FoodGrouping;
        }
       
        public async Task<FoodGrouping> Create(FoodGrouping FoodGrouping)
        {
            if (!await FoodGroupingValidator.Create(FoodGrouping))
                return FoodGrouping;

            try
            {
                await UOW.Begin();
                await UOW.FoodGroupingRepository.Create(FoodGrouping);
                await UOW.Commit();

                await Logging.CreateAuditLog(FoodGrouping, new { }, nameof(FoodGroupingService));
                return await UOW.FoodGroupingRepository.Get(FoodGrouping.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<FoodGrouping> Update(FoodGrouping FoodGrouping)
        {
            if (!await FoodGroupingValidator.Update(FoodGrouping))
                return FoodGrouping;
            try
            {
                var oldData = await UOW.FoodGroupingRepository.Get(FoodGrouping.Id);

                await UOW.Begin();
                await UOW.FoodGroupingRepository.Update(FoodGrouping);
                await UOW.Commit();

                var newData = await UOW.FoodGroupingRepository.Get(FoodGrouping.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(FoodGroupingService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<FoodGrouping> Delete(FoodGrouping FoodGrouping)
        {
            if (!await FoodGroupingValidator.Delete(FoodGrouping))
                return FoodGrouping;

            try
            {
                await UOW.Begin();
                await UOW.FoodGroupingRepository.Delete(FoodGrouping);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, FoodGrouping, nameof(FoodGroupingService));
                return FoodGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<FoodGrouping>> BulkDelete(List<FoodGrouping> FoodGroupings)
        {
            if (!await FoodGroupingValidator.BulkDelete(FoodGroupings))
                return FoodGroupings;

            try
            {
                await UOW.Begin();
                await UOW.FoodGroupingRepository.BulkDelete(FoodGroupings);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, FoodGroupings, nameof(FoodGroupingService));
                return FoodGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<FoodGrouping>> Import(List<FoodGrouping> FoodGroupings)
        {
            if (!await FoodGroupingValidator.Import(FoodGroupings))
                return FoodGroupings;
            try
            {
                await UOW.Begin();
                await UOW.FoodGroupingRepository.BulkMerge(FoodGroupings);
                await UOW.Commit();

                await Logging.CreateAuditLog(FoodGroupings, new { }, nameof(FoodGroupingService));
                return FoodGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
    }
}
