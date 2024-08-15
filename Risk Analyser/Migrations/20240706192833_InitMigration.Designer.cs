﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Risk_analyser.Data.DBContext;


#nullable disable

namespace Risk_analyser.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240706192833_InitMigration")]
    partial class InitMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ControlRiskRisk", b =>
                {
                    b.Property<long>("ControlsControlRiskId")
                        .HasColumnType("bigint");

                    b.Property<long>("RisksRiskId")
                        .HasColumnType("bigint");

                    b.HasKey("ControlsControlRiskId", "RisksRiskId");

                    b.HasIndex("RisksRiskId");

                    b.ToTable("ControlRiskRisk");
                });

            modelBuilder.Entity("Risk_analyser.Model.ActionReport", b =>
                {
                    b.Property<long>("RiskId")
                        .HasColumnType("bigint");

                    b.Property<long>("MitagatioActionId")
                        .HasColumnType("bigint");

                    b.Property<long>("ActionReportId")
                        .HasColumnType("bigint");

                    b.HasKey("RiskId", "MitagatioActionId");

                    b.HasIndex("MitagatioActionId");

                    b.ToTable("ActionReports");
                });

            modelBuilder.Entity("Risk_analyser.Model.Asset", b =>
                {
                    b.Property<long>("AssetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AssetId"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AssetId");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("Risk_analyser.Model.ControlRisk", b =>
                {
                    b.Property<long>("ControlRiskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ControlRiskId"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ControlRiskId");

                    b.ToTable("ControlsRisks");
                });

            modelBuilder.Entity("Risk_analyser.Model.Document", b =>
                {
                    b.Property<int>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentId"));

                    b.Property<long?>("FRAPAnalysisId")
                        .HasColumnType("bigint");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("FileSize")
                        .HasColumnType("bigint");

                    b.Property<string>("FileType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("RhombusAnalysisId")
                        .HasColumnType("bigint");

                    b.HasKey("DocumentId");

                    b.HasIndex("FRAPAnalysisId");

                    b.HasIndex("RhombusAnalysisId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Risk_analyser.Model.FRAPAnalysis", b =>
                {
                    b.Property<long>("FRAPAnalysisId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("FRAPAnalysisId"));

                    b.Property<long>("AssetId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("TimeAnalysis")
                        .HasColumnType("datetime2");

                    b.HasKey("FRAPAnalysisId");

                    b.HasIndex("AssetId");

                    b.ToTable("FRAPAnalyses");
                });

            modelBuilder.Entity("Risk_analyser.Model.MitagationAction", b =>
                {
                    b.Property<long>("MitagatioActionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("MitagatioActionId"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("DateOfAction")
                        .HasColumnType("date");

                    b.Property<string>("Person")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MitagatioActionId");

                    b.ToTable("MitagationActions");
                });

            modelBuilder.Entity("Risk_analyser.Model.Rhombus", b =>
                {
                    b.Property<long>("RhombusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RhombusId"));

                    b.Property<int>("Complexity")
                        .HasColumnType("int");

                    b.Property<int>("Inovation")
                        .HasColumnType("int");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.Property<long?>("RiskId")
                        .IsRequired()
                        .HasColumnType("bigint");

                    b.Property<int>("Technology")
                        .HasColumnType("int");

                    b.HasKey("RhombusId");

                    b.HasIndex("RiskId")
                        .IsUnique();

                    b.ToTable("Rhombuses");
                });

            modelBuilder.Entity("Risk_analyser.Model.RhombusAnalysis", b =>
                {
                    b.Property<long>("RhombusAnalysisId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RhombusAnalysisId"));

                    b.Property<long>("AssetId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("TimeAnalysis")
                        .HasColumnType("datetime2");

                    b.HasKey("RhombusAnalysisId");

                    b.HasIndex("AssetId");

                    b.ToTable("RhombusAnalyses");
                });

            modelBuilder.Entity("Risk_analyser.Model.Risk", b =>
                {
                    b.Property<long>("RiskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RiskId"));

                    b.Property<long>("AssetId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("PotencialEffect")
                        .HasColumnType("int");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<int>("Propability")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("RiskId");

                    b.HasIndex("AssetId");

                    b.ToTable("Risks");
                });

            modelBuilder.Entity("ControlRiskRisk", b =>
                {
                    b.HasOne("Risk_analyser.Model.ControlRisk", null)
                        .WithMany()
                        .HasForeignKey("ControlsControlRiskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Risk_analyser.Model.Risk", null)
                        .WithMany()
                        .HasForeignKey("RisksRiskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Risk_analyser.Model.ActionReport", b =>
                {
                    b.HasOne("Risk_analyser.Model.MitagationAction", "Action")
                        .WithMany("ActionReports")
                        .HasForeignKey("MitagatioActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Risk_analyser.Model.Risk", "Risk")
                        .WithMany("DoneMitigationActions")
                        .HasForeignKey("RiskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Action");

                    b.Navigation("Risk");
                });

            modelBuilder.Entity("Risk_analyser.Model.Document", b =>
                {
                    b.HasOne("Risk_analyser.Model.FRAPAnalysis", "FRAPAnalysis")
                        .WithMany("Results")
                        .HasForeignKey("FRAPAnalysisId");

                    b.HasOne("Risk_analyser.Model.RhombusAnalysis", "RhombusAnalysis")
                        .WithMany("Results")
                        .HasForeignKey("RhombusAnalysisId");

                    b.Navigation("FRAPAnalysis");

                    b.Navigation("RhombusAnalysis");
                });

            modelBuilder.Entity("Risk_analyser.Model.FRAPAnalysis", b =>
                {
                    b.HasOne("Risk_analyser.Model.Asset", "Asset")
                        .WithMany("FRAPAnalysis")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");
                });

            modelBuilder.Entity("Risk_analyser.Model.Rhombus", b =>
                {
                    b.HasOne("Risk_analyser.Model.Risk", "Risk")
                        .WithOne("RhombusParams")
                        .HasForeignKey("Risk_analyser.Model.Rhombus", "RiskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Risk");
                });

            modelBuilder.Entity("Risk_analyser.Model.RhombusAnalysis", b =>
                {
                    b.HasOne("Risk_analyser.Model.Asset", "Asset")
                        .WithMany("RhombusAnalysis")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");
                });

            modelBuilder.Entity("Risk_analyser.Model.Risk", b =>
                {
                    b.HasOne("Risk_analyser.Model.Asset", "Asset")
                        .WithMany("Risks")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");
                });

            modelBuilder.Entity("Risk_analyser.Model.Asset", b =>
                {
                    b.Navigation("FRAPAnalysis");

                    b.Navigation("RhombusAnalysis");

                    b.Navigation("Risks");
                });

            modelBuilder.Entity("Risk_analyser.Model.FRAPAnalysis", b =>
                {
                    b.Navigation("Results");
                });

            modelBuilder.Entity("Risk_analyser.Model.MitagationAction", b =>
                {
                    b.Navigation("ActionReports");
                });

            modelBuilder.Entity("Risk_analyser.Model.RhombusAnalysis", b =>
                {
                    b.Navigation("Results");
                });

            modelBuilder.Entity("Risk_analyser.Model.Risk", b =>
                {
                    b.Navigation("DoneMitigationActions");

                    b.Navigation("RhombusParams")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
