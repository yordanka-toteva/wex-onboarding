using System.Collections.Generic;
using System.Xml.Serialization;

namespace wex_onboarding.XMLToObject
{
    public class PaymentFile
    {
        public PaymentDetail PaymentDetail { get; set; }
    }

    public class PaymentDetail
    {
        public ReimbursementEOB ReimbursementEOB { get; set; }
    }

    public class ReimbursementEOB
    {
        public List<PlanDetail> PlanBalanceTable { get; set; }
    }

    public class PlanDetail
    {
        public string PlanName { get; set; }

        public decimal EligibleAmount { get; set; }

        public string IgnoreEligibleAmount { get; set; }

        public decimal SubmittedClaims { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal PendingAmount { get; set; }

        public decimal DeniedAmount { get; set; }

        public decimal PlanYearBalance { get; set; }

        public string IgnorePlanYearBalance { get; set; }

        public decimal YTDContributionAmount { get; set; }
    }

    public static class Methods
    {
        public static decimal ReturnTotalPaidAmount(List<PlanDetail> planBalanceTable)
        {
            decimal totalPaidAmount = 0;
            foreach (var planDetail in planBalanceTable)
            {
                totalPaidAmount += planDetail.PaidAmount;
            }

            return totalPaidAmount;
        }
    }
}
