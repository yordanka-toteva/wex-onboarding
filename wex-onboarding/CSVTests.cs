using NUnit.Framework;
using System.Collections.Generic;
using wex_onboarding.CSV;
using wex_onboarding.Operations;

namespace wex_onboarding
{
    [TestFixture]
    public class CSVTests
    {
        private readonly string destFolder = "Download";
        private readonly string fileName = "Contribution.csv";
        string pathToExtractedFile;
        bool isCSVFileCreated;

        private List<Contribution> contributions;
        private Contribution contrRow1 = new Contribution()
        {
            EmployeeIdentifier = "123456789",
            ContributionDate = "12122022",
            ContributionDescription = "Employer",
            ContributionAmount = "10000000.00",
            PlanName = "HSA",
            PriorTaxYear = "Current"
        };
        private Contribution contrRow2 = new Contribution()
        {
            EmployeeIdentifier = "123456789",
            ContributionDate = "12122022",
            ContributionDescription = "Employer",
            ContributionAmount = "10000000.00",
            PlanName = "FSA",
            PriorTaxYear = "Current"
        };

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            FileCommon.DeleteFileAndDirectory(destFolder);
        }

        [Test]
        public void VerifyIfCSVFileIsCreated()
        {
            contributions = new List<Contribution>();
            contributions.Add(contrRow1);
            contributions.Add(contrRow2);

            pathToExtractedFile = CSVFile.CreateCSV(contributions, destFolder, fileName);
            isCSVFileCreated = FileCommon.CheckFileExists(pathToExtractedFile);

            Assert.IsTrue(isCSVFileCreated, "A CSV file with data was not created.");
        }

        [Test]
        public void VerifyIfCSVFileIsValid()
        {
            var data = CSVFile.ReadCSV(pathToExtractedFile);
            var validationResult = CSVFile.ValidateCSVContent(data).Item1;

            Assert.IsTrue(validationResult, "The data in the CSV file is not valid.");
        }
    }
}