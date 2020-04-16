using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MM.Entities;
using MM;
using MM.Repositories;

namespace MM.Services.MOrderContent
{
    public interface IOrderContentValidator : IServiceScoped
    {
        Task<bool> Create(OrderContent OrderContent);
        Task<bool> Update(OrderContent OrderContent);
        Task<bool> Delete(OrderContent OrderContent);
        Task<bool> BulkDelete(List<OrderContent> OrderContents);
        Task<bool> Import(List<OrderContent> OrderContents);
    }

    public class OrderContentValidator : IOrderContentValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public OrderContentValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(OrderContent OrderContent)
        {
            OrderContentFilter OrderContentFilter = new OrderContentFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = OrderContent.Id },
                Selects = OrderContentSelect.Id
            };

            int count = await UOW.OrderContentRepository.Count(OrderContentFilter);
            if (count == 0)
                OrderContent.AddError(nameof(OrderContentValidator), nameof(OrderContent.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(OrderContent OrderContent)
        {
            return OrderContent.IsValidated;
        }

        public async Task<bool> Update(OrderContent OrderContent)
        {
            if (await ValidateId(OrderContent))
            {
            }
            return OrderContent.IsValidated;
        }

        public async Task<bool> Delete(OrderContent OrderContent)
        {
            if (await ValidateId(OrderContent))
            {
            }
            return OrderContent.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<OrderContent> OrderContents)
        {
            return true;
        }
        
        public async Task<bool> Import(List<OrderContent> OrderContents)
        {
            return true;
        }
    }
}
