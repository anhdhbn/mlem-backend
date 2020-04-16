using Common;
using Helpers;
using MM.Entities;
using MM.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MM.Services.MFoodType
{
    public interface IFoodTypeService :  IServiceScoped
    {
        Task<int> Count(FoodTypeFilter FoodTypeFilter);
        Task<List<FoodType>> List(FoodTypeFilter FoodTypeFilter);
        Task<FoodType> Get(long Id);
        Task<FoodType> Create(FoodType FoodType);
        Task<FoodType> Update(FoodType FoodType);
        Task<FoodType> Delete(FoodType FoodType);
        Task<List<FoodType>> BulkDelete(List<FoodType> FoodTypes);
        Task<List<FoodType>> Import(List<FoodType> FoodTypes);
    }

    public class FoodTypeService : IFoodTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IFoodTypeValidator FoodTypeValidator;

        public FoodTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IFoodTypeValidator FoodTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.FoodTypeValidator = FoodTypeValidator;
        }
        public async Task<int> Count(FoodTypeFilter FoodTypeFilter)
        {
            try
            {
                int result = await UOW.FoodTypeRepository.Count(FoodTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<FoodType>> List(FoodTypeFilter FoodTypeFilter)
        {
            try
            {
                List<FoodType> FoodTypes = await UOW.FoodTypeRepository.List(FoodTypeFilter);
                return FoodTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<FoodType> Get(long Id)
        {
            FoodType FoodType = await UOW.FoodTypeRepository.Get(Id);
            if (FoodType == null)
                return null;
            return FoodType;
        }
       
        public async Task<FoodType> Create(FoodType FoodType)
        {
            if (!await FoodTypeValidator.Create(FoodType))
                return FoodType;

            try
            {
                await UOW.Begin();
                await UOW.FoodTypeRepository.Create(FoodType);
                await UOW.Commit();

                await Logging.CreateAuditLog(FoodType, new { }, nameof(FoodTypeService));
                return await UOW.FoodTypeRepository.Get(FoodType.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<FoodType> Update(FoodType FoodType)
        {
            if (!await FoodTypeValidator.Update(FoodType))
                return FoodType;
            try
            {
                var oldData = await UOW.FoodTypeRepository.Get(FoodType.Id);

                await UOW.Begin();
                await UOW.FoodTypeRepository.Update(FoodType);
                await UOW.Commit();

                var newData = await UOW.FoodTypeRepository.Get(FoodType.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(FoodTypeService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<FoodType> Delete(FoodType FoodType)
        {
            if (!await FoodTypeValidator.Delete(FoodType))
                return FoodType;

            try
            {
                await UOW.Begin();
                await UOW.FoodTypeRepository.Delete(FoodType);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, FoodType, nameof(FoodTypeService));
                return FoodType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<FoodType>> BulkDelete(List<FoodType> FoodTypes)
        {
            if (!await FoodTypeValidator.BulkDelete(FoodTypes))
                return FoodTypes;

            try
            {
                await UOW.Begin();
                await UOW.FoodTypeRepository.BulkDelete(FoodTypes);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, FoodTypes, nameof(FoodTypeService));
                return FoodTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<FoodType>> Import(List<FoodType> FoodTypes)
        {
            if (!await FoodTypeValidator.Import(FoodTypes))
                return FoodTypes;
            try
            {
                await UOW.Begin();
                await UOW.FoodTypeRepository.BulkMerge(FoodTypes);
                await UOW.Commit();

                await Logging.CreateAuditLog(FoodTypes, new { }, nameof(FoodTypeService));
                return FoodTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
    }
}
