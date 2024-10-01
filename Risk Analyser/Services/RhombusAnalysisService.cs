using iTextSharp.text.pdf;
using OxyPlot;
using OxyPlot.Wpf;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.Data.Repository;
using Risk_analyser.services;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using Risk_analyser.Data.Model;
using System.Windows.Input;
using Risk_analyser.View;
using Microsoft.Win32;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

using DocumentFormat.OpenXml.Drawing;

using DocumentFormat.OpenXml.ExtendedProperties;
using iTextSharp.text;
using Path = System.IO.Path;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;

using Body = DocumentFormat.OpenXml.Wordprocessing.Body;
using PageSize = DocumentFormat.OpenXml.Wordprocessing.PageSize;
using ParagraphProperties = DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties;
using DataPoint = OxyPlot.DataPoint;
using DocumentFormat.OpenXml.Drawing.Charts;




namespace Risk_analyser.Services
{
    public class RhombusAnalysisService
    {
        private RhombusAnalysisRepository RhombusAnalysisRepository;
        private RhombusDocumentRepository RhombusDocumentRepository;
        private RiskService RiskService { get; set; }
        private AssetService AssetService { get; set; }

        public RhombusAnalysisService(RhombusAnalysisRepository rhombusAnalysisRepository, RhombusDocumentRepository rhombusDocumentRepository)
        {
            RhombusAnalysisRepository = rhombusAnalysisRepository;
            RhombusDocumentRepository = rhombusDocumentRepository;
        }
        public void SetAssetService()
        {
            AssetService = MainWindowService.GetAssetService();
        }
        public void SetRiskService()
        {
            RiskService = MainWindowService.GetRiskService();
        }

