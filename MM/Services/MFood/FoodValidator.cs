using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MM.Entities;
using MM;
using MM.Repositories;

namespace MM.Services.MFood
{
    public interface IFoodValidator : IServiceScoped
    {
        Task<bool> Create(Food Food);
        Task<bool> Update(Food Food);
        Task<bool> Delete(Food Food);
        Task<bool> BulkDelete(List<Food> Foods);
        Task<bool> Import(List<Food> Foods);
    }

    public class FoodValidator : IFoodValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public FoodValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Food Food)
        {
            FoodFilter FoodFilter = new FoodFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Food.Id },
                Selects = FoodSelect.Id
            };

            int count = await UOW.FoodRepository.Count(FoodFilter);
            if (count == 0)
                Food.AddError(nameof(FoodValidator), nameof(Food.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Food Food)
        {
            return Food.IsValidated;
        }

        public async Task<bool> Update(Food Food)
        {
            if (await ValidateId(Food))
            {
            }
            return Food.IsValidated;
        }

        public async Task<bool> Delete(Food Food)
        {
            if (await ValidateId(Food))
            {
            }
            return Food.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Food> Foods)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Food> Foods)
        {
            return true;
        }
    }
}
