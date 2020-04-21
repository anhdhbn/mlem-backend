using Common;
using Helpers;
using MM.Entities;
using MM.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MM.Services.MTable
{
    public interface ITableService :  IServiceScoped
    {
        Task<int> Count(TableFilter TableFilter);
        Task<List<Table>> List(TableFilter TableFilter);
        Task<Table> Get(long Id);
        Task<Table> Create(Table Table);
        Task<Table> Update(Table Table);
        Task<Table> Delete(Table Table);
    }

    public class TableService : ITableService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ITableValidator TableValidator;

        public TableService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ITableValidator TableValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.TableValidator = TableValidator;
        }
        public async Task<int> Count(TableFilter TableFilter)
        {
            try
            {
                int result = await UOW.TableRepository.Count(TableFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(TableService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Table>> List(TableFilter TableFilter)
        {
            try
            {
                List<Table> Tables = await UOW.TableRepository.List(TableFilter);
                return Tables;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(TableService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Table> Get(long Id)
        {
            Table Table = await UOW.TableRepository.Get(Id);
            if (Table == null)
                return null;
            return Table;
        }
       
        public async Task<Table> Create(Table Table)
        {
            if (!await TableValidator.Create(Table))
                return Table;

            try
            {
                await UOW.Begin();
                await UOW.TableRepository.Create(Table);
                await UOW.Commit();

                await Logging.CreateAuditLog(Table, new { }, nameof(TableService));
                return await UOW.TableRepository.Get(Table.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TableService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Table> Update(Table Table)
        {
            if (!await TableValidator.Update(Table))
                return Table;
            try
            {
                var oldData = await UOW.TableRepository.Get(Table.Id);

                await UOW.Begin();
                await UOW.TableRepository.Update(Table);
                await UOW.Commit();

                var newData = await UOW.TableRepository.Get(Table.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(TableService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TableService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Table> Delete(Table Table)
        {
            if (!await TableValidator.Delete(Table))
                return Table;

            try
            {
                await UOW.Begin();
                await UOW.TableRepository.Delete(Table);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Table, nameof(TableService));
                return Table;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TableService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
    }
}
