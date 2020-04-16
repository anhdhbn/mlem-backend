using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MM.Entities;
using MM;
using MM.Repositories;

namespace MM.Services.MNotificaiton
{
    public interface INotificaitonValidator : IServiceScoped
    {
        Task<bool> Create(Notificaiton Notificaiton);
        Task<bool> Update(Notificaiton Notificaiton);
        Task<bool> Delete(Notificaiton Notificaiton);
        Task<bool> BulkDelete(List<Notificaiton> Notificaitons);
        Task<bool> Import(List<Notificaiton> Notificaitons);
    }

    public class NotificaitonValidator : INotificaitonValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public NotificaitonValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Notificaiton Notificaiton)
        {
            NotificaitonFilter NotificaitonFilter = new NotificaitonFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Notificaiton.Id },
                Selects = NotificaitonSelect.Id
            };

            int count = await UOW.NotificaitonRepository.Count(NotificaitonFilter);
            if (count == 0)
                Notificaiton.AddError(nameof(NotificaitonValidator), nameof(Notificaiton.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Notificaiton Notificaiton)
        {
            return Notificaiton.IsValidated;
        }

        public async Task<bool> Update(Notificaiton Notificaiton)
        {
            if (await ValidateId(Notificaiton))
            {
            }
            return Notificaiton.IsValidated;
        }

        public async Task<bool> Delete(Notificaiton Notificaiton)
        {
            if (await ValidateId(Notificaiton))
            {
            }
            return Notificaiton.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Notificaiton> Notificaitons)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Notificaiton> Notificaitons)
        {
            return true;
        }
    }
}
