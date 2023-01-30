using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Linq;
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
            if (Directory.Exists(directoryName))
            {
                Directory.Delete(directoryName, true);
            }
        }

        public static Tuple<bool, List<Validation>> ValidateCSVContent(List<Contribution> contributions)
        {
            List<Validation> validations = new List<Validation>();
            bool isCSvValid = true;

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
                    if (contribution.EmployeeIdentifier.Length < 9 || contribution.EmployeeIdentifier.Length > 9)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.EmployeeIdentifier),
                            IsValid = false,
                            ValidationMessage = $"The allowed length for the {nameof(Contribution.EmployeeIdentifier)} property is 9."
                        });
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
                    if (!contribution.ContributionDescription.Equals(ContributionDescription.Employer.ToString()) &&
                        !contribution.ContributionDescription.Equals(ContributionDescription.Payroll.ToString()))
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.ContributionDescription),
                            IsValid = false,
                            ValidationMessage = $"The possible values for the {nameof(Contribution.ContributionDescription)} " +
                            $"property are {ContributionDescription.Employer} or {ContributionDescription.Payroll}."
                        });
                    }
                }

                if (String.IsNullOrEmpty(contribution.ContributionAmount))
                {
                    validations.Add(new Validation()
                    {
                        PropertyName = nameof(Contribution.ContributionAmount),
                        IsValid = false,
                        ValidationMessage = $"The value of the {nameof(Contribution.ContributionAmount)} property is required."
                    });
                }
                else
                {
                    if (contribution.ContributionAmount.Length > 13)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.ContributionAmount),
                            IsValid = false,
                            ValidationMessage = $"The maximum length of the {nameof(Contribution.ContributionAmount)} property is 13."
                        });
                    }

                    if (!contribution.ContributionAmount.All(c => char.IsDigit(c)))
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.ContributionAmount),
                            IsValid = false,
                            ValidationMessage = $"The {nameof(Contribution.ContributionAmount)} property must contain only digits."
                        });
                    }
                    else
                    {
                        decimal contributionAmount;
                        bool isAmountValidDecimal = Decimal.TryParse(contribution.ContributionAmount, NumberStyles.AllowDecimalPoint,
                            CultureInfo.GetCultureInfo("en-US"),
                            out contributionAmount);
                        if(!isAmountValidDecimal)
                        {
                            validations.Add(new Validation()
                            {
                                PropertyName = nameof(Contribution.ContributionAmount),
                                IsValid = false,
                                ValidationMessage = $"The value of the {nameof(Contribution.ContributionAmount)} property is not valid."
                            });
                        }

                        if (contribution.ContributionAmount.Contains(".") &&
                            contribution.ContributionAmount.Substring(contribution.ContributionAmount.IndexOf('.')).Length > 2)
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
                                        ValidationMessage = $"For HSA plans the values of the {nameof(Contribution.ContributionAmount)} property cannot negative and greater than 999999999.99."
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
                                        ValidationMessage = $"For non-HSA plans the values of the {nameof(Contribution.ContributionAmount)} property must be between -999999999.99 and 999999999.99."
                                    });
                                }
                            }
                        }
                    }

                    
                    if (String.IsNullOrEmpty(contribution.PlanName) && contributions.Count > 1)
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.PlanName),
                            IsValid = false,
                            ValidationMessage = $"The maximum length of the {nameof(Contribution.PlanName)} property is 13."
                        });
                    }
                    else
                    {
                        if(!contribution.PlanName.All(c => char.IsLetterOrDigit(c)))
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

                    if (contribution.PlanName.Contains("HSA") && String.IsNullOrEmpty(contribution.PriorTaxYear))
                    {
                        validations.Add(new Validation()
                        {
                            PropertyName = nameof(Contribution.PriorTaxYear),
                            IsValid = false,
                            ValidationMessage = $"The {nameof(Contribution.PriorTaxYear)} property value is required as the plan is HSA."
                        });
                    }
                    else
                    {
                        if (!contribution.PriorTaxYear.Equals(PriorTaxYear.Current.ToString()) && !contribution.PriorTaxYear.Equals(PriorTaxYear.Prior.ToString()))
                        {
                            validations.Add(new Validation()
                            {
                                PropertyName = nameof(Contribution.PlanName),
                                IsValid = false,
                                ValidationMessage = $"The value of the {nameof(Contribution.PlanName)} property can contain only letters and digits."
                            });
                        }
                    }
                }
            }

            if(validations.Count >= 1)
            {
                isCSvValid = false;
            }

            Tuple<bool, List<Validation>> validationResult = Tuple.Create(isCSvValid, validations);

            return validationResult;
        }
    }
}
