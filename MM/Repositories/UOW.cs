using Common;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using MM.Models;
using MM.Repositories;

namespace MM.Repositories
{
    public interface IUOW : IServiceScoped
    {
        Task Begin();
        Task Commit();
        Task Rollback();

        IAccountRepository AccountRepository { get; }
        IAccountFoodFavoriteRepository AccountFoodFavoriteRepository { get; }
        IFoodRepository FoodRepository { get; }
        IFoodGroupingRepository FoodGroupingRepository { get; }
        IFoodTypeRepository FoodTypeRepository { get; }
        INotificaitonRepository NotificaitonRepository { get; }
        IOrderContentRepository OrderContentRepository { get; }
        IOrderRepository OrderRepository { get; }
        IReservationRepository ReservationRepository { get; }
        ITableRepository TableRepository { get; }
    }

    public class UOW : IUOW
    {
        private DataContext DataContext;

        public IAccountRepository AccountRepository { get; private set; }
        public IAccountFoodFavoriteRepository AccountFoodFavoriteRepository { get; private set; }
        public IFoodRepository FoodRepository { get; private set; }
        public IFoodGroupingRepository FoodGroupingRepository { get; private set; }
        public IFoodTypeRepository FoodTypeRepository { get; private set; }
        public INotificaitonRepository NotificaitonRepository { get; private set; }
        public IOrderContentRepository OrderContentRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IReservationRepository ReservationRepository { get; private set; }
        public ITableRepository TableRepository { get; private set; }

        public UOW(DataContext DataContext)
        {
            this.DataContext = DataContext;

            AccountRepository = new AccountRepository(DataContext);
            AccountFoodFavoriteRepository = new AccountFoodFavoriteRepository(DataContext);
            FoodRepository = new FoodRepository(DataContext);
            FoodGroupingRepository = new FoodGroupingRepository(DataContext);
            FoodTypeRepository = new FoodTypeRepository(DataContext);
            NotificaitonRepository = new NotificaitonRepository(DataContext);
            OrderContentRepository = new OrderContentRepository(DataContext);
            OrderRepository = new OrderRepository(DataContext);
            ReservationRepository = new ReservationRepository(DataContext);
            TableRepository = new TableRepository(DataContext);
        }
        public async Task Begin()
        {
            await DataContext.Database.BeginTransactionAsync();
        }

        public Task Commit()
        {
            DataContext.Database.CommitTransaction();
            return Task.CompletedTask;
        }

        public Task Rollback()
        {
            DataContext.Database.RollbackTransaction();
            return Task.CompletedTask;
        }
    }
}