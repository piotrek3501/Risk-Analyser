using Microsoft.EntityFrameworkCore;
using Risk_analyser.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Repository
{
    public class FRAPDocumentRepository
    {
        private DbSet<FRAPDocument> FRAPDocuments;
        private ContextManager ContextManager;
        public Exception OccuredException { get; set; }

        public FRAPDocumentRepository(DbSet<FRAPDocument> fRAPDocuments)
        {
            FRAPDocuments = fRAPDocuments;
        }
        public FRAPDocument LoadFRAPDocumentWithEntities(long ?id)
        {
            return FRAPDocuments.Include(x=>x.FRAPAnalysis).ThenInclude(x=>x.Asset).Single(x => x.DocumentId == id);
        }
        public bool DeleteDocument(long id)
        {
            FRAPDocument frapDocument = LoadFRAPDocumentWithEntities(id);
            try
            {
                FRAPDocuments.Remove(frapDocument);
                ContextManager.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                OccuredException = ex;
                return false;
            }

        }

    }
}