        public bool AnyRhombusAnalysis(Asset asset)
        {
            return RhombusAnalysisRepository.GetRhombusAnalysesForAsset(asset).Any();
        }
        public bool OlderThanAllRhombusInAsset(Asset asset, object p)
        {
            Risk risk= p as Risk;
            List<RhombusDocument> RhombusDocuments = RhombusAnalysisRepository.GetRhombusAnalysesForAsset(asset);
            if (p as Risk != null)
            {
                risk = MainWindowService.RiskService.LoadRiskWithEntities(risk.RiskId);
                foreach (RhombusDocument r in RhombusDocuments)
                {
                    if (r.CreationTime > risk.CreationTime)
                    {
                        return true;
                    }
                }
            }
            else if(p as ControlRisk != null)
            {
                ControlRisk control = p as ControlRisk;
                control = MainWindowService.ControlRiskService.LoadControlRiskWithEntities(control.ControlRiskId);
                foreach (RhombusDocument r in RhombusDocuments)
                {
                    if (r.CreationTime > control.CreationDate)
                    {
                        return true;
                    }
                }

            }
            else if(p as MitagationAction != null)
            {
                MitagationAction mit = p as MitagationAction;
                mit = MainWindowService.mitigationActionService.LoadMitigationWithEntities(mit.MitagatioActionId);
                foreach (RhombusDocument r in RhombusDocuments)
                {
                    if (r.CreationTime > mit.CreationTime)
                    {
                        return true;
                    }
                }
            }
            //return RhombusDocuments.Count()==0?false:true;
          
           return false;
        }
        public ObservableCollection<RhombusAnalysisDataGridItem> GetRhombusDocuments(Asset asset, ICommand? SaveLocally, ICommand? DeleteAnalyse,
            bool InitButtons)
        {
            List<RhombusDocument> Documents = RhombusAnalysisRepository.GetRhombusAnalysesForAsset(asset);
            ObservableCollection<RhombusAnalysisDataGridItem> rhombusAnalysisDataGridItems = new ObservableCollection<RhombusAnalysisDataGridItem>();
            foreach (RhombusDocument document in Documents)
            {
                RhombusAnalysisDataGridItem DataGridItem = new RhombusAnalysisDataGridItem()
                {
                    DocumentId = document.DocumentId,
                    FileName = document.FileName,
                    CreationTime = document.CreationTime.ToString("g"),
                    SaveLocally = InitButtons ? SaveLocally : null,
                    DeleteAnalyse = InitButtons ? DeleteAnalyse : null
                };
                rhombusAnalysisDataGridItems.Add(DataGridItem);
            }
            return rhombusAnalysisDataGridItems;
        }
        public void DeleteRhombusAnalysis(long id)
        {
            bool Status = RhombusDocumentRepository.DeleteDocument(id);
            if (Status)
            {
                MessageBox.Show("Analiza została pomyślnie usunięta", "Usuwanie Analizy", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Nie można usunąć analizy ponieważ wystapił wyjątek: " + RhombusDocumentRepository.OccuredException.ToString(), "Usuwanie Analizy", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SaveLocalToPDF(long id)
        {
            RhombusDocument rhombusDocument = RhombusDocumentRepository.LoadRhombusDocumentWithEntities(id);
            SaveFileDialog WindowsSaveFileDialog = new SaveFileDialog();
            WindowsSaveFileDialog.DefaultExt = ".pdf";
            WindowsSaveFileDialog.Filter = "PDF Document (*.pdf)|*.pdf|All files (*.*)|*.*";
            string path = null;
            if (WindowsSaveFileDialog.ShowDialog() == true)
            {
                path = WindowsSaveFileDialog.FileName;
            }
            File.WriteAllBytes(path, rhombusDocument.FileData);

            MessageBox.Show("Analiza została pomyślnie zapisana w " + path, "Zapisz Analize do PDF", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public IList<DataPoint>? CalculateValuesFromRhombusParams(Asset SelectedAsset)
        {
            double TechnologyAverage = 0;
            double TechnologyWeightSum = 0;
            double InnovationAverage = 0;
            double InnovationWeightSum = 0;
            double ComplexityAverage = 0;
            double ComplexityWeightSum = 0;
            double RateAverage = 0;
            double RateWeightSum = 0;
            SelectedAsset = AssetService.LoadAssetWithEntities(SelectedAsset.AssetId);
            List<Risk> risks = RiskService.GetRisksForAsset(SelectedAsset);
            foreach (Risk r in risks)
            {
                Risk risk = RiskService.LoadRiskWithEntities(r.RiskId);

                int technologyValue = (int)risk.RhombusParams.Technology;
                TechnologyAverage = TechnologyAverage + (technologyValue * technologyValue);
                TechnologyWeightSum = TechnologyWeightSum + technologyValue;

                int innovationValue = (int)risk.RhombusParams.Inovation;
                InnovationAverage = InnovationAverage + (innovationValue * innovationValue);
                InnovationWeightSum = InnovationWeightSum + innovationValue;

                int complexityValue = (int)risk.RhombusParams.Complexity;
                ComplexityAverage = ComplexityAverage + (complexityValue * complexityValue);
                ComplexityWeightSum = ComplexityWeightSum + complexityValue;

                int rateValue = (int)risk.RhombusParams.Rate;
                RateAverage = RateAverage + (rateValue * rateValue);
                RateWeightSum = RateWeightSum + rateValue;
            }
            IList<DataPoint> Points = new List<DataPoint>();
            Points.Add(new DataPoint(0, Math.Round(TechnologyAverage / TechnologyWeightSum, MidpointRounding.AwayFromZero)));
            Points.Add(new DataPoint(Math.Round(InnovationAverage / InnovationWeightSum, MidpointRounding.AwayFromZero), 0));
            Points.Add(new DataPoint(0, -Math.Round(RateAverage / RateWeightSum, MidpointRounding.AwayFromZero)));
            Points.Add(new DataPoint(-Math.Round(ComplexityAverage / ComplexityWeightSum, MidpointRounding.AwayFromZero), 0));

            return Points;
        }
        public void SaveAnalysisToPDF(PlotModel plotModel, string PDFpath)
        {
            string tempFolder = System.IO.Path.GetTempPath();
            string tempPngPath = System.IO.Path.Combine(tempFolder, "temporary_plotDiagram.png");

            try
            {
                using (var stream = File.Create(tempPngPath))
                {
                    var pngExporter = new PngExporter { Width = 1200, Height = 1000 };
                    pngExporter.Export(plotModel, stream);
                }
                SavePngToPDF(tempPngPath, PDFpath);
                if (File.Exists(tempPngPath))
                {
                    File.Delete(tempPngPath);
                }
                MessageBox.Show("Analiza została zapisana w : " + PDFpath, "Analiza Romboidalna - Zapis do PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystapił wyjątek: " + ex, "Analiza Romboidalna - Zapis do PDF", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void SavePngToPDF(string tempPngPath, string PDFpath)
        {
            try
            {
                using (FileStream fs = new FileStream(PDFpath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate());
                    PdfWriter.GetInstance(document, fs);
                    document.Open();
                    document.SetMargins(0,5f,0,5f);
                    var pngImage = Image.GetInstance(tempPngPath);

                    //pngImage.ScaleToFit(iTextSharp.text.PageSize.A4.Width-20 , iTextSharp.text.PageSize.A4.Height-20 );
                    pngImage.ScaleAbsolute(iTextSharp.text.PageSize.A4.Height - 50, iTextSharp.text.PageSize.A4.Width - 50);
                    pngImage.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    //pngImage.SetAbsolutePosition()

                    document.Add(pngImage);
                    document.Close();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystapił wyjątek: " + ex, "Analiza Romboidalna - Zapis do PDF", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public void SaveAnalysisToDB(PlotModel plotModel, Asset asset, DateTime creationTime)
        {
            string tempFolder = Path.GetTempPath();
            string tempPngPath = Path.Combine(tempFolder, "temporary_plotDiagram.png");
            string tempPDFPath = Path.Combine(tempFolder, "temporary_plotDiagram.pdf");
            try
            {
                using (var stream = File.Create(tempPngPath))
                {
                    var pngExporter = new PngExporter { Width = 1200, Height = 1000 };
                    pngExporter.Export(plotModel, stream);
                }
                SavePngToPDF(tempPngPath, tempPDFPath);
                if (File.Exists(tempPngPath))
                {
                    File.Delete(tempPngPath);
                }
                RhombusDocument rhombusAnalysis = new RhombusDocument();
                rhombusAnalysis.CreationTime = creationTime;
                rhombusAnalysis.FileName = "AnalizaRombo-" + rhombusAnalysis.DocumentId + "-" + asset.Name + "-" + creationTime.ToString("g");
                using (var stream = File.Open(tempPDFPath, FileMode.Open))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        rhombusAnalysis.FileData = memoryStream.ToArray();
                    }
                }
                asset.RhombusAnalysis.Results.Add(rhombusAnalysis);
                AssetService.EditAsset(asset);
                if (File.Exists(tempPDFPath))
                {
                    File.Delete(tempPDFPath);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystapił wyjątek: " + ex, "Analiza Romboidalna - Zapis do PDF", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public void SaveAnalysisToDocx(string path, Asset asset, PlotModel plotModel)
        {
            string tempFolder = Path.GetTempPath();
            string tempPngPath = Path.Combine(tempFolder, "temporary_plotDiagram.png");

            // Eksportuj obraz do pliku PNG
            using (var stream = File.Create(tempPngPath))
            {
                var pngExporter = new PngExporter { Width = 1200, Height = 1000 };
                pngExporter.Export(plotModel, stream);
            }
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(path, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                // Initialize the document and body
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Set page orientation to landscape
                SectionProperties sectionProperties = new SectionProperties();
                PageSize pageSize = new PageSize()
                {
                    Width = 16838,  // 11.69 inches (A4 width in landscape mode)
                    Height = 11906, // 8.27 inches (A4 height in landscape mode)
                    Orient = PageOrientationValues.Landscape
                };
                sectionProperties.Append(pageSize);
                PageMargin pageMargin = new PageMargin() { Top = 360, Right = (UInt32Value)0U, Bottom = 0, Left = (UInt32Value)0U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
                sectionProperties.Append(pageMargin);
                //mainPart.Document.Body.Append(sectionProps);
                // Append SectionProperties to the body
                body.Append(sectionProperties);

                // Add new image part to MainDocumentPart
                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Png);

                // Load image from file
                using (FileStream stream = new FileStream(tempPngPath, FileMode.Open))
                {
                    imagePart.FeedData(stream);
                }

                // Get the image part ID
                string imagePartId = mainPart.GetIdOfPart(imagePart);

                // Pass the MainDocumentPart to AddImageToBody
                AddImageToBody(mainPart, imagePartId);

                // Remove headers and footers
                RemoveHeadersAndFooters(mainPart);

                // Save the changes to the document
                mainPart.Document.Save();
            }

        }
        private void RemoveHeadersAndFooters(MainDocumentPart mainPart)
        {
            // Remove headers
            foreach (var headerPart in mainPart.HeaderParts.ToList())
            {
                mainPart.DeletePart(headerPart);
            }

            // Remove footers
            foreach (var footerPart in mainPart.FooterParts.ToList())
            {
                mainPart.DeletePart(footerPart);
            }
        }


        private static void AddImageToBody(MainDocumentPart wordDoc, string relationshipId)
        {
            // Tworzenie elementu rysunku dla obrazu
            var element =
                 new Drawing(
                     new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
                         new DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent() { Cx = 28*360000L, Cy = 20*360000L }, // rozmiar w EMU
                         new DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Obraz 1"
                         },
                         new DocumentFormat.OpenXml.Drawing.Graphic(
                             new DocumentFormat.OpenXml.Drawing.GraphicData(
                                 new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                                     new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                         new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "Nowy Obraz"
                                         },
                                         new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()),
                                     new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                         new DocumentFormat.OpenXml.Drawing.Blip(
                                             new DocumentFormat.OpenXml.Drawing.BlipExtensionList(
                                                 new DocumentFormat.OpenXml.Drawing.BlipExtension()
                                                 {
                                                     Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 }))
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print
                                         },
                                         new DocumentFormat.OpenXml.Drawing.Stretch(
                                             new DocumentFormat.OpenXml.Drawing.FillRectangle())),
                                     new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
                                         new DocumentFormat.OpenXml.Drawing.Transform2D(
                                             new DocumentFormat.OpenXml.Drawing.Offset() { X = 0L, Y = 0L },
                                             new DocumentFormat.OpenXml.Drawing.Extents() { Cx = 28 * 360000L, Cy = 20 * 360000L }),
                                         new DocumentFormat.OpenXml.Drawing.PresetGeometry(
                                             new DocumentFormat.OpenXml.Drawing.AdjustValueList())
                                         { Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle })))
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }))
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U
                     });
            var paragraph = new Paragraph(
        new ParagraphProperties(
            new Justification() { Val = JustificationValues.Center } // Center the paragraph
        ),
        new Run(element) // Add the image inside the Run
    );
            // Dodaj obrazek jako nowy paragraf w treści dokumentu
            wordDoc.Document.Body.AppendChild(paragraph);
        }
    }
}


