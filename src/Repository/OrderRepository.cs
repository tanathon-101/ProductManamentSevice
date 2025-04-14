using Microsoft.Extensions.Configuration;
using ProductmanagementCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System;
using System.Linq;

namespace ProductmanagementCore.Repository
{
    public interface IOrdersRepository
    {
        Task<IEnumerable<Orders>> GetAll();
        Task<Orders> FindById(int id);
        Task<int> DeleteAsync(int entity);
        Task<int> UpdateAsync(Orders entity);
        Task<int> AddAsync(Orders entity);
        Task<IEnumerable<Orders>> FindByUserId(int id);
        Task<IQueryable<Orders>> QueryBy(Func<Orders, bool> predicate);

    }

    public class OrdersRepository : GenericReposiory<Orders>, IOrdersRepository
    {
        public OrdersRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override string CreateSeleteString()
        {
            return "SELECT * FROM [Orders] ";
        }

        public override async Task<int> DeleteAsync(int id)
        {
            var sqlCommand = string.Format(@"DELETE FROM [Orders] WHERE [Id] = @Id");
            return await WithConnection(async conn =>
             {
                 return await conn.ExecuteAsync(sqlCommand, new { Id = id });
             });
        }
        public async Task<IEnumerable<Orders>> FindByUserId(int id)
        {
            var sqlCommand = string.Format(@"SELECT * FROM [Orders] WHERE [UserId] = @UserId");

            return await WithConnection(async conn =>
            {
                return await conn.QueryAsync<Orders>(sqlCommand, new { Id = id });
            });
        }
        public override async Task<int> UpdateAsync(Orders entity)
        {
            var sqlCommand = string.Format(@"UPDATE [Orders] SET [ProductId] = @ProductId ,[UserId] = @UserId where [Id] =@Id");
            return await WithConnection(async conn =>
            {
                return await conn.ExecuteAsync(sqlCommand, new
                {
                    entity.Id,
                    entity.ProductId,
                    entity.UserId
                });
            });
        }

        public override async Task<int> AddAsync(Orders entity)
        {
            var sqlCommand = @"INSERT INTO [Orders] ([ProductId],[UserId]) VALUES (@ProductId,@UserId)SELECT CAST(SCOPE_IDENTITY() as int)";

            return await WithConnection(async conn =>
            {
                return await conn.ExecuteScalarAsync<int>(sqlCommand, new
                {
                    entity.Id,
                    entity.ProductId,
                    entity.UserId
                });
            });
        }

        public override Task<IQueryable<Orders>> QueryBy(Func<Orders, bool> predicate)
        {
            throw new System.NotImplementedException();
        }
    }
}
