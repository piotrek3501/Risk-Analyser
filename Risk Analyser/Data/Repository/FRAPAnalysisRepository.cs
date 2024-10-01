using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Repository
{
    public class FRAPAnalysisRepository
    {
        private  DbSet<FRAPAnalysis> FRAPAnalyses;
        private List<FRAPDocument> Documents;
        private  ContextManager ContextManager;

        public FRAPAnalysisRepository(DbSet<FRAPAnalysis> fRAPAnalyses) {
            FRAPAnalyses= fRAPAnalyses;
            
        }
       
        public List<FRAPDocument> GetAllFRAPAnalysisForAsset(Asset asset)
        {
            Documents=FRAPAnalyses.Where(x=>x.Asset.AssetId==asset.AssetId).SelectMany(c=>c.Results).ToList();
            return Documents;
        }
      

       
    }
}
