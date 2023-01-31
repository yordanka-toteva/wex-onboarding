using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(contributions);
            }

            return filePath;
        }


        public static List<Contribution> ReadCSV(string filePath)
        {
            List<Contribution> contributions = new List<Contribution>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var textReader = new StreamReader(fileStream, Encoding.UTF8))
                using (var csv = new CsvReader(textReader, config))
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


        public static Tuple<bool, List<Validation>> ValidateCSVContent(List<Contribution> contributions)
        {
            List<Validation> validations = new List<Validation>();
            bool isCSvValid = false;

            foreach (var contribution in contributions)
            {
                // EmployeeIdentifier validations
                if (String.IsNullOrEmpty(contribution.EmployeeIdentifier))
                {
                    validations.Add(new Validation()
                    {
                        PropertyName = nameof(Contribution.EmployeeIdentifier),
                        IsValid = false,
                        ValidationMessage = $"A value must be provided for the {nameof(Contribution.EmployeeIdentifier)} property."
                    });
                }
                else
                {
                    if (!contribution.EmployeeIdentifier.All(c => char.IsDigit(c)))
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.EmployeeIdentifier),
                            IsValid = false,
                            ValidationMessage = $"The {nameof(Contribution.EmployeeIdentifier)} property must contain only digits."
                        });
                    }
                    else
                    {
                        if (contribution.EmployeeIdentifier.Length < 9 || contribution.EmployeeIdentifier.Length > 9)
                        {
                            validations.Add(new Validation()
                            {
                                PropertyName = nameof(Contribution.EmployeeIdentifier),
                                IsValid = false,
                                ValidationMessage = $"The {nameof(Contribution.EmployeeIdentifier)} property is with fixed length 9."
                            });
                        }
                    }
                }

                // ContributionDate validations
                if (String.IsNullOrEmpty(contribution.ContributionDate))
                {
                    validations.Add(new Validation()
                    {
                        PropertyName = nameof(Contribution.ContributionDate),
                        IsValid = false,
                        ValidationMessage = $"A value must be provided for the {nameof(Contribution.ContributionDate)} property."
                    });
                }
                else
                {
                    bool isDateValid = DateTime.TryParseExact(contribution.ContributionDate, "MMddyyyy",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out DateTime dateParsed);
                    if (!isDateValid)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.ContributionDate),
                            IsValid = false,
                            ValidationMessage = $"The value of the {nameof(Contribution.ContributionDate)} property is not valid."
                        });
                    }
                }

                // ContributionDescription validations
                if (String.IsNullOrEmpty(contribution.ContributionDescription))
                {
                    validations.Add(new Validation()
                    {
                        PropertyName = nameof(Contribution.ContributionDescription),
                        IsValid = false,
                        ValidationMessage = $"A value must be provided for the {nameof(Contribution.ContributionDescription)} property."
                    });
                }
                else
                {
                    if (!contribution.ContributionDescription.Equals(ContributionDescription.Employer.ToString()) &&
                        !contribution.ContributionDescription.Equals(ContributionDescription.Payroll.ToString()))
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.ContributionDescription),
                            IsValid = false,
                            ValidationMessage = $"The possible values for the {nameof(Contribution.ContributionDescription)} " +
                            $"property are {ContributionDescription.Employer} and {ContributionDescription.Payroll}."
                        });
                    }
                }

                // ContributionAmount validations
                if (String.IsNullOrEmpty(contribution.ContributionAmount))
                {
                    validations.Add(new Validation()
                    {
                        PropertyName = nameof(Contribution.ContributionAmount),
                        IsValid = false,
                        ValidationMessage = $"A value must be provided for the {nameof(Contribution.ContributionAmount)} property."
                    });
                }
                else
                {
                    decimal contributionAmount;
                    bool isAmountValidDecimal = Decimal.TryParse(contribution.ContributionAmount,
                        NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                            CultureInfo.GetCultureInfo("en-US"),
                            out contributionAmount);
                    if (!isAmountValidDecimal)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.ContributionAmount),
                            IsValid = false,
                            ValidationMessage = $"The value of the {nameof(Contribution.ContributionAmount)} property is not valid."
                        });
                    }

                    if (contribution.ContributionAmount.Contains(".") && contribution.ContributionAmount.Substring(contribution.ContributionAmount.IndexOf('.') + 1).Length > 2)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.ContributionAmount),
                            IsValid = false,
                            ValidationMessage = $"Only 2 decimal places are allowed after the decimal separator for the {nameof(Contribution.ContributionAmount)} property."
                        });
                    }
                    else
                    {
                        if (contribution.PlanName.Contains("HSA"))
                        {
                            if (contributionAmount < 0M || contributionAmount > 999999999.99M)
                            {
                                validations.Add(new Validation()
                                {
                                    PropertyName = nameof(Contribution.ContributionAmount),
                                    IsValid = false,
                                    ValidationMessage = $"For HSA plans the value of the {nameof(Contribution.ContributionAmount)} property cannot be negative and greater than 999999999.99."
                                });
                            }
                        }
                        else
                        {
                            if (contributionAmount < -999999999.99M || contributionAmount > 999999999.99M)
                            {
                                validations.Add(new Validation()
                                {
                                    PropertyName = nameof(Contribution.ContributionAmount),
                                    IsValid = false,
                                    ValidationMessage = $"For non-HSA plans the value of the {nameof(Contribution.ContributionAmount)} property must be between -999999999.99 and 999999999.99."
                                });
                            }
                        }
                    }
                }

                // PlanName validations
                if (String.IsNullOrEmpty(contribution.PlanName) && contributions.Count > 1)
                {
                    validations.Add(new Validation()
                    {
                        PropertyName = nameof(Contribution.PlanName),
                        IsValid = false,
                        ValidationMessage = $"The {nameof(Contribution.PlanName)} property must have a value if there is more than 1 contribution in the file."
                    });
                }
                else
                {
                    if (!String.IsNullOrEmpty(contribution.PlanName) && !contribution.PlanName.All(c => char.IsLetterOrDigit(c)))
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.PlanName),
                            IsValid = false,
                            ValidationMessage = $"The value of the {nameof(Contribution.PlanName)} property can contain only letters and digits."
                        });
                    }
                    if (contribution.PlanName.Length > 255)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.PlanName),
                            IsValid = false,
                            ValidationMessage = $"The {nameof(Contribution.PlanName)} value cannot be more than 255 characters."
                        });
                    }
                }

                // PriorTaxYear validations
                if (contribution.PlanName.Contains("HSA"))
                {
                    if (String.IsNullOrEmpty(contribution.PriorTaxYear))
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.PriorTaxYear),
                            IsValid = false,
                            ValidationMessage = $"A value must be provided for the {nameof(Contribution.PriorTaxYear)} property as the plan is HSA."
                        });
                    }
                    else
                    {
                        if (!contribution.PriorTaxYear.Equals(PriorTaxYear.Current.ToString()) && !contribution.PriorTaxYear.Equals(PriorTaxYear.Prior.ToString()))
                        {
                            validations.Add(new Validation()
                            {
                                PropertyName = nameof(Contribution.PriorTaxYear),
                                IsValid = false,
                                ValidationMessage = $"The possible values for the {nameof(Contribution.PriorTaxYear)} property are {PriorTaxYear.Current} and {PriorTaxYear.Prior}."
                            });
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(contribution.PriorTaxYear))
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.PriorTaxYear),
                            IsValid = false,
                            ValidationMessage = $"The value of the {nameof(Contribution.PriorTaxYear)} property is valid only for HSA plans."
                        });
                    }
                }
            }

            if (contributions.Count > 0 && validations.Count == 0)
            {
                isCSvValid = true;
            }

            Tuple<bool, List<Validation>> validationResult = Tuple.Create(isCSvValid, validations);

            return validationResult;
        }
    }
}