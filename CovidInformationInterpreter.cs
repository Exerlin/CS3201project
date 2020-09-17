
using System;
using System.Collections.Generic;
using System.Linq;

namespace Covid19Analysis
{
    public class CovidInformationInterpreter
    {

        public static int Date = 0;
        public static int State = 1;
        public static int PositiveIncrease = 2;
        public static int NegativeIncrease = 3;
        public static int DeathIncrease = 4;
        public static int HospitalizedIncrease = 5;

        private readonly StackedStringArray covidInformationList;
        private readonly CovidStatistics theProcessedCovidStatistics;
        public CovidInformationInterpreter()
        {
            this.covidInformationList = new StackedStringArray();
            this.theProcessedCovidStatistics = new CovidStatistics();
        }

        public void addCovidInformation(List<String> eachLineInFile)
        {
            for (int currentIndex = eachLineInFile.Count - 1; currentIndex >= 0; currentIndex--)
            {
                List<String> attributesFromFile = new List<string>();
                attributesFromFile.AddRange(eachLineInFile.ElementAt(currentIndex).Split(","));
                if (isAttributeXString(attributesFromFile, 1, "GA"))
                {
                    this.covidInformationList.addListToList(
                        new List<string>(eachLineInFile.ElementAt(currentIndex).Split(",")));
                }
            }

            generateCovidStatistics();
        }

        private Boolean isAttributeXString(List<String> theList, int attributeIndex, String attributeValue)
        {
            if (theList.Count > 1 && theList.ElementAt(attributeIndex).Equals(attributeValue))
            {
                return true;
            }
            return false;
        }

