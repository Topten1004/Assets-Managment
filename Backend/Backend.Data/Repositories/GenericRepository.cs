using Backend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data.Repositories
{
    public interface IGenericRepository
    {
        #region Asset
        Task<IEnumerable<AssetEntity>> GetAssetsList();
        Task<AssetEntity> GetAssetDetailById(int Id);
        Task<AssetEntity> SaveAssetDetail(AssetEntity model);
        Task<AssetEntity> UpdateAssetDetail(AssetEntity model);
        Task DeleteAsset(int Id);
        #endregion

        #region Command
        Task<IEnumerable<CommandEntity>> GetCommandsList();
        Task<CommandEntity> GetCommandDetailById(int Id);
        Task<CommandEntity> SaveCommandDetail(CommandEntity model);
        Task<CommandEntity> UpdateCommandDetail(CommandEntity model);
        Task DeleteCommand(int Id);
        #endregion

        #region User
        Task<IEnumerable<UserEntity>> GetUsersList();
        Task<UserEntity> GetUserDetailById(int Id);
        Task<UserEntity> SaveUserDetail(UserEntity model);
        Task<UserEntity> UpdateUserDetail(UserEntity model);
        Task DeleteUser(int Id);
        #endregion

        #region Log
        Task<IEnumerable<LogEntity>> GetLogsList();
        Task<LogEntity> GetLogDetailById(int Id);
        Task<LogEntity> SaveLogDetail(LogEntity model);
        Task<LogEntity> UpdateLogDetail(LogEntity model);
        Task DeleteLog(int Id);
        #endregion
    }

    public class GenericRepository : IGenericRepository
    {
        private readonly ApplicationDbContext _model;
        public GenericRepository(ApplicationDbContext model)
        {
            _model = model;
        }

        #region Asset
        public async Task<IEnumerable<AssetEntity>> GetAssetsList()
        {
            var model = await _model.Assets.Include( x => x.Owner).ToListAsync();
            return model;
        }

        public async Task<AssetEntity> GetAssetDetailById(int Id)
        {
            return await _model.Assets.FindAsync(Id);
        }

        public async Task<AssetEntity> SaveAssetDetail(AssetEntity model)
        {
            await _model.Assets.AddAsync(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task<AssetEntity> UpdateAssetDetail(AssetEntity model)
        {
            _model.Update(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task DeleteAsset(int Id)
        {
            AssetEntity asset = await _model.Assets.FindAsync(Id);
            if (asset != null)
            {
                _model.Remove(asset);
                await _model.SaveChangesAsync();
            }
        }

        #endregion

        #region Command
        public async Task<IEnumerable<CommandEntity>> GetCommandsList()
        {
            var model = await _model.Commands.Include( x => x.Owner).ToListAsync();
            return model;
        }

        public async Task<CommandEntity> GetCommandDetailById(int Id)
        {
            return await _model.Commands.FindAsync(Id);
        }

        public async Task<CommandEntity> SaveCommandDetail(CommandEntity model)
        {
            await _model.Commands.AddAsync(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task<CommandEntity> UpdateCommandDetail(CommandEntity model)
        {
            _model.Commands.Update(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task DeleteCommand(int Id)
        {
            CommandEntity asset = await _model.Commands.FindAsync(Id);
            if (asset != null)
            {
                _model.Remove(asset);
                await _model.SaveChangesAsync();
            }
        }
        #endregion

        #region User
        public async Task<IEnumerable<UserEntity>> GetUsersList()
        {
            var model = await _model.Users.Include(x => x.Assets).ToListAsync();
            return model;
        }

        public async Task<UserEntity> GetUserDetailById(int Id)
        {
            return await _model.Users.FindAsync(Id);
        }

        public async Task<UserEntity> SaveUserDetail(UserEntity model)
        {
            await _model.Users.AddAsync(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task<UserEntity> UpdateUserDetail(UserEntity model)
        {
            _model.Users.Update(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task DeleteUser(int Id)
        {
            UserEntity asset = await _model.Users.FindAsync(Id);
            if (asset != null)
            {
                _model.Remove(asset);
                await _model.SaveChangesAsync();
            }
        }
        #endregion

        #region Log
        public async Task<IEnumerable<LogEntity>> GetLogsList()
        {
            var model = await _model.Logs.ToListAsync();
            return model;
        }

        public async Task<LogEntity> GetLogDetailById(int Id)
        {
            return await _model.Logs.FindAsync(Id);
        }

        public async Task<LogEntity> SaveLogDetail(LogEntity model)
        {
            await _model.Logs.AddAsync(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task<LogEntity> UpdateLogDetail(LogEntity model)
        {
            _model.Logs.Update(model);
            await _model.SaveChangesAsync();
            return model;
        }

        public async Task DeleteLog(int Id)
        {
            LogEntity asset = await _model.Logs.FindAsync(Id);
            if (asset != null)
            {
                _model.Remove(asset);
                await _model.SaveChangesAsync();
            }
        }
        #endregion
    }
}
