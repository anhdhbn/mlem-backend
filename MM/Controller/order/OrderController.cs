using Common;
using Microsoft.AspNetCore.Mvc;
using MM.Entities;
using MM.Services.MAccount;
using MM.Services.MOrder;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Controller.order
{
    public class OrderRoute : Root
    {
        public const string Master = Module + "/order/order-master";
        public const string Detail = Module + "/order/order-detail";
        private const string Default = Api + Module + "/order";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string SingleListAccount = Default + "/single-list-account";
    }

    public class OrderController : ApiController
    {
        private IAccountService AccountService;
        private IOrderService OrderService;
        private ICurrentContext CurrentContext;
        public OrderController(
            IAccountService AccountService,
            IOrderService OrderService,
            ICurrentContext CurrentContext
        )
        {
            this.AccountService = AccountService;
            this.OrderService = OrderService;
            this.CurrentContext = CurrentContext;
        }

        [Route(OrderRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Order_OrderFilterDTO Order_OrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrderFilter OrderFilter = ConvertFilterDTOToFilterEntity(Order_OrderFilterDTO);
            int count = await OrderService.Count(OrderFilter);
            return count;
        }

        [Route(OrderRoute.List), HttpPost]
        public async Task<ActionResult<List<Order_OrderDTO>>> List([FromBody] Order_OrderFilterDTO Order_OrderFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrderFilter OrderFilter = ConvertFilterDTOToFilterEntity(Order_OrderFilterDTO);
            List<Order> Orders = await OrderService.List(OrderFilter);
            List<Order_OrderDTO> Order_OrderDTOs = Orders
                .Select(c => new Order_OrderDTO(c)).ToList();
            return Order_OrderDTOs;
        }

        [Route(OrderRoute.Get), HttpPost]
        public async Task<ActionResult<Order_OrderDTO>> Get([FromBody]Order_OrderDTO Order_OrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Order Order = await OrderService.Get(Order_OrderDTO.Id);
            return new Order_OrderDTO(Order);
        }

        [Route(OrderRoute.Create), HttpPost]
        public async Task<ActionResult<Order_OrderDTO>> Create([FromBody] Order_OrderDTO Order_OrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Order Order = ConvertDTOToEntity(Order_OrderDTO);
            Order = await OrderService.Create(Order);
            Order_OrderDTO = new Order_OrderDTO(Order);
            if (Order.IsValidated)
                return Order_OrderDTO;
            else
                return BadRequest(Order_OrderDTO);
        }

        [Route(OrderRoute.Update), HttpPost]
        public async Task<ActionResult<Order_OrderDTO>> Update([FromBody] Order_OrderDTO Order_OrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Order Order = ConvertDTOToEntity(Order_OrderDTO);
            Order = await OrderService.Update(Order);
            Order_OrderDTO = new Order_OrderDTO(Order);
            if (Order.IsValidated)
                return Order_OrderDTO;
            else
                return BadRequest(Order_OrderDTO);
        }

        [Route(OrderRoute.Delete), HttpPost]
        public async Task<ActionResult<Order_OrderDTO>> Delete([FromBody] Order_OrderDTO Order_OrderDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Order Order = ConvertDTOToEntity(Order_OrderDTO);
            Order = await OrderService.Delete(Order);
            Order_OrderDTO = new Order_OrderDTO(Order);
            if (Order.IsValidated)
                return Order_OrderDTO;
            else
                return BadRequest(Order_OrderDTO);
        }
        
        private Order ConvertDTOToEntity(Order_OrderDTO Order_OrderDTO)
        {
            Order Order = new Order();
            Order.Id = Order_OrderDTO.Id;
            Order.Code = Order_OrderDTO.Code;
            Order.OrderDate = Order_OrderDTO.OrderDate;
            Order.PayDate = Order_OrderDTO.PayDate;
            Order.AccountId = Order_OrderDTO.AccountId;
            Order.NumOfTable = Order_OrderDTO.NumOfTable;
            Order.NumOfPerson = Order_OrderDTO.NumOfPerson;
            Order.Descreption = Order_OrderDTO.Descreption;
            Order.StatusId = Order_OrderDTO.StatusId;
            Order.Account = Order_OrderDTO.Account == null ? null : new Account
            {
                Id = Order_OrderDTO.Account.Id,
                DisplayName = Order_OrderDTO.Account.DisplayName,
                Email = Order_OrderDTO.Account.Email,
                Phone = Order_OrderDTO.Account.Phone,
                Password = Order_OrderDTO.Account.Password,
                Salt = Order_OrderDTO.Account.Salt,
                PasswordRecoveryCode = Order_OrderDTO.Account.PasswordRecoveryCode,
                ExpiredTimeCode = Order_OrderDTO.Account.ExpiredTimeCode,
                Address = Order_OrderDTO.Account.Address,
                Dob = Order_OrderDTO.Account.Dob,
                Avatar = Order_OrderDTO.Account.Avatar,
                RoleId = Order_OrderDTO.Account.RoleId,
            };
            Order.BaseLanguage = CurrentContext.Language;
            return Order;
        }

        private OrderFilter ConvertFilterDTOToFilterEntity(Order_OrderFilterDTO Order_OrderFilterDTO)
        {
            OrderFilter OrderFilter = new OrderFilter();
            OrderFilter.Selects = OrderSelect.ALL;
            OrderFilter.Skip = Order_OrderFilterDTO.Skip;
            OrderFilter.Take = Order_OrderFilterDTO.Take;
            OrderFilter.OrderBy = Order_OrderFilterDTO.OrderBy;
            OrderFilter.OrderType = Order_OrderFilterDTO.OrderType;

            OrderFilter.Id = Order_OrderFilterDTO.Id;
            OrderFilter.Code = Order_OrderFilterDTO.Code;
            OrderFilter.OrderDate = Order_OrderFilterDTO.OrderDate;
            OrderFilter.PayDate = Order_OrderFilterDTO.PayDate;
            OrderFilter.AccountId = Order_OrderFilterDTO.AccountId;
            OrderFilter.NumOfTable = Order_OrderFilterDTO.NumOfTable;
            OrderFilter.NumOfPerson = Order_OrderFilterDTO.NumOfPerson;
            OrderFilter.Descreption = Order_OrderFilterDTO.Descreption;
            OrderFilter.StatusId = Order_OrderFilterDTO.StatusId;
            return OrderFilter;
        }

        [Route(OrderRoute.SingleListAccount), HttpPost]
        public async Task<List<Order_AccountDTO>> SingleListAccount([FromBody] Order_AccountFilterDTO Order_AccountFilterDTO)
        {
            AccountFilter AccountFilter = new AccountFilter();
            AccountFilter.Skip = 0;
            AccountFilter.Take = 20;
            AccountFilter.OrderBy = AccountOrder.Id;
            AccountFilter.OrderType = OrderType.ASC;
            AccountFilter.Selects = AccountSelect.ALL;
            AccountFilter.Id = Order_AccountFilterDTO.Id;
            AccountFilter.DisplayName = Order_AccountFilterDTO.DisplayName;
            AccountFilter.Email = Order_AccountFilterDTO.Email;
            AccountFilter.Phone = Order_AccountFilterDTO.Phone;
            AccountFilter.Password = Order_AccountFilterDTO.Password;
            AccountFilter.Salt = Order_AccountFilterDTO.Salt;
            AccountFilter.PasswordRecoveryCode = Order_AccountFilterDTO.PasswordRecoveryCode;
            AccountFilter.ExpiredTimeCode = Order_AccountFilterDTO.ExpiredTimeCode;
            AccountFilter.Address = Order_AccountFilterDTO.Address;
            AccountFilter.Dob = Order_AccountFilterDTO.Dob;
            AccountFilter.Avatar = Order_AccountFilterDTO.Avatar;
            AccountFilter.RoleId = Order_AccountFilterDTO.RoleId;

            List<Account> Accounts = await AccountService.List(AccountFilter);
            List<Order_AccountDTO> Order_AccountDTOs = Accounts
                .Select(x => new Order_AccountDTO(x)).ToList();
            return Order_AccountDTOs;
        }

    }
}

