using Common;
using Helpers;
using MM.Entities;
using MM.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MM.Services.MFood
{
    public interface IFoodService :  IServiceScoped
    {
        Task<int> Count(FoodFilter FoodFilter);
        Task<List<Food>> List(FoodFilter FoodFilter);
        Task<Food> Get(long Id);
        Task<Food> Create(Food Food);
        Task<Food> Update(Food Food);
        Task<Food> Delete(Food Food);
        Task<List<Food>> BulkDelete(List<Food> Foods);
        Task<List<Food>> Import(List<Food> Foods);
    }

    public class FoodService : IFoodService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IFoodValidator FoodValidator;

        public FoodService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IFoodValidator FoodValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.FoodValidator = FoodValidator;
        }
        public async Task<int> Count(FoodFilter FoodFilter)
        {
            try
            {
                int result = await UOW.FoodRepository.Count(FoodFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Food>> List(FoodFilter FoodFilter)
        {
            try
            {
                List<Food> Foods = await UOW.FoodRepository.List(FoodFilter);
                return Foods;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Food> Get(long Id)
        {
            Food Food = await UOW.FoodRepository.Get(Id);
            if (Food == null)
                return null;
            return Food;
        }
       
        public async Task<Food> Create(Food Food)
        {
            if (!await FoodValidator.Create(Food))
                return Food;

            try
            {
                await UOW.Begin();
                await UOW.FoodRepository.Create(Food);
                await UOW.Commit();

                await Logging.CreateAuditLog(Food, new { }, nameof(FoodService));
                return await UOW.FoodRepository.Get(Food.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Food> Update(Food Food)
        {
            if (!await FoodValidator.Update(Food))
                return Food;
            try
            {
                var oldData = await UOW.FoodRepository.Get(Food.Id);

                await UOW.Begin();
                await UOW.FoodRepository.Update(Food);
                await UOW.Commit();

                var newData = await UOW.FoodRepository.Get(Food.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(FoodService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Food> Delete(Food Food)
        {
            if (!await FoodValidator.Delete(Food))
                return Food;

            try
            {
                await UOW.Begin();
                await UOW.FoodRepository.Delete(Food);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Food, nameof(FoodService));
                return Food;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Food>> BulkDelete(List<Food> Foods)
        {
            if (!await FoodValidator.BulkDelete(Foods))
                return Foods;

            try
            {
                await UOW.Begin();
                await UOW.FoodRepository.BulkDelete(Foods);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Foods, nameof(FoodService));
                return Foods;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Food>> Import(List<Food> Foods)
        {
            if (!await FoodValidator.Import(Foods))
                return Foods;
            try
            {
                await UOW.Begin();
                await UOW.FoodRepository.BulkMerge(Foods);
                await UOW.Commit();

                await Logging.CreateAuditLog(Foods, new { }, nameof(FoodService));
                return Foods;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FoodService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
        
    }
}
