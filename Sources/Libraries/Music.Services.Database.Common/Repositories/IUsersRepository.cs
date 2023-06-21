using Music.Services.Models;

namespace Music.Services.Database.Common.Repositories;

public interface IUsersRepository<TUser> : IRepository<TUser> where TUser : IUserBase
{
}