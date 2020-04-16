using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MM.Entities;
using MM.Services.MAccount;
using MM.Services.MNotificaiton;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MM.Rpc.notificaiton
{
    public class NotificaitonRoute : Root
    {
        public const string Master = Module + "/notificaiton/notificaiton-master";
        public const string Detail = Module + "/notificaiton/notificaiton-detail";
        private const string Default = Api + Module + "/notificaiton";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string SingleListAccount = Default + "/single-list-account";
    }

    public class NotificaitonController : RpcController
    {
        private IAccountService AccountService;
        private INotificaitonService NotificaitonService;
        private ICurrentContext CurrentContext;
        public NotificaitonController(
            IAccountService AccountService,
            INotificaitonService NotificaitonService,
            ICurrentContext CurrentContext
        )
        {
            this.AccountService = AccountService;
            this.NotificaitonService = NotificaitonService;
            this.CurrentContext = CurrentContext;
        }

        [Route(NotificaitonRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Notificaiton_NotificaitonFilterDTO Notificaiton_NotificaitonFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NotificaitonFilter NotificaitonFilter = ConvertFilterDTOToFilterEntity(Notificaiton_NotificaitonFilterDTO);
            int count = await NotificaitonService.Count(NotificaitonFilter);
            return count;
        }

        [Route(NotificaitonRoute.List), HttpPost]
        public async Task<ActionResult<List<Notificaiton_NotificaitonDTO>>> List([FromBody] Notificaiton_NotificaitonFilterDTO Notificaiton_NotificaitonFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            NotificaitonFilter NotificaitonFilter = ConvertFilterDTOToFilterEntity(Notificaiton_NotificaitonFilterDTO);
            List<Notificaiton> Notificaitons = await NotificaitonService.List(NotificaitonFilter);
            List<Notificaiton_NotificaitonDTO> Notificaiton_NotificaitonDTOs = Notificaitons
                .Select(c => new Notificaiton_NotificaitonDTO(c)).ToList();
            return Notificaiton_NotificaitonDTOs;
        }

        [Route(NotificaitonRoute.Get), HttpPost]
        public async Task<ActionResult<Notificaiton_NotificaitonDTO>> Get([FromBody]Notificaiton_NotificaitonDTO Notificaiton_NotificaitonDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Notificaiton Notificaiton = await NotificaitonService.Get(Notificaiton_NotificaitonDTO.Id);
            return new Notificaiton_NotificaitonDTO(Notificaiton);
        }

        [Route(NotificaitonRoute.Create), HttpPost]
        public async Task<ActionResult<Notificaiton_NotificaitonDTO>> Create([FromBody] Notificaiton_NotificaitonDTO Notificaiton_NotificaitonDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Notificaiton Notificaiton = ConvertDTOToEntity(Notificaiton_NotificaitonDTO);
            Notificaiton = await NotificaitonService.Create(Notificaiton);
            Notificaiton_NotificaitonDTO = new Notificaiton_NotificaitonDTO(Notificaiton);
            if (Notificaiton.IsValidated)
                return Notificaiton_NotificaitonDTO;
            else
                return BadRequest(Notificaiton_NotificaitonDTO);
        }

        [Route(NotificaitonRoute.Update), HttpPost]
        public async Task<ActionResult<Notificaiton_NotificaitonDTO>> Update([FromBody] Notificaiton_NotificaitonDTO Notificaiton_NotificaitonDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            
            Notificaiton Notificaiton = ConvertDTOToEntity(Notificaiton_NotificaitonDTO);
            Notificaiton = await NotificaitonService.Update(Notificaiton);
            Notificaiton_NotificaitonDTO = new Notificaiton_NotificaitonDTO(Notificaiton);
            if (Notificaiton.IsValidated)
                return Notificaiton_NotificaitonDTO;
            else
                return BadRequest(Notificaiton_NotificaitonDTO);
        }

        [Route(NotificaitonRoute.Delete), HttpPost]
        public async Task<ActionResult<Notificaiton_NotificaitonDTO>> Delete([FromBody] Notificaiton_NotificaitonDTO Notificaiton_NotificaitonDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            Notificaiton Notificaiton = ConvertDTOToEntity(Notificaiton_NotificaitonDTO);
            Notificaiton = await NotificaitonService.Delete(Notificaiton);
            Notificaiton_NotificaitonDTO = new Notificaiton_NotificaitonDTO(Notificaiton);
            if (Notificaiton.IsValidated)
                return Notificaiton_NotificaitonDTO;
            else
                return BadRequest(Notificaiton_NotificaitonDTO);
        }
        
        private Notificaiton ConvertDTOToEntity(Notificaiton_NotificaitonDTO Notificaiton_NotificaitonDTO)
        {
            Notificaiton Notificaiton = new Notificaiton();
            Notificaiton.Id = Notificaiton_NotificaitonDTO.Id;
            Notificaiton.AccountId = Notificaiton_NotificaitonDTO.AccountId;
            Notificaiton.Content = Notificaiton_NotificaitonDTO.Content;
            Notificaiton.Time = Notificaiton_NotificaitonDTO.Time;
            Notificaiton.Unread = Notificaiton_NotificaitonDTO.Unread;
            Notificaiton.Account = Notificaiton_NotificaitonDTO.Account == null ? null : new Account
            {
                Id = Notificaiton_NotificaitonDTO.Account.Id,
                DisplayName = Notificaiton_NotificaitonDTO.Account.DisplayName,
                Email = Notificaiton_NotificaitonDTO.Account.Email,
                Phone = Notificaiton_NotificaitonDTO.Account.Phone,
                Password = Notificaiton_NotificaitonDTO.Account.Password,
                Salt = Notificaiton_NotificaitonDTO.Account.Salt,
                PasswordRecoveryCode = Notificaiton_NotificaitonDTO.Account.PasswordRecoveryCode,
                ExpiredTimeCode = Notificaiton_NotificaitonDTO.Account.ExpiredTimeCode,
                Address = Notificaiton_NotificaitonDTO.Account.Address,
                Dob = Notificaiton_NotificaitonDTO.Account.Dob,
                Avatar = Notificaiton_NotificaitonDTO.Account.Avatar,
                RoleId = Notificaiton_NotificaitonDTO.Account.RoleId,
            };
            Notificaiton.BaseLanguage = CurrentContext.Language;
            return Notificaiton;
        }

        private NotificaitonFilter ConvertFilterDTOToFilterEntity(Notificaiton_NotificaitonFilterDTO Notificaiton_NotificaitonFilterDTO)
        {
            NotificaitonFilter NotificaitonFilter = new NotificaitonFilter();
            NotificaitonFilter.Selects = NotificaitonSelect.ALL;
            NotificaitonFilter.Skip = Notificaiton_NotificaitonFilterDTO.Skip;
            NotificaitonFilter.Take = Notificaiton_NotificaitonFilterDTO.Take;
            NotificaitonFilter.OrderBy = Notificaiton_NotificaitonFilterDTO.OrderBy;
            NotificaitonFilter.OrderType = Notificaiton_NotificaitonFilterDTO.OrderType;

            NotificaitonFilter.Id = Notificaiton_NotificaitonFilterDTO.Id;
            NotificaitonFilter.AccountId = Notificaiton_NotificaitonFilterDTO.AccountId;
            NotificaitonFilter.Content = Notificaiton_NotificaitonFilterDTO.Content;
            NotificaitonFilter.Time = Notificaiton_NotificaitonFilterDTO.Time;
            return NotificaitonFilter;
        }

        [Route(NotificaitonRoute.SingleListAccount), HttpPost]
        public async Task<List<Notificaiton_AccountDTO>> SingleListAccount([FromBody] Notificaiton_AccountFilterDTO Notificaiton_AccountFilterDTO)
        {
            AccountFilter AccountFilter = new AccountFilter();
            AccountFilter.Skip = 0;
            AccountFilter.Take = 20;
            AccountFilter.OrderBy = AccountOrder.Id;
            AccountFilter.OrderType = OrderType.ASC;
            AccountFilter.Selects = AccountSelect.ALL;
            AccountFilter.Id = Notificaiton_AccountFilterDTO.Id;
            AccountFilter.DisplayName = Notificaiton_AccountFilterDTO.DisplayName;
            AccountFilter.Email = Notificaiton_AccountFilterDTO.Email;
            AccountFilter.Phone = Notificaiton_AccountFilterDTO.Phone;
            AccountFilter.Password = Notificaiton_AccountFilterDTO.Password;
            AccountFilter.Salt = Notificaiton_AccountFilterDTO.Salt;
            AccountFilter.PasswordRecoveryCode = Notificaiton_AccountFilterDTO.PasswordRecoveryCode;
            AccountFilter.ExpiredTimeCode = Notificaiton_AccountFilterDTO.ExpiredTimeCode;
            AccountFilter.Address = Notificaiton_AccountFilterDTO.Address;
            AccountFilter.Dob = Notificaiton_AccountFilterDTO.Dob;
            AccountFilter.Avatar = Notificaiton_AccountFilterDTO.Avatar;
            AccountFilter.RoleId = Notificaiton_AccountFilterDTO.RoleId;

            List<Account> Accounts = await AccountService.List(AccountFilter);
            List<Notificaiton_AccountDTO> Notificaiton_AccountDTOs = Accounts
                .Select(x => new Notificaiton_AccountDTO(x)).ToList();
            return Notificaiton_AccountDTOs;
        }

    }
}

