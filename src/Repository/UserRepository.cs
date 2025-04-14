using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using ProductmanagementCore.Models;

namespace ProductmanagementCore.Repository
{
    public interface IUserRepository
    {
        Task<int> AddAsync(Users tModel);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<Users>> GetAll();
        Task<Users> FindById(int id);
        Task<int> UpdateAsync(Users tUsers);
        Task<IQueryable<Users>> QueryBy(Func<Users, bool> predicate);
    }
    public class UserRepository : GenericReposiory<Users>, IUserRepository
    {

        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public override string CreateSeleteString()
        {
            return "SELECT * FROM [Users] ";
        }

        public override async Task<int> AddAsync(Users tModel)
        {
            var sqlCommand = @"INSERT INTO [Users] ([Username],[Firstname],[Lastname],[Email],[Tel])
                            VALUES (@Username,@Firstname,@Lastname,@Email,@Tel)SELECT CAST(SCOPE_IDENTITY() as int)";

            return await WithConnection(async conn =>
           {
               return await conn.ExecuteScalarAsync<int>(sqlCommand, new
               {
                   tModel.Id,
                   tModel.Email,
                   tModel.Firstname,
                   tModel.Username,
                   tModel.Lastname,
                   tModel.Tel,
               });
           });


        }

        public override async Task<int> DeleteAsync(int id)
        {
            var sqlCommand = string.Format(@"DELETE FROM [Users] WHERE [Id] = @Id");
            return await WithConnection(async conn =>
            {
                return await conn.ExecuteAsync(sqlCommand, new { Id = id });
            });

        }

        public override async Task<int> UpdateAsync(Users tModel)
        {
            var sqlCommand = string.Format(@"UPDATE [Users] SET [Username] = @Username ,[Firstname] = @Firstname ,[Lastname] =@Lastname ,[Email] =@Email ,[Tel] =@Tel,[Password] = @Password WHERE [Id] = @Id");


            return await WithConnection(async conn =>
            {
                return await conn.ExecuteAsync(sqlCommand, new
                {
                    tModel.Id,
                    tModel.Username,
                    Fristname = tModel.Firstname,
                    tModel.Lastname,
                    tModel.Email,
                    tModel.Tel,
                });
            });
        }

        public override Task<IQueryable<Users>> QueryBy(Func<Users, bool> predicate)
        {
            throw new System.NotImplementedException();
        }
    }
}
