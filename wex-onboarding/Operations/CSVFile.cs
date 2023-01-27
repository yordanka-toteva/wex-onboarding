using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using wex_onboarding.CSV;

namespace wex_onboarding.Operations
{
    public static class CSVFile
    {
        public static string CreateCSV(List<Contribution> contributions, string destFolder, string fileName)
        {
            string filePath = Path.Combine(destFolder, fileName);
            var directoryName = Path.GetDirectoryName(filePath);

            if (directoryName != null) Directory.CreateDirectory(directoryName);

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(contributions);
            }

            return filePath;
        }

        public static List<Contribution> ReadCSV(string filePath)
        {
            List<Contribution> contributions = new List<Contribution>();
            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var textReader = new StreamReader(fileStream, Encoding.UTF8))
                using (var csv = new CsvReader(textReader, CultureInfo.InvariantCulture))
                {
                    var csvData = csv.GetRecords<Contribution>();

                    foreach (var row in csvData)
                    {
                        contributions.Add(row);
                    }
                }
            }

            return contributions;
        }

        public static void DeleteFileAndDirectory(string directoryName)
        {
            //string directoryName = Path.GetDirectoryName(pathToExtractedFile);

            if (Directory.Exists(directoryName))
            {
                Directory.Delete(directoryName, true);
            }
        }

        public static List<Validation> ValidateCSVContent(List<Contribution> contributions)
        {
            List<Validation> validations = new List<Validation>();

            foreach (var contribution in contributions)
            {
                if(String.IsNullOrEmpty(contribution.EmployeeIdentifier))
                {
                    validations.Add(new Validation()
                    {
                        PropertyName = nameof(Contribution.EmployeeIdentifier),
                        IsValid = false,
                        ValidationMessage = $"The value of the {nameof(Contribution.EmployeeIdentifier)} property is required."
                    });
                }
                else
                {
                    bool areDigitsOnly = int.TryParse(contribution.EmployeeIdentifier, out int result);
                    if (!areDigitsOnly)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.EmployeeIdentifier),
                            IsValid = false,
                            ValidationMessage = $"The value of the {nameof(Contribution.EmployeeIdentifier)} property must contain only digits."
                        });
                    }
                    if(contribution.EmployeeIdentifier.Length < 9 || contribution.EmployeeIdentifier.Length > 9)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.EmployeeIdentifier),
                            IsValid = false,
                            ValidationMessage = $"The length of the {nameof(Contribution.EmployeeIdentifier)} property value must be 9."
                        });
                    }
                }

                if (String.IsNullOrEmpty(contribution.ContributionDate))
                {
                    validations.Add(new Validation()
                    {
                        PropertyName = nameof(Contribution.ContributionDate),
                        IsValid = false,
                        ValidationMessage = $"The value of the {nameof(Contribution.ContributionDate)} property is required."
                    });
                }
                else
                {
                    DateTime dateParsed;
                    bool isDateValid = DateTime.TryParseExact(contribution.ContributionDate, "MMddyyyy",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out dateParsed);
                    if(!isDateValid)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.ContributionDate),
                            IsValid = false,
                            ValidationMessage = $"The value of the {nameof(Contribution.ContributionDate)} property is invalid."
                        });
                    }
                }

                if (String.IsNullOrEmpty(contribution.ContributionDescription))
                {
                    validations.Add(new Validation()
                    {
                        PropertyName = nameof(Contribution.ContributionDescription),
                        IsValid = false,
                        ValidationMessage = $"The value of the {nameof(Contribution.ContributionDescription)} property is required."
                    });
                }
                else
                {
                    if (!contribution.ContributionDescription.Equals(ContributionDescription.Employer) || !contribution.ContributionDescription.Equals(ContributionDescription.Payroll))
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.ContributionDescription),
                            IsValid = false,
                            ValidationMessage = $"The possible values for the {nameof(Contribution.ContributionDescription)} property are {ContributionDescription.Employer} or {ContributionDescription.Payroll}."
                        });
                    }
                }
            }

            return validations;
        }
    }
}
