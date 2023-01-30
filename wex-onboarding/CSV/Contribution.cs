using System;
using System.Collections.Generic;
using System.Text;

namespace wex_onboarding.CSV
{
    public class Contribution
    {
        public string EmployeeIdentifier { get; set; }
        public string ContributionDate { get; set; }
        public string ContributionDescription { get; set; }
        public string ContributionAmount { get; set; }
        public string PlanName { get; set; }
        public string PriorTaxYear { get; set; }
    }
}
