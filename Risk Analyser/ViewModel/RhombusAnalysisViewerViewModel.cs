using Risk_analyser.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.ComponentModel;
using Risk_analyser.Services;
using Risk_analyser.Data.Model.Entities;
using Risk_analyser.services;
using OxyPlot.Annotations;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows;
using Microsoft.Win32;
using TickStyle = OxyPlot.Axes.TickStyle;
using HorizontalAlignment = OxyPlot.HorizontalAlignment;
using VerticalAlignment = OxyPlot.VerticalAlignment;
using FontWeights = OxyPlot.FontWeights;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Risk_analyser.ViewModel
{
    public class RhombusAnalysisViewerViewModel : ViewModelBase
    {
        public string Title { get; set; }
        private RhombusAnalysisService rhombusService { get; set; }
        private Asset _selectedAsset {  get; set; }
        private RiskService riskService { get; set; }
        private DateTime _creationTime { get; set; }

        public IList<DataPoint> Points { get; set; }

        private PlotModel _model;

        public PlotModel RhombusAnalysisDiagram
        {
            get => _model;
            set
            {
                _model = value;
                OnPropertyChanged(nameof(RhombusAnalysisDiagram));
            }
        }
        public bool DialogResult { get; private set; }

        public ICommand SaveToDocsFileCommand {  get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand SaveToPDFFileCommand {  get; set; }



        public RhombusAnalysisViewerViewModel(Asset SelectedAsset)
        {
            _selectedAsset= SelectedAsset;
            rhombusService= MainWindowService.GetRhombusAnalysisService();
            SaveToDocsFileCommand = new RelayCommand(_ => SaveToDocsFile(), _ => true);
            SaveToPDFFileCommand=new RelayCommand(_=>SaveToPDFFile(),_=> true);
            CloseWindowCommand=new RelayCommand(_=>CloseWindows(),_ => true);
            riskService= MainWindowService.GetRiskService();
            InitPlot();
            SavePDFToDB();

        }
        private void InitPlot() {

            Points = rhombusService.CalculateValuesFromRhombusParams(_selectedAsset);
            RhombusAnalysisDiagram = new PlotModel
            {
                Title = "Analiza Romboidalna dla: " + _selectedAsset.Name.ToString(),
                Background = OxyColor.Parse("#595959"),
                TitleColor = OxyColor.Parse("#F5F5F5"),
                PlotAreaBackground = OxyColor.Parse("#F5F5F5")
            };
            _creationTime= DateTime.Now;
            var DateAdnotation = new TextAnnotation()
            {
                Text = "Utworzono: " + _creationTime.ToString("g"),
                TextPosition = new DataPoint(2.68, 4.7),
                FontSize = 14,
                TextHorizontalAlignment = HorizontalAlignment.Left,
                TextColor = OxyColors.Black
            };

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "",
                Minimum = -4,
                Maximum = 4,
                PositionAtZeroCrossing = true, // Przecięcie osi Y w punkcie (0,0)
                AxislineStyle = LineStyle.Solid,
                MajorStep = 1,      // Krok głównych podziałek
                MinorStep = 1,    // Krok pomocniczych podziałek
                TickStyle = TickStyle.Inside, // Wyświetlanie podziałek
                LabelFormatter = label => "", // Brak etykiet liczbowych
                TitleFontSize = 14,
                IsZoomEnabled = false,
                IsPanEnabled = false,

            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "",
                Minimum = -4,
                Maximum = 5,
                PositionAtZeroCrossing = true,
                AxislineStyle = LineStyle.Solid,
                MajorStep = 1,      // Krok głównych podziałek
                MinorStep = 1,    // Krok pomocniczych podziałek
                TickStyle = OxyPlot.Axes.TickStyle.Inside, // Wyświetlanie podziałek
                //ShowMinorTicks = true, // Wyświetlanie pomocniczych podziałek
                LabelFormatter = label => "", // Brak etykiet liczbowych
                TitleFontSize = 14,
                IsZoomEnabled = false,
                IsPanEnabled = false,

            };
            RhombusAnalysisDiagram.Axes.Add(xAxis);
            RhombusAnalysisDiagram.Axes.Add(yAxis);

            // Tworzymy serie danych
            var series = new LineSeries();

            // Dodajemy punkty do serii
            series.Points.Add(this.Points.ElementAt(0));
            series.Points.Add(this.Points.ElementAt(1));
            series.Points.Add(this.Points.ElementAt(2));
            series.Points.Add(this.Points.ElementAt(3));
            series.Points.Add(this.Points.ElementAt(0));

            AddNamesForQuarter();

            RhombusAnalysisDiagram.Series.Add(series);
            AddLabelForPoint(0.9, 0.9, "Mało Zaawansowana");
            AddLabelForPoint(1, 1.9, "Średnio Zaawansowana");
            AddLabelForPoint(1, 2.9, "Wysoko Zaawansowana");
            AddLabelForPoint(1.3, 3.9, "Bardzo Wysoko Zaawansowana");

            AddLabelForPoint(1.2, -0.2, "Pochodny");
            AddLabelForPoint(2.2, -0.2, "Platformowy");
            AddLabelForPoint(3.2, -0.2, "Przełomowy");

            AddLabelForPoint(-0.8, -0.2, "Montażowy");
            AddLabelForPoint(-1.8, -0.2, "Systemowy");
            AddLabelForPoint(-2.8, -0.2, "Macieżowy");

            AddLabelForPoint(0.4, -1.1, "Zwykły");
            AddLabelForPoint(0.95, -2.1, "Szybki/Konkurencyjny");
            AddLabelForPoint(0.6, -3.1, "Błyskawiczny");
            RhombusAnalysisDiagram.Annotations.Add(DateAdnotation);


        }
        private void SaveToPDFFile()
        {
            SaveFileDialog WindowsSaveFileDialog = new SaveFileDialog();
            WindowsSaveFileDialog.DefaultExt = ".pdf";
            WindowsSaveFileDialog.Filter = "PDF Document (*.pdf)|*.pdf|All files (*.*)|*.*";
            string path = null;
            if (WindowsSaveFileDialog.ShowDialog() == true)
            {
                path = WindowsSaveFileDialog.FileName;
            }
            rhombusService.SaveAnalysisToPDF(RhombusAnalysisDiagram, path);
            CloseWindows();
        }
        private void SavePDFToDB()
        {
            rhombusService.SaveAnalysisToDB(RhombusAnalysisDiagram,_selectedAsset,_creationTime);
        }

        private void CloseWindows()
        {
            
          foreach (Window window in System.Windows.Application.Current.Windows){

                if (window.DataContext == this)
                {
                    window.DialogResult = DialogResult;
                    window.Close();
                    break;
                }
            }
            
        }

        private void SaveToDocsFile()
        {
            SaveFileDialog WindowsSaveFileDialog= new SaveFileDialog();
            WindowsSaveFileDialog.DefaultExt=".docx";
            WindowsSaveFileDialog.Filter = "Word Document (*.docx)|*.docx|All files (*.*)|*.*";
            string path = null;
            if (WindowsSaveFileDialog.ShowDialog()==true)
            {
                path= WindowsSaveFileDialog.FileName;
            }
            rhombusService.SaveAnalysisToDocx(path,_selectedAsset,RhombusAnalysisDiagram);

        }
        private void  AddNamesForQuarter()
        {
            var title1 = new TextAnnotation
            {
                Text = "Technologia",
                TextPosition = new DataPoint(0.1, 4.5), // Przesunięcie do dolnej prawej ćwiartki
                TextHorizontalAlignment = HorizontalAlignment.Left,
                TextVerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = OxyColors.Transparent,
                StrokeThickness = 0

            };
            var title2 = new TextAnnotation
            {
                Text = "Tempo",
                TextPosition = new DataPoint(0.1, -3.7), // Przesunięcie do dolnej prawej ćwiartki
                TextHorizontalAlignment = HorizontalAlignment.Left,
                TextVerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = OxyColors.Transparent,
                StrokeThickness = 0

            };
            var title3 = new TextAnnotation
            {
                Text = "Złożoność",
                TextPosition = new DataPoint(-3.8, 0.2), // Przesunięcie do dolnej prawej ćwiartki
                TextHorizontalAlignment = HorizontalAlignment.Left,
                TextVerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = OxyColors.Transparent,
                StrokeThickness = 0

            };
            var title4 = new TextAnnotation
            {
                Text = "Innowacyjność",
                TextPosition = new DataPoint(3, 0.2), // Przesunięcie do dolnej prawej ćwiartki
                TextHorizontalAlignment = HorizontalAlignment.Left,
                TextVerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = OxyColors.Transparent,
                StrokeThickness = 0

            };
            RhombusAnalysisDiagram.Annotations.Add(title1);
            RhombusAnalysisDiagram.Annotations.Add(title2);
            RhombusAnalysisDiagram.Annotations.Add(title3);
            RhombusAnalysisDiagram.Annotations.Add(title4);
        }
        private void AddLabelForPoint(double x, double y,string label)
        {
           
            var Annotation = new TextAnnotation
            {
                Text = label,
                TextPosition = new DataPoint(x, y),
                TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Right,
                Stroke = OxyColors.Transparent, // Wyłącza obramowanie
                StrokeThickness = 0,


            };
          
            RhombusAnalysisDiagram.Annotations.Add(Annotation);
        }

 
    }
}

