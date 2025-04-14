using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace ProductmanagementCore.Repository
{
    public abstract class GenericReposiory<TModel>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;



        protected GenericReposiory(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection"); 
        }

        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                return await getData(connection);
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

       

        // use for buffered queries that do not return a type
        protected async Task WithConnection(Func<IDbConnection, Task> getData)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                await getData(connection);
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

        //use for non-buffered queries that return a type
        protected async ValueTask<TResult> WithConnection<TRead, TResult>(Func<IDbConnection, Task<TRead>> getData, Func<TRead, Task<TResult>> process)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                var data = await getData(connection);
                return await process(data);
            }
            catch (TimeoutException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL timeout", GetType().FullName), ex);
            }
            catch (SqlException ex)
            {
                throw new Exception(String.Format("{0}.WithConnection() experienced a SQL exception (not a timeout)", GetType().FullName), ex);
            }
        }

        public async Task<IEnumerable<TModel>> GetAll()
        {
            return await WithConnection(async conn =>
            {
                var query = await conn.QueryAsync<TModel>(CreateSeleteString());
                return query;
            });


        }

        public async Task<TModel> FindById(int id)
        {

            return await WithConnection(async conn =>
            {
                var query = await conn.QueryFirstOrDefaultAsync<TModel>(CreateSeleteString() + " WHERE Id = @Id", new { Id = id });
                return query;
            });
        }

        public abstract string CreateSeleteString();
        public abstract Task<int> UpdateAsync(TModel tModel);
        public abstract Task<int> DeleteAsync(int id);
        public abstract Task<int> AddAsync(TModel tModel);
        public abstract Task<IQueryable<TModel>> QueryBy(Func<TModel, bool> predicate);
    }

}
