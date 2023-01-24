using NUnit.Framework;
using wex_onboarding.Operations;
using wex_onboarding.XMLToObject;

namespace wex_onboarding
{
    public class UnitTests
    {

        [Test]
        public void VerifyTheTotalPaidAmount()
        {
            decimal expectedPaidAmountTotal = 3.4900M;

            PaymentFile paymentFile = XMLDeserialization.DeserializeToObject<PaymentFile>(@"Files\InputFile.xml");
            decimal actualPaidAmountTotal = Methods.ReturnTotalPaidAmount(paymentFile.PaymentDetail.ReimbursementEOB.PlanBalanceTable);

            Assert.AreEqual(expectedPaidAmountTotal, actualPaidAmountTotal, $"The expected total paid amount ({expectedPaidAmountTotal}) " +
                $"is different from the actual one ({actualPaidAmountTotal}).");
        }
    }
}