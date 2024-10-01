﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Risk_analyser.Data.DBContext;

#nullable disable

namespace Risk_analyser.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ControlRiskMitagationAction", b =>
                {
                    b.Property<long>("ControlRisksControlRiskId")
                        .HasColumnType("bigint");

                    b.Property<long>("MitagationsMitagatioActionId")
                        .HasColumnType("bigint");

                    b.HasKey("ControlRisksControlRiskId", "MitagationsMitagatioActionId");

                    b.HasIndex("MitagationsMitagatioActionId");

                    b.ToTable("ControlRiskMitagationAction");
                });

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

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.Asset", b =>
                {
                    b.Property<long>("AssetId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("AssetId"));

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("FRAPAnalysisId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("RhombusAnalysisId")
                        .HasColumnType("bigint");

                    b.HasKey("AssetId");

                    b.HasIndex("FRAPAnalysisId")
                        .IsUnique();

                    b.HasIndex("RhombusAnalysisId")
                        .IsUnique();

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.ControlRisk", b =>
                {
                    b.Property<long>("ControlRiskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("ControlRiskId"));

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ControlRiskId");

                    b.ToTable("ControlsRisks");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.Document", b =>
                {
                    b.Property<int>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DocumentId"));

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("nvarchar(21)");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DocumentId");

                    b.ToTable("Files");

                    b.HasDiscriminator().HasValue("Document");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.FRAPAnalysis", b =>
                {
                    b.Property<long>("FRAPAnalysisId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("FRAPAnalysisId"));

                    b.HasKey("FRAPAnalysisId");

                    b.ToTable("FRAPAnalyses");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.MitagationAction", b =>
                {
                    b.Property<long>("MitagatioActionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("MitagatioActionId"));

                    b.Property<string>("Action")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfAction")
                        .HasColumnType("datetime2");

                    b.Property<string>("Person")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("MitagatioActionId");

                    b.ToTable("MitagationActions");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.Rhombus", b =>
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

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.RhombusAnalysis", b =>
                {
                    b.Property<long>("RhombusAnalysisId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RhombusAnalysisId"));

                    b.HasKey("RhombusAnalysisId");

                    b.ToTable("RhombusAnalyses");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.Risk", b =>
                {
                    b.Property<long>("RiskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RiskId"));

                    b.Property<long>("AssetId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

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

                    b.Property<int>("Vulnerability")
                        .HasColumnType("int");

                    b.HasKey("RiskId");

                    b.HasIndex("AssetId");

                    b.ToTable("Risks");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.RiskType", b =>
                {
                    b.Property<long>("RiskTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RiskTypeId"));

                    b.Property<long>("RiskId")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("RiskTypeId");

                    b.HasIndex("RiskId", "Type")
                        .IsUnique();

                    b.ToTable("RiskTypes");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.FRAPDocument", b =>
                {
                    b.HasBaseType("Risk_analyser.Data.Model.Entities.Document");

                    b.Property<long?>("FRAPAnalysisId")
                        .HasColumnType("bigint");

                    b.HasIndex("FRAPAnalysisId");

                    b.HasDiscriminator().HasValue("FRAPDocument");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.RhombusDocument", b =>
                {
                    b.HasBaseType("Risk_analyser.Data.Model.Entities.Document");

                    b.Property<long?>("RhombusAnalysisId")
                        .HasColumnType("bigint");

                    b.HasIndex("RhombusAnalysisId");

                    b.HasDiscriminator().HasValue("RhombusDocument");
                });

            modelBuilder.Entity("ControlRiskMitagationAction", b =>
                {
                    b.HasOne("Risk_analyser.Data.Model.Entities.ControlRisk", null)
                        .WithMany()
                        .HasForeignKey("ControlRisksControlRiskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Risk_analyser.Data.Model.Entities.MitagationAction", null)
                        .WithMany()
                        .HasForeignKey("MitagationsMitagatioActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ControlRiskRisk", b =>
                {
                    b.HasOne("Risk_analyser.Data.Model.Entities.ControlRisk", null)
                        .WithMany()
                        .HasForeignKey("ControlsControlRiskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Risk_analyser.Data.Model.Entities.Risk", null)
                        .WithMany()
                        .HasForeignKey("RisksRiskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.Asset", b =>
                {
                    b.HasOne("Risk_analyser.Data.Model.Entities.FRAPAnalysis", "FRAPAnalysis")
                        .WithOne("Asset")
                        .HasForeignKey("Risk_analyser.Data.Model.Entities.Asset", "FRAPAnalysisId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Risk_analyser.Data.Model.Entities.RhombusAnalysis", "RhombusAnalysis")
                        .WithOne("Asset")
                        .HasForeignKey("Risk_analyser.Data.Model.Entities.Asset", "RhombusAnalysisId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FRAPAnalysis");

                    b.Navigation("RhombusAnalysis");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.Rhombus", b =>
                {
                    b.HasOne("Risk_analyser.Data.Model.Entities.Risk", "Risk")
                        .WithOne("RhombusParams")
                        .HasForeignKey("Risk_analyser.Data.Model.Entities.Rhombus", "RiskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Risk");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.Risk", b =>
                {
                    b.HasOne("Risk_analyser.Data.Model.Entities.Asset", "Asset")
                        .WithMany("Risks")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.RiskType", b =>
                {
                    b.HasOne("Risk_analyser.Data.Model.Entities.Risk", "Risk")
                        .WithMany("Types")
                        .HasForeignKey("RiskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Risk");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.FRAPDocument", b =>
                {
                    b.HasOne("Risk_analyser.Data.Model.Entities.FRAPAnalysis", "FRAPAnalysis")
                        .WithMany("Results")
                        .HasForeignKey("FRAPAnalysisId");

                    b.Navigation("FRAPAnalysis");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.RhombusDocument", b =>
                {
                    b.HasOne("Risk_analyser.Data.Model.Entities.RhombusAnalysis", "RhombusAnalysis")
                        .WithMany("Results")
                        .HasForeignKey("RhombusAnalysisId");

                    b.Navigation("RhombusAnalysis");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.Asset", b =>
                {
                    b.Navigation("Risks");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.FRAPAnalysis", b =>
                {
                    b.Navigation("Asset")
                        .IsRequired();

                    b.Navigation("Results");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.RhombusAnalysis", b =>
                {
                    b.Navigation("Asset")
                        .IsRequired();

                    b.Navigation("Results");
                });

            modelBuilder.Entity("Risk_analyser.Data.Model.Entities.Risk", b =>
                {
                    b.Navigation("RhombusParams")
                        .IsRequired();

                    b.Navigation("Types");
                });
#pragma warning restore 612, 618
        }
    }
}
