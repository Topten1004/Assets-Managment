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
        Task<IEnumerable<AssetEntity>> GetAssetsList();
        Task<AssetEntity> GetAssetDetailById(int Id);
        Task<AssetEntity> SaveAssetDetail(AssetEntity model);
        Task<AssetEntity> UpdateAssetDetail(AssetEntity model);
        Task DeleteAsset(int Id);
    }

    public class GenericRepository : IGenericRepository
    {
        private readonly ApplicationDbContext _model;
        public GenericRepository(ApplicationDbContext model)
        {
            _model = model;
        }

        public async Task<IEnumerable<AssetEntity>> GetAssetsList()
        {
            var model = await _model.Assets.ToListAsync();
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
    }
}
