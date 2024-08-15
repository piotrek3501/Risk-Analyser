using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Risk_analyser.Model;
using System.Windows;

namespace Risk_analyser.Data.DBContext
{
    public class DataContext : DbContext
    {

        public DbSet<ActionReport> ActionReports { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<ControlRisk> ControlsRisks { get; set; }
        public DbSet<FRAPAnalysis> FRAPAnalyses { get; set; }
        public DbSet<Rhombus> Rhombuses { get; set; }
        public DbSet<RhombusAnalysis> RhombusAnalyses { get; set; }
        public DbSet<Risk> Risks { get; set; }
        public DbSet<MitagationAction> MitagationActions { get; set; }
        public DbSet<Document> Files { get; set; }
        public DbSet<RiskType> RiskTypes { get; set; }
        public DbSet<FRAPDocument> FRAPDocuments { get; set; }
        public DbSet<RhombusDocument> RhombusDocuments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionReport>(ar =>
            {

                ar.HasKey(ra => new { ra.RiskId, ra.MitagatioActionId });

                ar.HasOne(x => x.Risk)
                .WithMany(c => c.DoneMitigationActions)
                .HasForeignKey(x => x.RiskId);

                ar.HasOne(x => x.Action)
                .WithMany(c => c.ActionReports)
                .HasForeignKey(x => x.MitagatioActionId);

            });
            modelBuilder.Entity<MitagationAction>(ma =>
            {
                ma.HasKey(x => x.MitagatioActionId);
            });

            modelBuilder.Entity<Rhombus>(r =>
            {

                r.HasKey(x => x.RhombusId);

            });
            modelBuilder.Entity<RiskType>(rt =>
            {
                rt.HasIndex(x => new { x.RiskId, x.Type })
                .IsUnique();


            });
            modelBuilder.Entity<Risk>(r =>
            {
                r.HasKey(x => x.RiskId);

                r.HasOne(x => x.RhombusParams)
                .WithOne(c => c.Risk)
                .HasForeignKey<Rhombus>(c => c.RiskId);

                r.HasOne(x => x.Asset)
                .WithMany(c => c.Risks);

                r.HasMany(x => x.Controls)
                .WithMany(c => c.Risks);

                r.HasMany(x => x.Types)
                .WithOne(c => c.Risk)
                .HasForeignKey(c => c.RiskId);


            });

            modelBuilder.Entity<Asset>(a =>
            {
                a.HasKey(x => x.AssetId);

                a.HasMany(x => x.Risks)
                .WithOne(c => c.Asset);

            });

            modelBuilder.Entity<RhombusAnalysis>(ra =>
            {
                ra.HasKey(x => x.RhombusAnalysisId);

                ra.HasOne(x => x.Asset)
                .WithMany(c => c.RhombusAnalysis);

                ra.HasMany(x => x.Results)
                .WithOne(c => c.RhombusAnalysis)
                .HasForeignKey(c => c.RhombusAnalysisId);

            });

            modelBuilder.Entity<FRAPAnalysis>(fa =>
            {
                fa.HasKey(x => x.FRAPAnalysisId);

                fa.HasOne(x => x.Asset)
                .WithMany(c => c.FRAPAnalysis);

                fa.HasMany(x => x.Results)
                .WithOne(c => c.FRAPAnalysis)
                .HasForeignKey(c => c.FRAPAnalysisId);

            });

            modelBuilder.Entity<ControlRisk>(cr =>
            {
                cr.HasKey(x => x.ControlRiskId);

                cr.HasMany(x => x.Risks)
                .WithMany(c => c.Controls);

                cr.HasMany(x => x.Mitagations)
                .WithMany(c => c.ControlRisks);

            });

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server =.\\SQLEXPRESS2022; Database = RiskAnalyserDB; Trusted_Connection = True;Encrypt=False");
        }
    }

}
