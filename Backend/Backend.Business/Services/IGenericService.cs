using Backend.Data.Entities;
using Backend.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Business.Services
{
    public interface IGenericService
    {
        Task<IEnumerable<AssetEntity>> GetAssetsList();

        Task<AssetEntity> GetAssetDetailById(int Id);

        Task<AssetEntity> SaveAssetDetail(AssetEntity model);

        Task<AssetEntity> UpdateAssetDetail(AssetEntity model);

        Task DeleteAsset(int Id);
    }

    public class GenericService : IGenericService
    {
        private readonly IGenericRepository _assetRepository;

        public GenericService(IGenericRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

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
    }
}
