using NUnit.Framework;
using wex_onboarding.Operations;
using wex_onboarding.XMLToObject;

namespace wex_onboarding
{
    public class UnitTests
    {
        private readonly string filePath = @"Files\InputFile.xml";

        [Test]
        public void VerifyTheTotalPaidAmount()
        {
            decimal expectedPaidAmountTotal = 3.4900M;

            PaymentFile paymentFile = XMLDeserialization.DeserializeToObject<PaymentFile>(filePath);
            decimal actualPaidAmountTotal = Methods.ReturnTotalPaidAmount(paymentFile.PaymentDetail.ReimbursementEOB.PlanBalanceTable);

            Assert.AreEqual(expectedPaidAmountTotal, actualPaidAmountTotal, $"The expected total paid amount ({expectedPaidAmountTotal}) " +
                $"is different from the actual one ({actualPaidAmountTotal}).");
        }
    }
}