using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Risk_analyser.Data.Repository
{
    public class AssetRepository
    {
        private  DbSet<Asset> assets;
        private  ContextManager ContextManager;

        public AssetRepository(DbSet<Asset> Assets)
        {
            assets = Assets;
        }

        public void AddAsset(Asset asset)
        {
            assets.Add(asset);
            ContextManager.SaveChanges();
        }
        public void EditAsset(Asset asset)
        {
            assets.Update(asset);
            ContextManager.SaveChanges();
        }
        public void DeleteAsset(Asset asset)
        {
            assets.Remove(asset);
            ContextManager.SaveChanges();
        }
        public List<Asset> GetAllAssets()
        {
            return assets.ToList();
        }
        public Asset LoadAssetWithEntities(long id)
        {
            return assets.Include(x => x.Risks)
                .Include(x => x.FRAPAnalysis)
                .ThenInclude(x=>x.Results)
                .Include(x => x.RhombusAnalysis)
                .ThenInclude(r=>r.Results)
                .Single(x => x.AssetId == id);
        }
       
    }
}
