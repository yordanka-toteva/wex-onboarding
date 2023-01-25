using NUnit.Framework;
using wex_onboarding.Operations;
using wex_onboarding.XMLToObject;

namespace wex_onboarding
{
    [TestFixture]
    public class UnitTests
    {
        private readonly string srcFile = @"Files\SNC_AOD_01102023_061442.xml.zip";
        private readonly string destFolder = "OutputFiles";
        private string pathToExtractedFile;

        [Test]
        public void VerifyTheTotalPaidAmount()
        {
            decimal expectedPaidAmountTotal = 3.4900M;

            pathToExtractedFile = ZipFileExtract.ExtractAndCopyFileFromZIP(srcFile, destFolder);
            PaymentFile paymentFile = XMLDeserialization.DeserializeToObject<PaymentFile>(pathToExtractedFile);
            decimal actualPaidAmountTotal = Methods.ReturnTotalPaidAmount(paymentFile.PaymentDetail.ReimbursementEOB.PlanBalanceTable);

            Assert.AreEqual(expectedPaidAmountTotal, actualPaidAmountTotal, $"The expected total paid amount ({expectedPaidAmountTotal}) " +
                $"is different from the actual one ({actualPaidAmountTotal}).");
        }

        [TearDown]
        public void TearDown()
        {
            ZipFileExtract.DeleteFileAndDirectory(pathToExtractedFile);
        }
    }
}