using Common;
using Helpers;
using MM.Entities;
using MM.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MM.Services.MNotificaiton
{
    public interface INotificaitonService :  IServiceScoped
    {
        Task<int> Count(NotificaitonFilter NotificaitonFilter);
        Task<List<Notificaiton>> List(NotificaitonFilter NotificaitonFilter);
        Task<Notificaiton> Get(long Id);
        Task<Notificaiton> Create(Notificaiton Notificaiton);
        Task<Notificaiton> Update(Notificaiton Notificaiton);
        Task<Notificaiton> Delete(Notificaiton Notificaiton);
        Task<List<Notificaiton>> BulkDelete(List<Notificaiton> Notificaitons);
        Task<List<Notificaiton>> Import(List<Notificaiton> Notificaitons);
    }

    public class NotificaitonService : INotificaitonService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private INotificaitonValidator NotificaitonValidator;

        public NotificaitonService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            INotificaitonValidator NotificaitonValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.NotificaitonValidator = NotificaitonValidator;
        }
        public async Task<int> Count(NotificaitonFilter NotificaitonFilter)
        {
            try
            {
                int result = await UOW.NotificaitonRepository.Count(NotificaitonFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificaitonService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Notificaiton>> List(NotificaitonFilter NotificaitonFilter)
        {
            try
            {
                List<Notificaiton> Notificaitons = await UOW.NotificaitonRepository.List(NotificaitonFilter);
                return Notificaitons;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificaitonService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Notificaiton> Get(long Id)
        {
            Notificaiton Notificaiton = await UOW.NotificaitonRepository.Get(Id);
            if (Notificaiton == null)
                return null;
            return Notificaiton;
        }
       
        public async Task<Notificaiton> Create(Notificaiton Notificaiton)
        {
            if (!await NotificaitonValidator.Create(Notificaiton))
                return Notificaiton;

            try
            {
                await UOW.Begin();
                await UOW.NotificaitonRepository.Create(Notificaiton);
                await UOW.Commit();

                await Logging.CreateAuditLog(Notificaiton, new { }, nameof(NotificaitonService));
                return await UOW.NotificaitonRepository.Get(Notificaiton.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificaitonService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Notificaiton> Update(Notificaiton Notificaiton)
        {
            if (!await NotificaitonValidator.Update(Notificaiton))
                return Notificaiton;
            try
            {
                var oldData = await UOW.NotificaitonRepository.Get(Notificaiton.Id);

                await UOW.Begin();
                await UOW.NotificaitonRepository.Update(Notificaiton);
                await UOW.Commit();

                var newData = await UOW.NotificaitonRepository.Get(Notificaiton.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(NotificaitonService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificaitonService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Notificaiton> Delete(Notificaiton Notificaiton)
        {
            if (!await NotificaitonValidator.Delete(Notificaiton))
                return Notificaiton;

            try
            {
                await UOW.Begin();
                await UOW.NotificaitonRepository.Delete(Notificaiton);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Notificaiton, nameof(NotificaitonService));
                return Notificaiton;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificaitonService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Notificaiton>> BulkDelete(List<Notificaiton> Notificaitons)
        {
            if (!await NotificaitonValidator.BulkDelete(Notificaitons))
                return Notificaitons;

            try
            {
                await UOW.Begin();
                await UOW.NotificaitonRepository.BulkDelete(Notificaitons);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Notificaitons, nameof(NotificaitonService));
                return Notificaitons;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificaitonService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Notificaiton>> Import(List<Notificaiton> Notificaitons)
        {
            if (!await NotificaitonValidator.Import(Notificaitons))
                return Notificaitons;
            try
            {
                await UOW.Begin();
                await UOW.NotificaitonRepository.BulkMerge(Notificaitons);
                await UOW.Commit();

                await Logging.CreateAuditLog(Notificaitons, new { }, nameof(NotificaitonService));
                return Notificaitons;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(NotificaitonService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }     
    }
}
