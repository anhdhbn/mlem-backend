using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MM.Entities;
using MM;
using MM.Repositories;

namespace MM.Services.MFoodType
{
    public interface IFoodTypeValidator : IServiceScoped
    {
        Task<bool> Create(FoodType FoodType);
        Task<bool> Update(FoodType FoodType);
        Task<bool> Delete(FoodType FoodType);
        Task<bool> BulkDelete(List<FoodType> FoodTypes);
        Task<bool> Import(List<FoodType> FoodTypes);
    }

    public class FoodTypeValidator : IFoodTypeValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public FoodTypeValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(FoodType FoodType)
        {
            FoodTypeFilter FoodTypeFilter = new FoodTypeFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = FoodType.Id },
                Selects = FoodTypeSelect.Id
            };

            int count = await UOW.FoodTypeRepository.Count(FoodTypeFilter);
            if (count == 0)
                FoodType.AddError(nameof(FoodTypeValidator), nameof(FoodType.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(FoodType FoodType)
        {
            return FoodType.IsValidated;
        }

        public async Task<bool> Update(FoodType FoodType)
        {
            if (await ValidateId(FoodType))
            {
            }
            return FoodType.IsValidated;
        }

        public async Task<bool> Delete(FoodType FoodType)
        {
            if (await ValidateId(FoodType))
            {
            }
            return FoodType.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<FoodType> FoodTypes)
        {
            return true;
        }
        
        public async Task<bool> Import(List<FoodType> FoodTypes)
        {
            return true;
        }
    }
}
