using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MM.Entities;
using MM.Services.MOrder;
using MM.Services.MTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Rpc.table
{
    public class TableRoute : Root
    {
        public const string Master = Module + "/table/table-master";
        public const string Detail = Module + "/table/table-detail";
        private const string Default = Api + Module + "/table";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string SingleListOrder = Default + "/single-list-order";
    }

    public class TableController : RpcController
    {
        private IOrderService OrderService;
        private ITableService TableService;
        private ICurrentContext CurrentContext;
        public TableController(
            IOrderService OrderService,
            ITableService TableService,
            ICurrentContext CurrentContext
        )
        {
            this.OrderService = OrderService;
            this.TableService = TableService;
            this.CurrentContext = CurrentContext;
        }

        [Route(TableRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Table_TableFilterDTO Table_TableFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TableFilter TableFilter = ConvertFilterDTOToFilterEntity(Table_TableFilterDTO);
            int count = await TableService.Count(TableFilter);
            return count;
        }

        [Route(TableRoute.List), HttpPost]
        public async Task<ActionResult<List<Table_TableDTO>>> List([FromBody] Table_TableFilterDTO Table_TableFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            TableFilter TableFilter = ConvertFilterDTOToFilterEntity(Table_TableFilterDTO);
            List<Table> Tables = await TableService.List(TableFilter);
            List<Table_TableDTO> Table_TableDTOs = Tables
                .Select(c => new Table_TableDTO(c)).ToList();
            return Table_TableDTOs;
        }

        [Route(TableRoute.Get), HttpPost]
        public async Task<ActionResult<Table_TableDTO>> Get([FromBody]Table_TableDTO Table_TableDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Table Table = await TableService.Get(Table_TableDTO.Id);
            return new Table_TableDTO(Table);
        }

        [Route(TableRoute.Create), HttpPost]
        public async Task<ActionResult<Table_TableDTO>> Create([FromBody] Table_TableDTO Table_TableDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Table Table = ConvertDTOToEntity(Table_TableDTO);
            Table = await TableService.Create(Table);
            Table_TableDTO = new Table_TableDTO(Table);
            if (Table.IsValidated)
                return Table_TableDTO;
            else
                return BadRequest(Table_TableDTO);
        }

        [Route(TableRoute.Update), HttpPost]
        public async Task<ActionResult<Table_TableDTO>> Update([FromBody] Table_TableDTO Table_TableDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Table Table = ConvertDTOToEntity(Table_TableDTO);
            Table = await TableService.Update(Table);
            Table_TableDTO = new Table_TableDTO(Table);
            if (Table.IsValidated)
                return Table_TableDTO;
            else
                return BadRequest(Table_TableDTO);
        }

        [Route(TableRoute.Delete), HttpPost]
        public async Task<ActionResult<Table_TableDTO>> Delete([FromBody] Table_TableDTO Table_TableDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Table Table = ConvertDTOToEntity(Table_TableDTO);
            Table = await TableService.Delete(Table);
            Table_TableDTO = new Table_TableDTO(Table);
            if (Table.IsValidated)
                return Table_TableDTO;
            else
                return BadRequest(Table_TableDTO);
        }
        
        private Table ConvertDTOToEntity(Table_TableDTO Table_TableDTO)
        {
            Table Table = new Table();
            Table.Id = Table_TableDTO.Id;
            Table.Code = Table_TableDTO.Code;
            Table.StatusId = Table_TableDTO.StatusId;
            Table.OrderId = Table_TableDTO.OrderId;
            Table.Order = Table_TableDTO.Order == null ? null : new Order
            {
                Id = Table_TableDTO.Order.Id,
                Code = Table_TableDTO.Order.Code,
                OrderDate = Table_TableDTO.Order.OrderDate,
                PayDate = Table_TableDTO.Order.PayDate,
                AccountId = Table_TableDTO.Order.AccountId,
                NumOfTable = Table_TableDTO.Order.NumOfTable,
                NumOfPerson = Table_TableDTO.Order.NumOfPerson,
                Descreption = Table_TableDTO.Order.Descreption,
                StatusId = Table_TableDTO.Order.StatusId,
            };
            Table.BaseLanguage = CurrentContext.Language;
            return Table;
        }

        private TableFilter ConvertFilterDTOToFilterEntity(Table_TableFilterDTO Table_TableFilterDTO)
        {
            TableFilter TableFilter = new TableFilter();
            TableFilter.Selects = TableSelect.ALL;
            TableFilter.Skip = Table_TableFilterDTO.Skip;
            TableFilter.Take = Table_TableFilterDTO.Take;
            TableFilter.OrderBy = Table_TableFilterDTO.OrderBy;
            TableFilter.OrderType = Table_TableFilterDTO.OrderType;

            TableFilter.Id = Table_TableFilterDTO.Id;
            TableFilter.Code = Table_TableFilterDTO.Code;
            TableFilter.StatusId = Table_TableFilterDTO.StatusId;
            TableFilter.OrderId = Table_TableFilterDTO.OrderId;
            return TableFilter;
        }

        [Route(TableRoute.SingleListOrder), HttpPost]
        public async Task<List<Table_OrderDTO>> SingleListOrder([FromBody] Table_OrderFilterDTO Table_OrderFilterDTO)
        {
            OrderFilter OrderFilter = new OrderFilter();
            OrderFilter.Skip = 0;
            OrderFilter.Take = 20;
            OrderFilter.OrderBy = OrderOrder.Id;
            OrderFilter.OrderType = OrderType.ASC;
            OrderFilter.Selects = OrderSelect.ALL;
            OrderFilter.Id = Table_OrderFilterDTO.Id;
            OrderFilter.Code = Table_OrderFilterDTO.Code;
            OrderFilter.OrderDate = Table_OrderFilterDTO.OrderDate;
            OrderFilter.PayDate = Table_OrderFilterDTO.PayDate;
            OrderFilter.AccountId = Table_OrderFilterDTO.AccountId;
            OrderFilter.NumOfTable = Table_OrderFilterDTO.NumOfTable;
            OrderFilter.NumOfPerson = Table_OrderFilterDTO.NumOfPerson;
            OrderFilter.Descreption = Table_OrderFilterDTO.Descreption;
            OrderFilter.StatusId = Table_OrderFilterDTO.StatusId;

            List<Order> Orders = await OrderService.List(OrderFilter);
            List<Table_OrderDTO> Table_OrderDTOs = Orders
                .Select(x => new Table_OrderDTO(x)).ToList();
            return Table_OrderDTOs;
        }

    }
}

