using Common;
using Helpers;
using MM.Entities;
using MM.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MM.Services.MOrder
{
    public interface IOrderService :  IServiceScoped
    {
        Task<int> Count(OrderFilter OrderFilter);
        Task<List<Order>> List(OrderFilter OrderFilter);
        Task<Order> Get(long Id);
        Task<Order> Create(Order Order);
        Task<Order> Update(Order Order);
        Task<Order> Delete(Order Order);
        Task<List<Order>> BulkDelete(List<Order> Orders);
        Task<List<Order>> Import(List<Order> Orders);
    }

    public class OrderService : IOrderService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IOrderValidator OrderValidator;

        public OrderService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IOrderValidator OrderValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.OrderValidator = OrderValidator;
        }
        public async Task<int> Count(OrderFilter OrderFilter)
        {
            try
            {
                int result = await UOW.OrderRepository.Count(OrderFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Order>> List(OrderFilter OrderFilter)
        {
            try
            {
                List<Order> Orders = await UOW.OrderRepository.List(OrderFilter);
                return Orders;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Order> Get(long Id)
        {
            Order Order = await UOW.OrderRepository.Get(Id);
            if (Order == null)
                return null;
            return Order;
        }
       
        public async Task<Order> Create(Order Order)
        {
            if (!await OrderValidator.Create(Order))
                return Order;

            try
            {
                await UOW.Begin();
                await UOW.OrderRepository.Create(Order);
                await UOW.Commit();

                await Logging.CreateAuditLog(Order, new { }, nameof(OrderService));
                return await UOW.OrderRepository.Get(Order.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Order> Update(Order Order)
        {
            if (!await OrderValidator.Update(Order))
                return Order;
            try
            {
                var oldData = await UOW.OrderRepository.Get(Order.Id);

                await UOW.Begin();
                await UOW.OrderRepository.Update(Order);
                await UOW.Commit();

                var newData = await UOW.OrderRepository.Get(Order.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(OrderService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Order> Delete(Order Order)
        {
            if (!await OrderValidator.Delete(Order))
                return Order;

            try
            {
                await UOW.Begin();
                await UOW.OrderRepository.Delete(Order);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Order, nameof(OrderService));
                return Order;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Order>> BulkDelete(List<Order> Orders)
        {
            if (!await OrderValidator.BulkDelete(Orders))
                return Orders;

            try
            {
                await UOW.Begin();
                await UOW.OrderRepository.BulkDelete(Orders);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Orders, nameof(OrderService));
                return Orders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Order>> Import(List<Order> Orders)
        {
            if (!await OrderValidator.Import(Orders))
                return Orders;
            try
            {
                await UOW.Begin();
                await UOW.OrderRepository.BulkMerge(Orders);
                await UOW.Commit();

                await Logging.CreateAuditLog(Orders, new { }, nameof(OrderService));
                return Orders;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(OrderService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
    }
}
