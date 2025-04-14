using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using ProductmanagementCore.Models;

namespace ProductmanagementCore.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Products>> GetAll();
        Task<Products> FindById(int id);
        Task<int> AddAsync(Products entity);
        Task<int> UpdateAsync(Products entity);
        Task<int> DeleteAsync(int id);
        Task<IQueryable<Products>> QueryBy(Func<Products, bool> predicate);
    }

    public class ProductRepository : GenericReposiory<Products>,IProductRepository
    {

        public ProductRepository(IConfiguration configuration) : base(configuration)
        {
           
        }

        public override string CreateSeleteString()
        {
            return "SELECT * FROM [Products] ";
        }



        public override async Task< int> AddAsync(Products entity)
        {
            const string sqlCommand = @"INSERT INTO [Products] ([Name],[Price]) VALUES (@Name,@Price)SELECT CAST(SCOPE_IDENTITY() as int)";
            return await WithConnection(async conn =>
            {
                return await conn.ExecuteScalarAsync<int>(sqlCommand, new
                {
                    entity.Id,
                    entity.Name,
                    entity.Price
                });
            });
        }

        public override async Task<int> UpdateAsync(Products entity)
        {
            var sqlCommand = @"UPDATE [Products] SET [Name] = @Name ,[Price] = @Price where [Id] =@Id";
            return await WithConnection(async conn =>
            {
                return await conn.ExecuteAsync(sqlCommand, new
                {
                    entity.Id,
                    entity.Name,
                    entity.Price
                });
            });
        }

        public override async Task<int> DeleteAsync(int id)
        {
            var sqlCommand = @"DELETE FROM [Products] WHERE [Id] = @Id";
            return await WithConnection(async conn =>
            {
                return await conn.ExecuteAsync(sqlCommand, new { Id = id });
            });
        }

        public override async Task<IQueryable<Products>> QueryBy(Func<Products, bool> predicate)
        {


            return await WithConnection(async conn =>
            {
                return (await conn.QueryAsync<Products>(CreateSeleteString(), null)).Where(predicate).AsQueryable();
            });
        }
    }
}