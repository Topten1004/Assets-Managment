using Backend.Data.Entities;
using Backend.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Business.Services
{
    public interface IGenericService
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

    public class GenericService : IGenericService
    {
        private readonly IGenericRepository _assetRepository;

        public GenericService(IGenericRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        #region Asset
        public async Task<IEnumerable<AssetEntity>> GetAssetsList()
        {
            return await _assetRepository.GetAssetsList();
        }

        public async Task<AssetEntity> GetAssetDetailById(int Id)
        {
            return await _assetRepository.GetAssetDetailById(Id);
        }

        public async Task<AssetEntity> SaveAssetDetail(AssetEntity model)
        {
            return await _assetRepository.SaveAssetDetail(model);
        }

        public async Task<AssetEntity> UpdateAssetDetail(AssetEntity model)
        {
            return await _assetRepository.UpdateAssetDetail(model);
        }

        public async Task DeleteAsset(int Id)
        {
            await _assetRepository.DeleteAsset(Id);
        }
        #endregion

        #region Command
        public async Task<IEnumerable<CommandEntity>> GetCommandsList()
        {
            return await _assetRepository.GetCommandsList();
        }

        public async Task<CommandEntity> GetCommandDetailById(int Id)
        {
            return await _assetRepository.GetCommandDetailById(Id);
        }

        public async Task<CommandEntity> SaveCommandDetail(CommandEntity model)
        {
            return await _assetRepository.SaveCommandDetail(model);
        }

        public async Task<CommandEntity> UpdateCommandDetail(CommandEntity model)
        {
            return await _assetRepository.UpdateCommandDetail(model);
        }

        public async Task DeleteCommand(int Id)
        {
            await _assetRepository.DeleteAsset(Id);
        }
        #endregion

        #region User
        public async Task<IEnumerable<UserEntity>> GetUsersList()
        {
            return await _assetRepository.GetUsersList();
        }

        public async Task<UserEntity> GetUserDetailById(int Id)
        {
            return await _assetRepository.GetUserDetailById(Id);
        }

        public async Task<UserEntity> SaveUserDetail(UserEntity model)
        {
            return await _assetRepository.SaveUserDetail(model);
        }

        public async Task<UserEntity> UpdateUserDetail(UserEntity model)
        {
            return await _assetRepository.UpdateUserDetail(model);
        }

        public async Task DeleteUser(int Id)
        {
            await _assetRepository.DeleteUser(Id);
        }
        #endregion

        #region Logs
        public async Task<IEnumerable<LogEntity>> GetLogsList()
        {
            return await _assetRepository.GetLogsList();
        }

        public async Task<LogEntity> GetLogDetailById(int Id)
        {
            return await _assetRepository.GetLogDetailById(Id);
        }

        public async Task<LogEntity> SaveLogDetail(LogEntity model)
        {
            return await _assetRepository.SaveLogDetail(model);
        }

        public async Task<LogEntity> UpdateLogDetail(LogEntity model)
        {
            return await _assetRepository.UpdateLogDetail(model);
        }

        public async Task DeleteLog(int Id)
        {
            await _assetRepository.DeleteLog(Id);
        }
        #endregion
    }
}
