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
    public class RhombusAnalysisRepository
    {
        private  DbSet<RhombusAnalysis> RhombusAnalyses;
        private List<RhombusDocument> rhombusDocuments;
        private  ContextManager ContextManager;

        public RhombusAnalysisRepository(DbSet<RhombusAnalysis> rhombusAnalyses)
        {
            RhombusAnalyses = rhombusAnalyses;
        }
       
        public List<RhombusDocument> GetRhombusAnalysesForAsset(Asset asset)
        {
            rhombusDocuments = RhombusAnalyses.Where(x => x.Asset.AssetId == asset.AssetId).SelectMany(c => c.Results).ToList();

            return rhombusDocuments;

        }
    }
}
