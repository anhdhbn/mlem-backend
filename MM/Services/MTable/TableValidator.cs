using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MM.Entities;
using MM;
using MM.Repositories;

namespace MM.Services.MTable
{
    public interface ITableValidator : IServiceScoped
    {
        Task<bool> Create(Table Table);
        Task<bool> Update(Table Table);
        Task<bool> Delete(Table Table);
        Task<bool> BulkDelete(List<Table> Tables);
        Task<bool> Import(List<Table> Tables);
    }

    public class TableValidator : ITableValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public TableValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(Table Table)
        {
            TableFilter TableFilter = new TableFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = Table.Id },
                Selects = TableSelect.Id
            };

            int count = await UOW.TableRepository.Count(TableFilter);
            if (count == 0)
                Table.AddError(nameof(TableValidator), nameof(Table.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool>Create(Table Table)
        {
            return Table.IsValidated;
        }

        public async Task<bool> Update(Table Table)
        {
            if (await ValidateId(Table))
            {
            }
            return Table.IsValidated;
        }

        public async Task<bool> Delete(Table Table)
        {
            if (await ValidateId(Table))
            {
            }
            return Table.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<Table> Tables)
        {
            return true;
        }
        
        public async Task<bool> Import(List<Table> Tables)
        {
            return true;
        }
    }
}
