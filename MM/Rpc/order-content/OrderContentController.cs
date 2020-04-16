using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MM.Entities;
using MM.Services.MOrder;
using MM.Services.MOrderContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Rpc.order_content
{
    public class OrderContentRoute : Root
    {
        public const string Master = Module + "/order-content/order-content-master";
        public const string Detail = Module + "/order-content/order-content-detail";
        private const string Default = Api + Module + "/order-content";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string SingleListOrder = Default + "/single-list-order";
    }

    public class OrderContentController : RpcController
    {
        private IOrderService OrderService;
        private IOrderContentService OrderContentService;
        private ICurrentContext CurrentContext;
        public OrderContentController(
            IOrderService OrderService,
            IOrderContentService OrderContentService,
            ICurrentContext CurrentContext
        )
        {
            this.OrderService = OrderService;
            this.OrderContentService = OrderContentService;
            this.CurrentContext = CurrentContext;
        }

        [Route(OrderContentRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] OrderContent_OrderContentFilterDTO OrderContent_OrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrderContentFilter OrderContentFilter = ConvertFilterDTOToFilterEntity(OrderContent_OrderContentFilterDTO);
            int count = await OrderContentService.Count(OrderContentFilter);
            return count;
        }

        [Route(OrderContentRoute.List), HttpPost]
        public async Task<ActionResult<List<OrderContent_OrderContentDTO>>> List([FromBody] OrderContent_OrderContentFilterDTO OrderContent_OrderContentFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrderContentFilter OrderContentFilter = ConvertFilterDTOToFilterEntity(OrderContent_OrderContentFilterDTO);
            List<OrderContent> OrderContents = await OrderContentService.List(OrderContentFilter);
            List<OrderContent_OrderContentDTO> OrderContent_OrderContentDTOs = OrderContents
                .Select(c => new OrderContent_OrderContentDTO(c)).ToList();
            return OrderContent_OrderContentDTOs;
        }

        [Route(OrderContentRoute.Get), HttpPost]
        public async Task<ActionResult<OrderContent_OrderContentDTO>> Get([FromBody]OrderContent_OrderContentDTO OrderContent_OrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrderContent OrderContent = await OrderContentService.Get(OrderContent_OrderContentDTO.Id);
            return new OrderContent_OrderContentDTO(OrderContent);
        }

        [Route(OrderContentRoute.Create), HttpPost]
        public async Task<ActionResult<OrderContent_OrderContentDTO>> Create([FromBody] OrderContent_OrderContentDTO OrderContent_OrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            OrderContent OrderContent = ConvertDTOToEntity(OrderContent_OrderContentDTO);
            OrderContent = await OrderContentService.Create(OrderContent);
            OrderContent_OrderContentDTO = new OrderContent_OrderContentDTO(OrderContent);
            if (OrderContent.IsValidated)
                return OrderContent_OrderContentDTO;
            else
                return BadRequest(OrderContent_OrderContentDTO);
        }

        [Route(OrderContentRoute.Update), HttpPost]
        public async Task<ActionResult<OrderContent_OrderContentDTO>> Update([FromBody] OrderContent_OrderContentDTO OrderContent_OrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            OrderContent OrderContent = ConvertDTOToEntity(OrderContent_OrderContentDTO);
            OrderContent = await OrderContentService.Update(OrderContent);
            OrderContent_OrderContentDTO = new OrderContent_OrderContentDTO(OrderContent);
            if (OrderContent.IsValidated)
                return OrderContent_OrderContentDTO;
            else
                return BadRequest(OrderContent_OrderContentDTO);
        }

        [Route(OrderContentRoute.Delete), HttpPost]
        public async Task<ActionResult<OrderContent_OrderContentDTO>> Delete([FromBody] OrderContent_OrderContentDTO OrderContent_OrderContentDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            OrderContent OrderContent = ConvertDTOToEntity(OrderContent_OrderContentDTO);
            OrderContent = await OrderContentService.Delete(OrderContent);
            OrderContent_OrderContentDTO = new OrderContent_OrderContentDTO(OrderContent);
            if (OrderContent.IsValidated)
                return OrderContent_OrderContentDTO;
            else
                return BadRequest(OrderContent_OrderContentDTO);
        }
        
        private OrderContent ConvertDTOToEntity(OrderContent_OrderContentDTO OrderContent_OrderContentDTO)
        {
            OrderContent OrderContent = new OrderContent();
            OrderContent.Id = OrderContent_OrderContentDTO.Id;
            OrderContent.Code = OrderContent_OrderContentDTO.Code;
            OrderContent.OrderId = OrderContent_OrderContentDTO.OrderId;
            OrderContent.FoodFoodTypeMappingId = OrderContent_OrderContentDTO.FoodFoodTypeMappingId;
            OrderContent.Quantity = OrderContent_OrderContentDTO.Quantity;
            OrderContent.StatusId = OrderContent_OrderContentDTO.StatusId;
            OrderContent.FoodFoodTypeMapping = OrderContent_OrderContentDTO.FoodFoodTypeMapping == null ? null : new FoodFoodTypeMapping
            {
                Id = OrderContent_OrderContentDTO.FoodFoodTypeMapping.Id,
                FoodId = OrderContent_OrderContentDTO.FoodFoodTypeMapping.FoodId,
                FoodTypeId = OrderContent_OrderContentDTO.FoodFoodTypeMapping.FoodTypeId,
            };
            OrderContent.Order = OrderContent_OrderContentDTO.Order == null ? null : new Order
            {
                Id = OrderContent_OrderContentDTO.Order.Id,
                Code = OrderContent_OrderContentDTO.Order.Code,
                OrderDate = OrderContent_OrderContentDTO.Order.OrderDate,
                PayDate = OrderContent_OrderContentDTO.Order.PayDate,
                AccountId = OrderContent_OrderContentDTO.Order.AccountId,
                NumOfTable = OrderContent_OrderContentDTO.Order.NumOfTable,
                NumOfPerson = OrderContent_OrderContentDTO.Order.NumOfPerson,
                Descreption = OrderContent_OrderContentDTO.Order.Descreption,
                StatusId = OrderContent_OrderContentDTO.Order.StatusId,
            };
            OrderContent.BaseLanguage = CurrentContext.Language;
            return OrderContent;
        }

        private OrderContentFilter ConvertFilterDTOToFilterEntity(OrderContent_OrderContentFilterDTO OrderContent_OrderContentFilterDTO)
        {
            OrderContentFilter OrderContentFilter = new OrderContentFilter();
            OrderContentFilter.Selects = OrderContentSelect.ALL;
            OrderContentFilter.Skip = OrderContent_OrderContentFilterDTO.Skip;
            OrderContentFilter.Take = OrderContent_OrderContentFilterDTO.Take;
            OrderContentFilter.OrderBy = OrderContent_OrderContentFilterDTO.OrderBy;
            OrderContentFilter.OrderType = OrderContent_OrderContentFilterDTO.OrderType;

            OrderContentFilter.Id = OrderContent_OrderContentFilterDTO.Id;
            OrderContentFilter.Code = OrderContent_OrderContentFilterDTO.Code;
            OrderContentFilter.OrderId = OrderContent_OrderContentFilterDTO.OrderId;
            OrderContentFilter.FoodFoodTypeMappingId = OrderContent_OrderContentFilterDTO.FoodFoodTypeMappingId;
            OrderContentFilter.Quantity = OrderContent_OrderContentFilterDTO.Quantity;
            OrderContentFilter.StatusId = OrderContent_OrderContentFilterDTO.StatusId;
            return OrderContentFilter;
        }

        [Route(OrderContentRoute.SingleListOrder), HttpPost]
        public async Task<List<OrderContent_OrderDTO>> SingleListOrder([FromBody] OrderContent_OrderFilterDTO OrderContent_OrderFilterDTO)
        {
            OrderFilter OrderFilter = new OrderFilter();
            OrderFilter.Skip = 0;
            OrderFilter.Take = 20;
            OrderFilter.OrderBy = OrderOrder.Id;
            OrderFilter.OrderType = OrderType.ASC;
            OrderFilter.Selects = OrderSelect.ALL;
            OrderFilter.Id = OrderContent_OrderFilterDTO.Id;
            OrderFilter.Code = OrderContent_OrderFilterDTO.Code;
            OrderFilter.OrderDate = OrderContent_OrderFilterDTO.OrderDate;
            OrderFilter.PayDate = OrderContent_OrderFilterDTO.PayDate;
            OrderFilter.AccountId = OrderContent_OrderFilterDTO.AccountId;
            OrderFilter.NumOfTable = OrderContent_OrderFilterDTO.NumOfTable;
            OrderFilter.NumOfPerson = OrderContent_OrderFilterDTO.NumOfPerson;
            OrderFilter.Descreption = OrderContent_OrderFilterDTO.Descreption;
            OrderFilter.StatusId = OrderContent_OrderFilterDTO.StatusId;

            List<Order> Orders = await OrderService.List(OrderFilter);
            List<OrderContent_OrderDTO> OrderContent_OrderDTOs = Orders
                .Select(x => new OrderContent_OrderDTO(x)).ToList();
            return OrderContent_OrderDTOs;
        }

    }
}

