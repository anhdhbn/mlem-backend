using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MM.Entities;
using MM.Services.MReservation;
using MM.Services.MTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Controller.reservation
{
    public class ReservationRoute : Root
    {
        private const string Default = Api + Module + "/reservation";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string SingleListTable = Default + "/single-list-table";
    }

    public class ReservationController : ApiController
    {
        private ITableService TableService;
        private IReservationService ReservationService;
        private ICurrentContext CurrentContext;
        public ReservationController(
            ITableService TableService,
            IReservationService ReservationService,
            ICurrentContext CurrentContext
        )
        {
            this.TableService = TableService;
            this.ReservationService = ReservationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ReservationRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Reservation_ReservationFilterDTO Reservation_ReservationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ReservationFilter ReservationFilter = ConvertFilterDTOToFilterEntity(Reservation_ReservationFilterDTO);
            int count = await ReservationService.Count(ReservationFilter);
            return count;
        }

        [Route(ReservationRoute.List), HttpPost]
        public async Task<ActionResult<List<Reservation_ReservationDTO>>> List([FromBody] Reservation_ReservationFilterDTO Reservation_ReservationFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ReservationFilter ReservationFilter = ConvertFilterDTOToFilterEntity(Reservation_ReservationFilterDTO);
            List<Reservation> Reservations = await ReservationService.List(ReservationFilter);
            List<Reservation_ReservationDTO> Reservation_ReservationDTOs = Reservations
                .Select(c => new Reservation_ReservationDTO(c)).ToList();
            return Reservation_ReservationDTOs;
        }

        [Route(ReservationRoute.Get), HttpPost]
        public async Task<ActionResult<Reservation_ReservationDTO>> Get([FromBody]Reservation_ReservationDTO Reservation_ReservationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Reservation Reservation = await ReservationService.Get(Reservation_ReservationDTO.Id);
            return new Reservation_ReservationDTO(Reservation);
        }

        [Route(ReservationRoute.Create), HttpPost]
        public async Task<ActionResult<Reservation_ReservationDTO>> Create([FromBody] Reservation_ReservationDTO Reservation_ReservationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Reservation Reservation = ConvertDTOToEntity(Reservation_ReservationDTO);
            Reservation = await ReservationService.Create(Reservation);
            Reservation_ReservationDTO = new Reservation_ReservationDTO(Reservation);
            if (Reservation.IsValidated)
                return Reservation_ReservationDTO;
            else
                return BadRequest(Reservation_ReservationDTO);
        }

        [Route(ReservationRoute.Update), HttpPost]
        public async Task<ActionResult<Reservation_ReservationDTO>> Update([FromBody] Reservation_ReservationDTO Reservation_ReservationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Reservation Reservation = ConvertDTOToEntity(Reservation_ReservationDTO);
            Reservation = await ReservationService.Update(Reservation);
            Reservation_ReservationDTO = new Reservation_ReservationDTO(Reservation);
            if (Reservation.IsValidated)
                return Reservation_ReservationDTO;
            else
                return BadRequest(Reservation_ReservationDTO);
        }

        [Route(ReservationRoute.Delete), HttpPost]
        public async Task<ActionResult<Reservation_ReservationDTO>> Delete([FromBody] Reservation_ReservationDTO Reservation_ReservationDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Reservation Reservation = ConvertDTOToEntity(Reservation_ReservationDTO);
            Reservation = await ReservationService.Delete(Reservation);
            Reservation_ReservationDTO = new Reservation_ReservationDTO(Reservation);
            if (Reservation.IsValidated)
                return Reservation_ReservationDTO;
            else
                return BadRequest(Reservation_ReservationDTO);
        }
        
        private Reservation ConvertDTOToEntity(Reservation_ReservationDTO Reservation_ReservationDTO)
        {
            Reservation Reservation = new Reservation();
            Reservation.Id = Reservation_ReservationDTO.Id;
            Reservation.TableId = Reservation_ReservationDTO.TableId;
            Reservation.Date = Reservation_ReservationDTO.Date;
            Reservation.StatusId = Reservation_ReservationDTO.StatusId;
            Reservation.Table = Reservation_ReservationDTO.Table == null ? null : new Table
            {
                Id = Reservation_ReservationDTO.Table.Id,
                Code = Reservation_ReservationDTO.Table.Code,
            };
            Reservation.BaseLanguage = CurrentContext.Language;
            return Reservation;
        }

        private ReservationFilter ConvertFilterDTOToFilterEntity(Reservation_ReservationFilterDTO Reservation_ReservationFilterDTO)
        {
            ReservationFilter ReservationFilter = new ReservationFilter();
            ReservationFilter.Selects = ReservationSelect.ALL;
            ReservationFilter.Skip = Reservation_ReservationFilterDTO.Skip;
            ReservationFilter.Take = Reservation_ReservationFilterDTO.Take;
            ReservationFilter.OrderBy = Reservation_ReservationFilterDTO.OrderBy;
            ReservationFilter.OrderType = Reservation_ReservationFilterDTO.OrderType;

            ReservationFilter.Id = Reservation_ReservationFilterDTO.Id;
            ReservationFilter.TableId = Reservation_ReservationFilterDTO.TableId;
            ReservationFilter.Date = Reservation_ReservationFilterDTO.Date;
            ReservationFilter.StatusId = Reservation_ReservationFilterDTO.StatusId;
            return ReservationFilter;
        }

        [Route(ReservationRoute.SingleListTable), HttpPost]
        public async Task<List<Reservation_TableDTO>> SingleListTable([FromBody] Reservation_TableFilterDTO Reservation_TableFilterDTO)
        {
            TableFilter TableFilter = new TableFilter();
            TableFilter.Skip = 0;
            TableFilter.Take = 20;
            TableFilter.OrderBy = TableOrder.Id;
            TableFilter.OrderType = OrderType.ASC;
            TableFilter.Selects = TableSelect.ALL;
            TableFilter.Id = Reservation_TableFilterDTO.Id;
            TableFilter.Code = Reservation_TableFilterDTO.Code;

            List<Table> Tables = await TableService.List(TableFilter);
            List<Reservation_TableDTO> Reservation_TableDTOs = Tables
                .Select(x => new Reservation_TableDTO(x)).ToList();
            return Reservation_TableDTOs;
        }

    }
}

