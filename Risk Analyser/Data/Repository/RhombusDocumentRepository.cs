using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Repository
{
    public class RhombusDocumentRepository
    {
        private DbSet<RhombusDocument> rhombusDocuments;
        private ContextManager ContextManager;
        public Exception OccuredException { get; set; }

        public RhombusDocumentRepository(DbSet<RhombusDocument> documents)
        {
            rhombusDocuments = documents;
        }
        public RhombusDocument LoadRhombusDocumentWithEntities(long id)
        {
            return rhombusDocuments.Include(x=>x.RhombusAnalysis).Single(x => x.DocumentId == id);
        }
        public bool  DeleteDocument(long id) {
            RhombusDocument rhombusDocument = LoadRhombusDocumentWithEntities(id);
            try
            {
                rhombusDocuments.Remove(rhombusDocument);
                ContextManager.SaveChanges();
                return true;
            }
            catch (Exception ex) {
                OccuredException = ex;
                return false;
            }
            
        }
    }
}
