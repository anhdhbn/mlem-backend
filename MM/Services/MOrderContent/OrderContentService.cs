using Common;
using Helpers;
using MM.Entities;
using MM.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MM.Services.MOrderContent
{
    public interface IOrderContentService :  IServiceScoped
    {
        Task<int> Count(OrderContentFilter OrderContentFilter);
        Task<List<OrderContent>> List(OrderContentFilter OrderContentFilter);
        Task<OrderContent> Get(long Id);
        Task<OrderContent> Create(OrderContent OrderContent);
        Task<OrderContent> Update(OrderContent OrderContent);
        Task<OrderContent> Delete(OrderContent OrderContent);
        Task<List<OrderContent>> BulkDelete(List<OrderContent> OrderContents);
        Task<List<OrderContent>> Import(List<OrderContent> OrderContents);
    }

    public class OrderContentService : IOrderContentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IOrderContentValidator OrderContentValidator;

        public OrderContentService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IOrderContentValidator OrderContentValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.OrderContentValidator = OrderContentValidator;
        }
        public async Task<int> Count(OrderContentFilter OrderContentFilter)
        {
            try
            {
                int result = await UOW.OrderContentRepository.Count(OrderContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<OrderContent>> List(OrderContentFilter OrderContentFilter)
        {
            try
            {
                List<OrderContent> OrderContents = await UOW.OrderContentRepository.List(OrderContentFilter);
                return OrderContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<OrderContent> Get(long Id)
        {
            OrderContent OrderContent = await UOW.OrderContentRepository.Get(Id);
            if (OrderContent == null)
                return null;
            return OrderContent;
        }
       
        public async Task<OrderContent> Create(OrderContent OrderContent)
        {
            if (!await OrderContentValidator.Create(OrderContent))
                return OrderContent;

            try
            {
                await UOW.Begin();
                await UOW.OrderContentRepository.Create(OrderContent);
                await UOW.Commit();

                await Logging.CreateAuditLog(OrderContent, new { }, nameof(OrderContentService));
                return await UOW.OrderContentRepository.Get(OrderContent.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<OrderContent> Update(OrderContent OrderContent)
        {
            if (!await OrderContentValidator.Update(OrderContent))
                return OrderContent;
            try
            {
                var oldData = await UOW.OrderContentRepository.Get(OrderContent.Id);

                await UOW.Begin();
                await UOW.OrderContentRepository.Update(OrderContent);
                await UOW.Commit();

                var newData = await UOW.OrderContentRepository.Get(OrderContent.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(OrderContentService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<OrderContent> Delete(OrderContent OrderContent)
        {
            if (!await OrderContentValidator.Delete(OrderContent))
                return OrderContent;

            try
            {
                await UOW.Begin();
                await UOW.OrderContentRepository.Delete(OrderContent);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, OrderContent, nameof(OrderContentService));
                return OrderContent;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<OrderContent>> BulkDelete(List<OrderContent> OrderContents)
        {
            if (!await OrderContentValidator.BulkDelete(OrderContents))
                return OrderContents;

            try
            {
                await UOW.Begin();
                await UOW.OrderContentRepository.BulkDelete(OrderContents);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, OrderContents, nameof(OrderContentService));
                return OrderContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<OrderContent>> Import(List<OrderContent> OrderContents)
        {
            if (!await OrderContentValidator.Import(OrderContents))
                return OrderContents;
            try
            {
                await UOW.Begin();
                await UOW.OrderContentRepository.BulkMerge(OrderContents);
                await UOW.Commit();

                await Logging.CreateAuditLog(OrderContents, new { }, nameof(OrderContentService));
                return OrderContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
    }
}
