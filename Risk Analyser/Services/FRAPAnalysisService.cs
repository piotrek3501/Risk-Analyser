using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Risk_analyser.Data;
using Risk_analyser.Data.DBContext;
using Risk_analyser.Data.Model;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using Risk_analyser.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DocumentFormat.OpenXml.Wordprocessing;
using WordParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Font = iTextSharp.text.Font;
using Paragraph = iTextSharp.text.Paragraph;
using DocumentFormat.OpenXml.Spreadsheet;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TopBorder = DocumentFormat.OpenXml.Wordprocessing.TopBorder;
using BottomBorder = DocumentFormat.OpenXml.Wordprocessing.BottomBorder;
using LeftBorder = DocumentFormat.OpenXml.Wordprocessing.LeftBorder;
using RightBorder = DocumentFormat.OpenXml.Wordprocessing.RightBorder;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;

namespace Risk_analyser.Services
{
    public class FRAPAnalysisService
    {
        private readonly FRAPAnalysisRepository FRAPAnalysisRepository;
        private readonly FRAPDocumentRepository FRAPDocumentRepository;
        private AssetService _assetService;

        public FRAPAnalysisService(FRAPAnalysisRepository fRAPAnalysisRepository,FRAPDocumentRepository fRAPDocumentRepository)
        {
            FRAPAnalysisRepository = fRAPAnalysisRepository;
            FRAPDocumentRepository= fRAPDocumentRepository;
            _assetService=MainWindowService.AssetService;
        }

        public bool AnyFRAPAnalysis(Asset asset)
        {
            return FRAPAnalysisRepository.GetAllFRAPAnalysisForAsset(asset).Any();

        }
        public bool OlderThanAllFRAPsInAsset(Asset asset,object p)
        {
            List<FRAPDocument> FRAPsDocuments = FRAPAnalysisRepository.GetAllFRAPAnalysisForAsset(asset);
            Risk risk = p as Risk;
            if (p as Risk != null)
            {
                risk = MainWindowService.RiskService.LoadRiskWithEntities(risk.RiskId);
                foreach (FRAPDocument FRAPDocument in FRAPsDocuments)
                {
                    if (FRAPDocument.CreationTime > risk.CreationTime)
                    {
                        return true;
                    }
                }
            }
            else if (p as ControlRisk != null)
            {
                ControlRisk control = p as ControlRisk;
                control = MainWindowService.ControlRiskService.LoadControlRiskWithEntities(control.ControlRiskId);
                foreach (FRAPDocument FRAPDocument in FRAPsDocuments)
                {
                    if (FRAPDocument.CreationTime > control.CreationDate)
                    {
                        return true;
                    }
                }

            }
            else if (p as MitagationAction != null)
            {
                MitagationAction mit = p as MitagationAction;
                mit = MainWindowService.mitigationActionService.LoadMitigationWithEntities(mit.MitagatioActionId);
                foreach (FRAPDocument FRAPDocument in FRAPsDocuments)
                {
                    if (FRAPDocument.CreationTime > mit.CreationTime)
                    {
                        return true;
                    }
                }
            }
            //return FRAPsDocuments.Count==0?false:true;
            return false;
          
        }

