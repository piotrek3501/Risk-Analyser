using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Model;
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
        private DataContext _context;
        private DbSet<Asset> assets;
        public AssetRepository(DataContext context)
        {
            _context = context;
            assets = _context.Assets;
        }

        public void AddAsset(Asset asset)
        {
            assets.Add(asset);
            _context.SaveChanges();
        }
        public void EditAsset(Asset asset)
        {
            assets.Update(asset);
            _context.SaveChanges();
        }
        public void DeleteAsset(Asset asset)
        {
            assets.Remove(asset);
            _context.SaveChanges();
        }
        public Asset LoadAssetWithEntities(long id)
        {
            return assets.Include(x => x.Risks)
                .Include(x => x.FRAPAnalysis)
                .Include(x => x.RhombusAnalysis)
                .Single(x => x.AssetId == id);
        }
        public bool CanDeleteAsset(Asset asset)
        {
            asset=LoadAssetWithEntities(asset.AssetId);
            bool anyFRAP = _context.FRAPAnalyses.Where(x => x.Asset.AssetId == asset.AssetId).Any();
            bool anyRhombus = _context.RhombusAnalyses.Where(x => x.Asset.AssetId == asset.AssetId).Any();
            bool anyRisks=_context.Risks.Where(x=>x.Asset.AssetId==asset.AssetId).Any();
            return !(anyFRAP || anyRhombus || anyRisks);
        }
    }
}
