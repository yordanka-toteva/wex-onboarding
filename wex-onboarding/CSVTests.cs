using NUnit.Framework;
using System.Collections.Generic;
using wex_onboarding.CSV;
using wex_onboarding.Operations;
using wex_onboarding.XMLToObject;

namespace wex_onboarding
{
    [TestFixture]
    public class CSVTests
    {
        private readonly string destFolder = "Download";
        private readonly string fileName = "Contribution.csv";
        string pathToExtractedFile;
        private List<Contribution> contributions;
        private Contribution contrRow1 = new Contribution()
        {
            EmployeeIdentifier = "123456789",
            ContributionDate = "10-10-2022",
            ContributionDescription = "Employ",
            ContributionAmount = 1000,
            PlanName = "HSA"
        };
        private Contribution contrRow2 = new Contribution()
        {
            EmployeeIdentifier = "1a3456789",
            ContributionDate = "12/12/2022",
            ContributionDescription = "Employ",
            ContributionAmount = 1000,
            PlanName = "HSA"
        };
        private Contribution contrRow3 = new Contribution()
        {
            EmployeeIdentifier = "12345678",
            ContributionDate = "12/12/2022",
            ContributionDescription = "Employ",
            ContributionAmount = 1000,
            PlanName = "HSA"
        };
        private Contribution contrRow4 = new Contribution()
        {
            EmployeeIdentifier = "",
            ContributionDate = "12/12/2022",
            ContributionDescription = "Employ",
            ContributionAmount = 1000,
            PlanName = "HSA"
        };

        [SetUp]
        public void Setup()
        {
            //CSVFile.DeleteFileAndDirectory(destFolder);
        }


        [Test]
        public void VerifyTheTotalPaidAmount()
        {
            contributions = new List<Contribution>();
            contributions.Add(contrRow1);
            //contributions.Add(contrRow2);
            //contributions.Add(contrRow3);
            //contributions.Add(contrRow4);

            pathToExtractedFile = CSVFile.CreateCSV(contributions, destFolder, fileName);
            var data = CSVFile.ReadCSV(pathToExtractedFile);
            var validations = CSVFile.ValidateCSVContent(data);
        }
    }
}