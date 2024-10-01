using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_analyser.Data.Model
{
    public class RiskDataGridItem
    {
        public string Risk { get; set; }
        public long RiskId { get; set; }
        public string RiskDesc { get; set; }
        public string Asset { get; set; }
        public bool BelongToControlRisk { get; set; }
        public RiskDataGridItem(long id, string risk, string riskdesc, string asset, bool status)
        {
            RiskId = id;
            Risk = risk;
            Asset = asset;
            RiskDesc = riskdesc;
            BelongToControlRisk = status;
        }
    }
}