        public DateTime getDate(int valueIndex)
        {
            return DateTime.Parse(this.covidInformationList.getString(Date, valueIndex), 
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public string getState(int valueIndex)
        {
            return this.covidInformationList.getString(State, valueIndex);
        }

        public string getPositiveIncrease(int valueIndex)
        {
            return this.covidInformationList.getString(PositiveIncrease, valueIndex);
        }

        public string getNegativeIncrease(int valueIndex)
        {
            return this.covidInformationList.getString(NegativeIncrease, valueIndex);
        }

        public string getDeathIncrease(int valueIndex)
        {
            return this.covidInformationList.getString(DeathIncrease, valueIndex);
        }

        public string getHospitalizedIncrease(int valueIndex)
        {
            return this.covidInformationList.getString(HospitalizedIncrease, valueIndex);
        }

        public CovidStatistics getCovidStatistics()
        {
            return this.theProcessedCovidStatistics;
        }

        /**
         * Generates statistics regarding the affects of Covid by using the data provided.
         */
        public void generateCovidStatistics()
        {
            this.firstPositiveTestDate();
            this.highestNumberOfAttribute(PositiveIncrease);
            this.highestNumberOfAttribute(NegativeIncrease);
            this.highestNumberOfAttribute(DeathIncrease);
            this.highestNumberOfAttribute(HospitalizedIncrease);
        }

        private void firstPositiveTestDate()
        {
            foreach (var currentList in this.getStackedStringList().Where(currentList => !currentList.ElementAt(PositiveIncrease).Equals("0")))
            {
                this.theProcessedCovidStatistics.DateFirstPositiveTest = currentList.ElementAt(Date);
                break;
            }
        }

        private void highestNumberOfAttribute(int attributeToFindHighestOf)
        {
            var dateOfCurrentHighest = "00000000";
            var currentHighestNumber = int.MinValue;

            foreach (var currentList in this.getStackedStringList().Where(currentList => int.Parse(currentList.ElementAt(attributeToFindHighestOf)) > currentHighestNumber))
            {
                dateOfCurrentHighest = currentList.ElementAt(Date);
                currentHighestNumber = int.Parse(currentList.ElementAt(attributeToFindHighestOf));
            }

            if (attributeToFindHighestOf == PositiveIncrease)
            {
                this.theProcessedCovidStatistics.DatePositiveTestsHighest = dateOfCurrentHighest;
                this.theProcessedCovidStatistics.NumberPositiveTestsHighest = currentHighestNumber.ToString();
            } else if (attributeToFindHighestOf == NegativeIncrease)
            {
                this.theProcessedCovidStatistics.DateNegativeTestsHighest = dateOfCurrentHighest;
                this.theProcessedCovidStatistics.NumberNegativeTestsHighest = currentHighestNumber.ToString();
            } else if (attributeToFindHighestOf == DeathIncrease)
            {
                this.theProcessedCovidStatistics.DateDeathsHighest = dateOfCurrentHighest;
                this.theProcessedCovidStatistics.NumberDeathsHighest = currentHighestNumber.ToString();
            } else if (attributeToFindHighestOf == HospitalizedIncrease)
            {
                this.theProcessedCovidStatistics.DateHospitalizationsHighest = dateOfCurrentHighest;
                this.theProcessedCovidStatistics.NumberHospitalizationsHighest = currentHighestNumber.ToString();
            }
        }

        private String averageNumberOfPositiveTestsPerDay()
        {
            int firstDayOfPositiveTests = 0;
            int totalNumberOfPositiveTests = 0;

            foreach (List<String> currentList in this.getStackedStringList())
            {
                if (int.Parse(currentList.ElementAt(2)) > 0 && firstDayOfPositiveTests == 0)
                {
                    firstDayOfPositiveTests = int.Parse(currentList.ElementAt(0));
                    totalNumberOfPositiveTests += int.Parse(currentList.ElementAt(2));
                }
                else if (int.Parse(currentList.ElementAt(2)) > 0)
                {
                    totalNumberOfPositiveTests += int.Parse(currentList.ElementAt(2));
                }
            }

            int numberOfDaysCounted =
                int.Parse(this.getStackedStringList().ElementAt(this.getStackedStringList().Count - 1).ElementAt(0))
                - firstDayOfPositiveTests;

            return "The average number of positive tests since the first recorded positive test: " + (totalNumberOfPositiveTests / numberOfDaysCounted).ToString();
        }

        private String numberOfDaysWithOverTwoThousandFiftyPositiveTests()
        {
            int numberOfDays = 0;
            foreach (List<String> currentList in this.getStackedStringList())
            {
                if (int.Parse(currentList.ElementAt(2)) > 2500)
                {
                    numberOfDays++;
                }
            }
            return "The number of days with over 2,500 positive tests: " + numberOfDays;
        }

        private String numberOfDaysWithUnderOneThousandPositiveTests()
        {
            int numberOfDays = 0;
            Boolean firstPositiveTestPassed = false;
            foreach (List<String> currentList in this.getStackedStringList())
            {
                if (!firstPositiveTestPassed && !currentList.ElementAt(2).Equals("0"))
                {
                    firstPositiveTestPassed = true;
                }
                else if (firstPositiveTestPassed && int.Parse(currentList.ElementAt(2)) < 1000)
                {
                    numberOfDays++;
                }
            }
            return "The number of days with under 1,000 positive tests: " + numberOfDays;
        }

        private String highestPercentOfPositiveTests()
        {
            String highestPercentageDay = "";
            double highestPercentage = int.MinValue;
            foreach (List<String> currentList in this.getStackedStringList())
            {
                if (double.Parse(currentList.ElementAt(2)) / (double.Parse(currentList.ElementAt(2))
                    + double.Parse(currentList.ElementAt(3))) > highestPercentage)
                {
                    highestPercentageDay = currentList.ElementAt(0);
                    highestPercentage = double.Parse(currentList.ElementAt(2))
                       / (double.Parse(currentList.ElementAt(2)) + double.Parse(currentList.ElementAt(3)));
                }
            }
            highestPercentage *= 100;
            return " has the highest percentage of positive tests: "
                + highestPercentage;
        }

        private String overallPositivity()
        {
            double numberOfPositiveTests = 0;
            double numberOfNegativeTests = 0;

            foreach (List<String> currentList in this.getStackedStringList())
            {
                numberOfPositiveTests += int.Parse(currentList.ElementAt(2));
                numberOfNegativeTests += int.Parse(currentList.ElementAt(3));
            }
            double overallPercentage = (numberOfPositiveTests / (numberOfNegativeTests + numberOfPositiveTests)) * 100;
            overallPercentage = Math.Round(overallPercentage, 2);
            return "The overall positivity of all tests: " + overallPercentage;
        }

        private List<List<String>> getStackedStringList()
        {
            return this.covidInformationList.getListOfList();
        }
    }
}