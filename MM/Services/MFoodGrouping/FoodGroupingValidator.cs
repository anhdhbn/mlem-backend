using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MM.Entities;
using MM;
using MM.Repositories;

namespace MM.Services.MFoodGrouping
{
    public interface IFoodGroupingValidator : IServiceScoped
    {
        Task<bool> Create(FoodGrouping FoodGrouping);
        Task<bool> Update(FoodGrouping FoodGrouping);
        Task<bool> Delete(FoodGrouping FoodGrouping);
        Task<bool> BulkDelete(List<FoodGrouping> FoodGroupings);
        Task<bool> Import(List<FoodGrouping> FoodGroupings);
    }

    public class FoodGroupingValidator : IFoodGroupingValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public FoodGroupingValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(FoodGrouping FoodGrouping)
        {
            FoodGroupingFilter FoodGroupingFilter = new FoodGroupingFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = FoodGrouping.Id },
                Selects = FoodGroupingSelect.Id
            };

            int count = await UOW.FoodGroupingRepository.Count(FoodGroupingFilter);
            if (count == 0)
                FoodGrouping.AddError(nameof(FoodGroupingValidator), nameof(FoodGrouping.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(FoodGrouping FoodGrouping)
        {
            return FoodGrouping.IsValidated;
        }

        public async Task<bool> Update(FoodGrouping FoodGrouping)
        {
            if (await ValidateId(FoodGrouping))
            {
            }
            return FoodGrouping.IsValidated;
        }

        public async Task<bool> Delete(FoodGrouping FoodGrouping)
        {
            if (await ValidateId(FoodGrouping))
            {
            }
            return FoodGrouping.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<FoodGrouping> FoodGroupings)
        {
            return true;
        }
        
        public async Task<bool> Import(List<FoodGrouping> FoodGroupings)
        {
            return true;
        }
    }
}
