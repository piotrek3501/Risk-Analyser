using Risk_analyser.Data.Model.Entities;
using Risk_analyser.MVVM;
using Risk_analyser.services;
using Risk_analyser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Risk_analyser.Data.Model.Entities.Rhombus;

namespace Risk_analyser.ViewModel
{
    public class RiskDetailsViewModel :ViewModelBase
    {
        private RiskService _serviceRisk;
        private Risk _selectedRisk { get; set; }
        private string _riskName { get; set; }
        private string _riskDescription { get; set; }
        private DateTime _riskCreationDate { get; set; }
        private bool _riskIntegrity { get; set; }
        private bool _riskConfidenciality { get; set; }
        private bool _riskAvailiability { get; set; }
        private Priority _riskPriority { get; set; }
        private Vulnerability _riskVulnerability { get; set; }
        private Propability _riskPropability { get; set; }
        private PotencialEffect _riskPotencialEffect {  get; set; }
        private TypeInovation _riskInovation { get; set; }
        private TypeTechnology _riskTechnology { get; set; }
        private TypeComplexity _riskComplexity { get; set; }
        private TypeRate _riskRate { get; set; }

        public string RiskName
        {
            get { return _riskName; }
            set { }
        }
        public string RiskDescription
        {
            get
            {
                return _riskDescription;
            }
            set { }
        }
        public string RiskCreationDate
        {
            get
            {
                return _riskCreationDate.ToString("dd-MM-yyyy");
            }
            set { }
        }
        public bool RiskTypeIntegrity
        {
            get { return _riskIntegrity; }
            set { }
        }
        public bool RiskTypeConfidence
        {
            get { return _riskConfidenciality; }
            set { }
        }
        public bool RiskTypeAvailability
        {
            get { return _riskAvailiability; }
            set { }
            
        }
        public Priority RiskPriority
        {
            get { return _riskPriority; }
            set { }
            
        }
        public Propability RiskPropability
        {
            get { return _riskPropability; }
            set { }
        }
        public Vulnerability RiskVulnerability
        {
            get { return _riskVulnerability; }
            set { }
        }
        public PotencialEffect RiskPotencialEffect
        {
            get { return _riskPotencialEffect; }
            set { }
        }
        public TypeInovation RiskTypeInovation
        {
            get { return _riskInovation; }
            set { }
        }
        public TypeTechnology RiskTypeTechnology
        {
            get { return _riskTechnology; }
            set { }
        }
        public TypeComplexity RiskTypeComplexity
        {
            get
            {
                return _riskComplexity;
            }
            set { }
        }
        public TypeRate RiskTypeRate
        {
            get
            {
                return _riskRate;
            }
            set { }
        }
        public RiskDetailsViewModel(Risk risk)
        {
            _serviceRisk = MainWindowService.GetRiskService();
            _selectedRisk = _serviceRisk.LoadRiskWithEntities(risk.RiskId);
            _riskName= risk.Name;
            _riskDescription = risk.Description;
            _riskCreationDate=risk.CreationTime;
            _riskIntegrity=risk.Types.Any(x => x.Type == TypeRiskValues.INT) ? true : false;
            _riskAvailiability=risk.Types.Any(x=>x.Type==TypeRiskValues.AVA) ? true :false;
            _riskConfidenciality = risk.Types.Any(x => x.Type == TypeRiskValues.CON) ? true : false;
            _riskPriority=risk.Priority;
            _riskVulnerability=risk.Vulnerability;
            _riskPropability=risk.Propability;
            _riskPotencialEffect=risk.PotencialEffect;
            _riskInovation=risk.RhombusParams.Inovation;
            _riskTechnology=risk.RhombusParams.Technology;
            _riskComplexity=risk.RhombusParams.Complexity;
            _riskRate=risk.RhombusParams.Rate;
        }
    }
}