        public FRAPDocument MakeAnalyse(Asset selectedAsset)
        {
            selectedAsset= _assetService.LoadAssetWithEntities(selectedAsset.AssetId);
            string tempFolder = Path.GetTempPath();
            string tempPDFPath = Path.Combine(tempFolder, "temporary_FRAPAnalyse.pdf");

            DateTime creationTime = DateTime.Now;
            FRAPDocument fRAPDocument = new FRAPDocument()
            {
                FileName= "AnalizaFRAP_"+selectedAsset.Name+"_"+creationTime.ToString("g"),
                CreationTime=creationTime
            };
            SaveAnalyseToPDF(selectedAsset,tempPDFPath);

            using(var stream = File.Open(tempPDFPath, FileMode.Open))
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    fRAPDocument.FileData = memoryStream.ToArray();
                }
            }
            SaveFRAPAnalyseToDB(selectedAsset,fRAPDocument);
            return fRAPDocument;
        }

        public void SaveAnalyseLocalyToDocsFile(long? fRAPAnalysisId,string path)
        {
            FRAPDocument fRAPDocument = FRAPDocumentRepository.LoadFRAPDocumentWithEntities(fRAPAnalysisId);
            Asset asset = fRAPDocument.FRAPAnalysis.Asset;
            asset=_assetService.LoadAssetWithEntities(asset.AssetId);

            List<Risk> Risks = asset.Risks;
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                Body body = new Body();

                WordParagraph title = new WordParagraph();
                ParagraphProperties paragraphProperties = new ParagraphProperties();

                // Ustawienie odstępów między wierszami
                SpacingBetweenLines spacing = new SpacingBetweenLines();

                Run run = new Run();
                RunProperties runProperties = new RunProperties();
                RunFonts runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                FontSize fontSize = new FontSize() { Val = "36" };
                runProperties.Append(runFont);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Analiza FRAP dla:"));
                title.Append(run);
                body.Append(title);

                WordParagraph subTitle = new WordParagraph();
                runProperties = new RunProperties();
                run = new Run();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "32" };
                runProperties.Append(runFont);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text(asset.Name));
                subTitle.Append(run);
                body.Append(subTitle);

                WordParagraph Section1 = new WordParagraph();
                 paragraphProperties = new ParagraphProperties();

                // Ustawienie odstępów między wierszami
                 spacing = new SpacingBetweenLines()
                {
                    Before = "720", // Odstęp przed paragrafem (wartość w jednostkach Dxa, 240 to około 0.17 cala)
                    After = "240",  // Odstęp po paragrafie (również w Dxa)
                    Line = "480",   // Odstęp między wierszami w paragrafie (480 Dxa to podwójna interlinia)
                    LineRule = LineSpacingRuleValues.Auto // Automatyczne obliczenie odstępów między wierszami
                };

                // Dodanie właściwości odstępów do paragrafu
                paragraphProperties.Append(spacing);
                Section1.Append(paragraphProperties);

                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                  HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "32" };
                runProperties.Append(runFont);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Wykaz ryzyk dla zasobu"));
                Section1.Append(run);
                body.Append(Section1);

                Table RiskTable = new Table();
                TableProperties tableProperties = new TableProperties();
                TableBorders tableBorders = new TableBorders(
                         new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Górna krawędź
                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Dolna krawędź
                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Lewa krawędź
                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Prawa krawędź
                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },  // Wewnętrzne poziome krawędzie
                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 }   // Wewnętrzne pionowe krawędzie
                );
                tableProperties.Append(tableBorders);
                TableLayout tableLayout = new TableLayout() { Type = TableLayoutValues.Fixed };  // Wyłączenie automatycznego dopasowania kolumn

                // Ustawienie szerokości tabeli na 100%
                TableWidth tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };
                tableProperties.Append(tableWidth);
                tableProperties.Append(tableLayout);
                TableLook tableLook = new TableLook() { FirstRow = true, NoHorizontalBand = false, NoVerticalBand = true };
                tableProperties.Append(tableLook);
                RiskTable.AppendChild(tableProperties);

                TableRow riskTableRowHeaders = new TableRow();

                WordParagraph riskLpHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                Bold bold = new Bold();
                runProperties.Append(runFont);
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Lp"));
                riskLpHeaderParagraph.Append(run);
                TableCell riskLpHeader = new TableCell(riskLpHeaderParagraph);

                WordParagraph riskNameHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Nazwa"));
                riskNameHeaderParagraph.Append(run);
                TableCell riskNameHeader = new TableCell(riskNameHeaderParagraph);

                TableCellProperties cellProperties = new TableCellProperties();
                TableCellWidth cellWidth = new TableCellWidth() { Width = "1567", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                SpacingBetweenLines spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                Justification justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                TableCellMargin cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "100", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                NoWrap noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                riskNameHeader.Append(cellProperties);

                WordParagraph riskTypeHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Typ"));
                riskTypeHeaderParagraph.Append(run);
                TableCell riskTypeHeader = new TableCell(riskTypeHeaderParagraph);

                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "567", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "100", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                riskTypeHeader.Append(cellProperties);

                WordParagraph riskPriorityHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Priorytet"));
                riskPriorityHeaderParagraph.Append(run);
                TableCell riskPriorityHeader = new TableCell(riskPriorityHeaderParagraph);

                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "730", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "100", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                riskPriorityHeader.Append(cellProperties);

                WordParagraph riskControlsHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Środki łagodzące"));
                riskControlsHeaderParagraph.Append(run);
                TableCell riskControlsHeader = new TableCell(riskControlsHeaderParagraph);

                 cellProperties = new TableCellProperties();
                 cellWidth = new TableCellWidth() { Width = "1000", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                 paragraphProperties = new ParagraphProperties();
                 spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                 justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                 cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "100", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "100", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                 noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                riskControlsHeader.Append(cellProperties);

                riskTableRowHeaders.Append(riskLpHeader, riskNameHeader, riskTypeHeader, riskPriorityHeader, riskControlsHeader);
                RiskTable.Append(riskTableRowHeaders);
                RiskService riskService = MainWindowService.GetRiskService();
                int i;
                foreach (Risk r in Risks)
                {
                    TableRow RiskDataRow = new TableRow();
                    Risk risk = riskService.LoadRiskWithEntities(r.RiskId);

                    WordParagraph riskDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(risk.RiskId.ToString()));
                    riskDataRowParagraph.Append(run);
                    TableCell riskLpCell = new TableCell(riskDataRowParagraph);

                    riskDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(risk.Name));
                    riskDataRowParagraph.Append(run);
                    TableCell riskNameCell = new TableCell(riskDataRowParagraph);

                    string TypesOfRisk = "";
                    i = 0;
                    foreach (RiskType t in risk.Types)
                    {
                        i++;
                        if (i == 1 && t.RiskTypeId != risk.Types.Last().RiskTypeId)
                        {
                            TypesOfRisk += t.Type.ToString() + ",";
                        }
                        else if (t.RiskTypeId != risk.Types.Last().RiskTypeId)
                        {
                            TypesOfRisk += t.Type.ToString() + ",";
                        }
                        else if (t.RiskTypeId == risk.Types.Last().RiskTypeId)
                        {
                            TypesOfRisk += t.Type.ToString();
                        }
                    }
                    riskDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(TypesOfRisk));
                    riskDataRowParagraph.Append(run);
                    TableCell riskTypeCell = new TableCell(riskDataRowParagraph);

                    riskDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(risk.Priority.ToString()));
                    riskDataRowParagraph.Append(run);
                    TableCell riskPriorityCell = new TableCell(riskDataRowParagraph);

                    i = 0;
                    string Controls = "";
                    foreach (ControlRisk c in risk.Controls)
                    {
                        i++;
                        if (i == 1 && c.ControlRiskId != risk.Controls.Last().ControlRiskId)
                        {
                            Controls += c.Name + ",";
                        }
                        else if (i % 4 == 0)
                        {
                            Controls += "\n" + c.Name + ",";
                        }
                        else if (c.ControlRiskId != risk.Controls.Last().ControlRiskId)
                        {
                            Controls += c.Name + ",";
                        }
                        else if (c.ControlRiskId == risk.Controls.Last().ControlRiskId)
                        {
                            Controls += c.Name;

                        }
                    }
                    riskDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(Controls));
                    riskDataRowParagraph.Append(run);
                    TableCell riskControlsCell = new TableCell(riskDataRowParagraph);
                    RiskDataRow.Append(riskLpCell,riskNameCell,riskTypeCell,riskPriorityCell,riskControlsCell);
                    RiskTable.Append(RiskDataRow);
                }
                body.Append(RiskTable);

                WordParagraph Section2 = new WordParagraph();

                paragraphProperties = new ParagraphProperties();

                // Ustawienie odstępów między wierszami
                spacing = new SpacingBetweenLines()
                {
                    Before = "480", // Odstęp przed paragrafem (wartość w jednostkach Dxa, 240 to około 0.17 cala)
                    After = "240",  // Odstęp po paragrafie (również w Dxa)
                    Line = "480",   // Odstęp między wierszami w paragrafie (480 Dxa to podwójna interlinia)
                    LineRule = LineSpacingRuleValues.Auto // Automatyczne obliczenie odstępów między wierszami
                };

                // Dodanie właściwości odstępów do paragrafu
                paragraphProperties.Append(spacing);
                Section2.Append(paragraphProperties);

                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "32" };
                runProperties.Append(runFont);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Wykaz środków łagodzących dla zagrożeń"));
                Section2.Append(run);
                body.Append(Section2);

                Table ControlsTable = new Table();
                tableProperties = new TableProperties();
                 tableBorders = new TableBorders(
                      new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Górna krawędź
                     new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Dolna krawędź
                     new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Lewa krawędź
                     new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Prawa krawędź
                     new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },  // Wewnętrzne poziome krawędzie
                     new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 }   // Wewnętrzne pionowe krawędzie
             );
                tableProperties.Append(tableBorders);
                tableLayout = new TableLayout() { Type = TableLayoutValues.Fixed };  // Wyłączenie automatycznego dopasowania kolumn

                // Ustawienie szerokości tabeli na 100%
                tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };

                tableProperties.Append(tableWidth);
                tableProperties.Append(tableLayout);
                tableLook = new TableLook() { FirstRow = true, NoHorizontalBand = false, NoVerticalBand = true };
                tableProperties.Append(tableLook);
                ControlsTable.AppendChild(tableProperties);

                // Ustawienie szerokości tabeli na 100%

                TableRow controlTableRowHeaders = new TableRow();

                WordParagraph controlLpHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                 bold = new Bold();
                runProperties.Append(runFont);
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Lp"));
               
                controlLpHeaderParagraph.Append(run);
                TableCell controlLpHeader = new TableCell(controlLpHeaderParagraph);

                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "100", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "10", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "10", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                controlLpHeader.Append(cellProperties);

                WordParagraph controlNameHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Nazwa"));
                controlNameHeaderParagraph.Append(run);
                TableCell controlNameHeader = new TableCell(controlNameHeaderParagraph);

                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "2000", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "10", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "10", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                controlNameHeader.Append(cellProperties);

                WordParagraph controlDescriptionHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Opis"));
                controlDescriptionHeaderParagraph.Append(run);
                TableCell controlDescriptionHeader = new TableCell(controlDescriptionHeaderParagraph);
                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "2000", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "10", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "10", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                controlDescriptionHeader.Append(cellProperties);

                controlTableRowHeaders.Append(controlLpHeader, controlNameHeader, controlDescriptionHeader);
                ControlsTable.Append(controlTableRowHeaders);
                ControlRiskService controlRiskService = MainWindowService.GetControlRiskService();
                List<ControlRisk> controlRisks = controlRiskService.GetAllUniqueControlRiskForRisks(asset.Risks);
                i = 0;
                foreach (ControlRisk c in controlRisks)
                {
                    TableRow ControlDataRow = new TableRow();

                    WordParagraph controlLpDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(c.ControlRiskId.ToString()));
                    controlLpDataRowParagraph.Append(run);
                    TableCell controlLpCell = new TableCell(controlLpDataRowParagraph);

                    WordParagraph controlNameDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(c.Name));
                    controlNameDataRowParagraph.Append(run);
                    TableCell controlNameCell = new TableCell(controlNameDataRowParagraph);

                    WordParagraph controlDescriptionDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(c.Description));
                    controlDescriptionDataRowParagraph.Append(run);
                    TableCell controlDescriptionCell = new TableCell(controlDescriptionDataRowParagraph);
                    ControlDataRow.Append(controlLpCell, controlNameCell, controlDescriptionCell);
                    ControlsTable.Append(ControlDataRow);
                }
                body.Append(ControlsTable);


                WordParagraph Section3 = new WordParagraph();
                paragraphProperties = new ParagraphProperties();

                // Ustawienie odstępów między wierszami
                spacing = new SpacingBetweenLines()
                {
                    Before = "480", // Odstęp przed paragrafem (wartość w jednostkach Dxa, 240 to około 0.17 cala)
                    After = "240",  // Odstęp po paragrafie (również w Dxa)
                    Line = "480",   // Odstęp między wierszami w paragrafie (480 Dxa to podwójna interlinia)
                    LineRule = LineSpacingRuleValues.Auto // Automatyczne obliczenie odstępów między wierszami
                };

                // Dodanie właściwości odstępów do paragrafu
                paragraphProperties.Append(spacing);
                Section3.Append(paragraphProperties);

                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "32" };
                runProperties.Append(runFont);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Wykaz akcji dla środków łagodzących"));
                Section3.Append(run);
                body.Append(Section3);

                Table MitigationTable = new Table();
                tableProperties = new TableProperties();
                 tableBorders = new TableBorders(
                      new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Górna krawędź
                     new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Dolna krawędź
                     new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Lewa krawędź
                     new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },   // Prawa krawędź
                     new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },  // Wewnętrzne poziome krawędzie
                     new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 }   // Wewnętrzne pionowe krawędzie
             );
                tableProperties.Append(tableBorders);
                tableLayout = new TableLayout() { Type = TableLayoutValues.Fixed };  // Wyłączenie automatycznego dopasowania kolumn

                // Ustawienie szerokości tabeli na 100%
                tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };

                tableProperties.Append(tableWidth);
                tableProperties.Append(tableLayout);
                tableLook = new TableLook() { FirstRow = true, NoHorizontalBand = false, NoVerticalBand = true };
                tableProperties.Append(tableLook);
                MitigationTable.AppendChild(tableProperties);


                TableRow MitigationTableRowHeaders = new TableRow();

                WordParagraph MitigationLpHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                bold = new Bold();
                runProperties.Append(runFont);
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Lp"));
                MitigationLpHeaderParagraph.Append(run);
                TableCell MitigationlLpHeader = new TableCell(MitigationLpHeaderParagraph);

                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "100", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "10", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "10", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                MitigationlLpHeader.Append(cellProperties);

                WordParagraph MitigationNameHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Nazwa"));
                MitigationNameHeaderParagraph.Append(run);
                TableCell MitigationNameHeader = new TableCell(MitigationNameHeaderParagraph);

                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "720", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "10", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "10", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                MitigationNameHeader.Append(cellProperties);

                WordParagraph MitigationWorkerHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Przypisana osoba"));
                MitigationWorkerHeaderParagraph.Append(run);
                TableCell MitigationWorkerHeader = new TableCell(MitigationWorkerHeaderParagraph);

                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "720", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "10", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "10", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                MitigationWorkerHeader.Append(cellProperties);

                WordParagraph MitigationDateofActionHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Zrealizowano"));
                MitigationDateofActionHeaderParagraph.Append(run);
                TableCell MitigationDateofActionHeader = new TableCell(MitigationDateofActionHeaderParagraph);

                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "720", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "10", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "10", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                MitigationDateofActionHeader.Append(cellProperties);

                WordParagraph MitigationControlsLpHeaderParagraph = new WordParagraph();
                run = new Run();
                runProperties = new RunProperties();
                runFont = new RunFonts() { Ascii = "Arial",
                    HighAnsi = "Arial",
                    EastAsia = "Arial",
                    ComplexScript = "Arial"
                };
                fontSize = new FontSize() { Val = "28" };
                runProperties.Append(runFont);
                bold = new Bold();
                runProperties.Append(bold);
                runProperties.Append(fontSize);
                run.Append(runProperties);
                run.Append(new Text("Nr środka łagodzącego"));
                MitigationControlsLpHeaderParagraph.Append(run);
                TableCell MitigationControlsLpHeader = new TableCell(MitigationControlsLpHeaderParagraph);

                cellProperties = new TableCellProperties();
                cellWidth = new TableCellWidth() { Width = "720", Type = TableWidthUnitValues.Dxa };  // Ustawienie szerokości komórki
                cellProperties.Append(cellWidth);
                paragraphProperties = new ParagraphProperties();
                spacingBetweenLines = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto };
                justification = new Justification() { Val = JustificationValues.Left };
                paragraphProperties.Append(spacingBetweenLines);
                paragraphProperties.Append(justification);

                // Dodanie marginesów wewnątrz komórki
                cellMargin = new TableCellMargin()
                {
                    LeftMargin = new LeftMargin() { Width = "10", Type = TableWidthUnitValues.Dxa },
                    RightMargin = new RightMargin() { Width = "10", Type = TableWidthUnitValues.Dxa }
                };
                cellProperties.Append(cellMargin);
                noWrap = new NoWrap() { Val = OnOffOnlyValues.Off };  // Ustawienie zawijania tekstu
                cellProperties.Append(noWrap);
                MitigationControlsLpHeader.Append(cellProperties);

                MitigationTableRowHeaders.Append(MitigationlLpHeader, MitigationNameHeader, MitigationWorkerHeader, MitigationDateofActionHeader, MitigationControlsLpHeader);
                MitigationTable.Append(MitigationTableRowHeaders);

                string mitigationActionString = "";
                List<MitagationAction> mitagationActions = controlRiskService.GetAllUniqueMitagationForControls(controlRisks);
                i = 0;
                MitigationActionService mitigationActionService = MainWindowService.GetMitigationActionService();

                foreach(MitagationAction m in mitagationActions)
                {
                    MitagationAction mit = mitigationActionService.LoadMitigationWithEntities(m.MitagatioActionId);
                    TableRow MitigationDataRow = new TableRow();

                    WordParagraph mitigationLpDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(m.MitagatioActionId.ToString()));
                    mitigationLpDataRowParagraph.Append(run);
                    TableCell mitigationLpCell = new TableCell(mitigationLpDataRowParagraph);

                    WordParagraph mitigationActionDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(m.Action));
                    mitigationActionDataRowParagraph.Append(run);
                    TableCell mitigationActionCell = new TableCell(mitigationActionDataRowParagraph);

                    WordParagraph mitigationWorkerDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(m.Person));
                    mitigationWorkerDataRowParagraph.Append(run);
                    TableCell mitigationWorkerCell = new TableCell(mitigationWorkerDataRowParagraph);

                    WordParagraph mitigationDateofActionDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(m.DateOfAction.ToString("dd/MM/yyyy")));
                    mitigationDateofActionDataRowParagraph.Append(run);
                    TableCell mitigationDateofActionCell = new TableCell(mitigationDateofActionDataRowParagraph);

                    List<ControlRisk> controls = mit.ControlRisks;
                    foreach (ControlRisk c in controls)
                    {
                        i++;
                        if (i == 1 && controls.Last().ControlRiskId != c.ControlRiskId)
                        {
                            mitigationActionString += c.ControlRiskId.ToString() + ",";
                        }
                        else if (i % 4 == 0)
                        {
                            mitigationActionString += "\n" + c.ControlRiskId + ",";
                        }
                        else if (controls.Last().ControlRiskId != c.ControlRiskId)
                        {
                            mitigationActionString += c.ControlRiskId.ToString() + ",";
                        }
                        else if (controls.Last().ControlRiskId == c.ControlRiskId)
                        {
                            mitigationActionString += c.ControlRiskId.ToString();

                        }

                    }
                    WordParagraph mitigationLpControlsDataRowParagraph = new WordParagraph();
                    run = new Run();
                    runProperties = new RunProperties();
                    runFont = new RunFonts() { Ascii = "Arial",
                        HighAnsi = "Arial",
                        EastAsia = "Arial",
                        ComplexScript = "Arial"
                    };
                    fontSize = new FontSize() { Val = "24" };
                    runProperties.Append(runFont);
                    runProperties.Append(fontSize);
                    run.Append(runProperties);
                    run.Append(new Text(mitigationActionString));
                    mitigationLpControlsDataRowParagraph.Append(run);
                    TableCell mitigationLpControlsCell = new TableCell(mitigationLpControlsDataRowParagraph);
                    mitigationActionString = "";
                    MitigationDataRow.Append(mitigationLpCell, mitigationActionCell, mitigationWorkerCell, mitigationDateofActionCell, mitigationLpControlsCell);
                    MitigationTable.Append(MitigationDataRow);
                }
                body.Append(MitigationTable);
                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }


        }
        public void SaveLocalToPDF(long id)
        {
            FRAPDocument FrapDocument = FRAPDocumentRepository.LoadFRAPDocumentWithEntities(id);
            SaveFileDialog WindowsSaveFileDialog = new SaveFileDialog();
            WindowsSaveFileDialog.DefaultExt = ".pdf";
            WindowsSaveFileDialog.Filter = "PDF Document (*.pdf)|*.pdf|All files (*.*)|*.*";
            string path = null;
            if (WindowsSaveFileDialog.ShowDialog() == true)
            {
                path = WindowsSaveFileDialog.FileName;
            }
            File.WriteAllBytes(path, FrapDocument.FileData);

            MessageBox.Show("Analiza została pomyślnie zapisana w " + path, "Zapisz Analize do PDF", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        public void SaveAnalyseToPDF(Asset asset,string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                    document.SetMargins(70.87f, 70.87f, 70.87f, 70.87f);

                    PdfWriter.GetInstance(document, fs);
                    document.Open();

                    // Ścieżka do czcionki (np. Arial)
                    string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");

                    // Załaduj czcionkę Arial z kodowaniem UTF-8
                    BaseFont baseFont = BaseFont.CreateFont(arialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font ParagrahFont = new Font(baseFont, 16, Font.NORMAL);
                    Font TitleFont = new Font(baseFont, 18, Font.NORMAL);
                    Font SubTitleFont=new Font(baseFont,16, Font.NORMAL);
                    Font TableHeaderFont = new Font(baseFont, 14, Font.BOLD);
                    Font TableDataFont=new Font(baseFont,12,Font.NORMAL);

                    Paragraph title=new Paragraph("Analiza FRAP dla:",TitleFont);
                   // title.SpacingAfter = 10f;
                    Paragraph subtitle = new Paragraph(asset.Name,SubTitleFont);
                    subtitle.Alignment = Element.ALIGN_LEFT;
                    //subtitle.SpacingAfter = 60f;
                    title.Alignment=Element.ALIGN_LEFT;
                    document.Add(title);
                    document.Add(subtitle);
                    Paragraph Section1 = new Paragraph("Wykaz ryzyk dla zasobu", ParagrahFont);
                    Section1.SpacingBefore = 720 / 20f; // counting DXA to Points
                    Section1.SpacingAfter = 240/20f;
                    Section1.Leading = 480 / 20f;
                    Section1.Alignment=Element.ALIGN_LEFT;
                    document.Add(Section1);
                    PdfPTable risksTable = new PdfPTable(5);
                    float[] RiskTableColumnsWidth = { 0.5f,2f,1f,1.5f,2f};
                    risksTable.SetWidths(RiskTableColumnsWidth);
                    risksTable.HorizontalAlignment = Element.ALIGN_CENTER;
                    risksTable.AddCell(new PdfPCell(new Phrase("Lp", TableHeaderFont)));
                    risksTable.AddCell(new PdfPCell(new Phrase("Nazwa", TableHeaderFont)));
                    risksTable.AddCell(new PdfPCell(new Phrase("Typ", TableHeaderFont)));
                    risksTable.AddCell(new PdfPCell(new Phrase("Priorytet", TableHeaderFont)));
                    risksTable.AddCell(new PdfPCell(new Phrase("Środki łagodzące", TableHeaderFont)));
                    List<Risk>Risks=asset.Risks;
                    RiskService riskService=MainWindowService.GetRiskService();
                    int i = 0;
                    foreach (Risk r in Risks)
                    {
                        Risk risk=riskService.LoadRiskWithEntities(r.RiskId);
                        risksTable.AddCell(new PdfPCell(new Phrase(risk.RiskId.ToString(), TableDataFont)));
                        risksTable.AddCell(new PdfPCell(new Phrase(risk.Name, TableDataFont)));
                        string TypesOfRisk = "";
                         i = 0;
                        foreach (RiskType t in risk.Types)
                        {
                            i++;
                            if (i == 1&& t.RiskTypeId != risk.Types.Last().RiskTypeId)
                            {
                                TypesOfRisk += t.Type.ToString()+",";
                            }
                            else if(t.RiskTypeId!=risk.Types.Last().RiskTypeId)
                            {
                                TypesOfRisk += t.Type.ToString()+",";
                            }
                            else if(t.RiskTypeId == risk.Types.Last().RiskTypeId)
                            {
                                TypesOfRisk += t.Type.ToString();
                            }
                        }
                        i = 0;
                        risksTable.AddCell(new PdfPCell(new Phrase(TypesOfRisk, TableDataFont)));
                        PdfPCell PriorityCell = new PdfPCell(new Phrase(risk.Priority.ToString(), TableDataFont));
                        PriorityCell.HorizontalAlignment = Element.ALIGN_CENTER; 
                        PriorityCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        risksTable.AddCell(PriorityCell);
                        
                        string Controls = "";
                        foreach (ControlRisk c in risk.Controls)
                        {
                            i++;
                            if(i == 1&& c.ControlRiskId != risk.Controls.Last().ControlRiskId)
                            {
                                Controls += c.Name+",";
                            }
                            else if (i % 4 == 0)
                            {
                                Controls += "\n"+c.Name+",";
                            }
                            else if(c.ControlRiskId != risk.Controls.Last().ControlRiskId)
                            {
                                Controls += c.Name + ",";
                            }
                            else if (c.ControlRiskId == risk.Controls.Last().ControlRiskId)
                            {
                                Controls +=  c.Name;

                            }
                        }
                        risksTable.AddCell(new PdfPCell(new Phrase(Controls, TableDataFont)));
                    }
                    document.Add(risksTable);

                    Paragraph Section2 = new Paragraph("Wykaz środków łagodzących dla zagrożeń", ParagrahFont);
                    Section2.SpacingBefore = 480/20f;
                    Section2.SpacingAfter = 240/20f;
                    Section2.Leading = 480 / 20f;
                    Section2.Alignment = Element.ALIGN_LEFT;
                    document.Add(Section2);
                    PdfPTable ControlsTable = new PdfPTable(3);
                    float[] ControlsTableColumnsWidth = {1f,2f,2f };
                    ControlsTable.SetWidths(ControlsTableColumnsWidth);
                    ControlsTable.HorizontalAlignment = Element.ALIGN_CENTER;
                    ControlsTable.AddCell(new PdfPCell(new Phrase("Lp", TableHeaderFont)));
                    ControlsTable.AddCell(new PdfPCell(new Phrase("Nazwa", TableHeaderFont)));
                    ControlsTable.AddCell(new PdfPCell(new Phrase("Opis", TableHeaderFont)));
                    PdfPTable MitigationActions= new PdfPTable(5);
                    float[] MitigationTableColumnsWidth = { 0.5f,2f,2f,2.25f,2.25f };
                    MitigationActions.SetWidths(MitigationTableColumnsWidth);
                    MitigationActions.HorizontalAlignment = Element.ALIGN_CENTER;
                    MitigationActions.AddCell(new PdfPCell(new Phrase("Lp", TableHeaderFont)));
                    MitigationActions.AddCell(new PdfPCell(new Phrase("Akcja", TableHeaderFont)));
                    MitigationActions.AddCell(new PdfPCell(new Phrase("Przypisana osoba", TableHeaderFont)));
                    MitigationActions.AddCell(new PdfPCell(new Phrase("Zrealizowano", TableHeaderFont)));
                    MitigationActions.AddCell(new PdfPCell(new Phrase("Lp środka łagodzącego", TableHeaderFont)));
                    ControlRiskService controlRiskService=MainWindowService.GetControlRiskService();
                    List<ControlRisk> controlRisks = controlRiskService.GetAllUniqueControlRiskForRisks(asset.Risks);
                    foreach(ControlRisk c in controlRisks)
                    {
                        ControlsTable.AddCell(new PdfPCell(new Phrase(c.ControlRiskId.ToString(), TableDataFont)));
                        ControlsTable.AddCell(new PdfPCell(new Phrase(c.Name, TableDataFont)));
                        ControlsTable.AddCell(new PdfPCell(new Phrase(c.Description, TableDataFont)));
                    }
                    string mitigationActionString = "";
                    List<MitagationAction> mitagationActions = controlRiskService.GetAllUniqueMitagationForControls(controlRisks);
                     i = 0;
                    MitigationActionService mitigationActionService=MainWindowService.GetMitigationActionService();
                    foreach(MitagationAction m in mitagationActions)
                    {
                        MitagationAction mit = mitigationActionService.LoadMitigationWithEntities(m.MitagatioActionId);
                        MitigationActions.AddCell(new PdfPCell(new Phrase(mit.MitagatioActionId.ToString(), TableDataFont)));
                        MitigationActions.AddCell(new PdfPCell(new Phrase(mit.Action, TableDataFont)));
                        MitigationActions.AddCell(new PdfPCell(new Phrase(mit.Person, TableDataFont)));
                        MitigationActions.AddCell(new PdfPCell(new Phrase(mit.DateOfAction.ToString("dd/MM/yyyy"), TableDataFont)));
                        List<ControlRisk>controls=mit.ControlRisks;
                        foreach(ControlRisk c in controls)
                        {
                            i++;
                            if (i == 1&& controls.Last().ControlRiskId != c.ControlRiskId)
                            {
                                mitigationActionString += c.ControlRiskId.ToString()+",";
                            }
                            else if (i % 4 == 0)
                            {
                                mitigationActionString += "\n"+c.ControlRiskId+",";
                            }
                            else if(controls.Last().ControlRiskId != c.ControlRiskId)
                            {
                                mitigationActionString += c.ControlRiskId.ToString() + ",";
                            }
                            else if (controls.Last().ControlRiskId == c.ControlRiskId)
                            {
                                mitigationActionString += c.ControlRiskId.ToString();

                            }
                        }
                        MitigationActions.AddCell(new PdfPCell(new Phrase(mitigationActionString, TableDataFont)));
                        mitigationActionString = "";

                    }
                    document.Add(ControlsTable);
                    Paragraph Section3 = new Paragraph("Wykaz Akcji dla środków łagodzących", ParagrahFont);
                    Section3.SpacingBefore = 480/20f;
                    Section3.SpacingAfter = 240/20f;
                    Section3.Leading = 480 / 20f;
                    Section3.Alignment=Element.ALIGN_LEFT;
                    document.Add(Section3);
                    document.Add(MitigationActions);

                    // document.Add();
                    document.Close();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystapił wyjątek: " + ex, "Analiza FRAP - Zapis do PDF ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
        public void DeleteFRAPAnalysis(long id)
        {
            bool Status = FRAPDocumentRepository.DeleteDocument(id);
            if (Status)
            {
                MessageBox.Show("Analiza została pomyślnie usunięta", "Usuwanie Analizy", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Nie można usunąć analizy ponieważ wystapił wyjątek: " + FRAPDocumentRepository.OccuredException.ToString(), "Usuwanie Analizy", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ObservableCollection<FRAPAnalysisDataGridItem> GetFRAPDocuments(Asset selectedAsset, ICommand SaveLocally, ICommand DeleteAnalyse, bool InitButtons)
        {
            List<FRAPDocument> Documents = FRAPAnalysisRepository.GetAllFRAPAnalysisForAsset(selectedAsset);
            ObservableCollection<FRAPAnalysisDataGridItem> fRAPAnalysisDataGridItems = new ObservableCollection<FRAPAnalysisDataGridItem>();
            foreach (FRAPDocument document in Documents)
            {
                FRAPAnalysisDataGridItem DataGridItem = new FRAPAnalysisDataGridItem()
                {
                    DocumentId = document.DocumentId,
                    FileName = document.FileName,
                    CreationTime = document.CreationTime.ToString("g"),
                    SaveLocally = InitButtons ? SaveLocally : null,
                    DeleteAnalyse = InitButtons ? DeleteAnalyse : null
                };
                fRAPAnalysisDataGridItems.Add(DataGridItem);
            }
            return fRAPAnalysisDataGridItems;
        }

        private void SaveFRAPAnalyseToDB(Asset asset,FRAPDocument fRAPDocument)
        {
            asset.FRAPAnalysis.Results.Add(fRAPDocument);
            AssetService assetService = MainWindowService.GetAssetService();
            assetService.EditAsset(asset);
        }
    }
}
