
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static int DefaultLowerBoundary = 2500;
        public static int DefaultUpperBoundary = 1000;

        private readonly StackedStringArray covidInformationList;
        private readonly CovidStatistics theProcessedCovidStatistics;
        private string firstPositiveTestDate;
        private string firstTestDate;
        private readonly List<MonthlyCovidStatistics> theProcessedMonthlyCovidStatisticses;

        public CovidInformationInterpreter()
        {
            this.covidInformationList = new StackedStringArray();
            this.theProcessedCovidStatistics = new CovidStatistics();
            this.theProcessedMonthlyCovidStatisticses = new List<MonthlyCovidStatistics>();
            this.firstPositiveTestDate = "000000";
            this.firstTestDate = "000000";
        }

        public void addCovidInformation(List<string> eachLineInFile)
        {
            for (int currentIndex = eachLineInFile.Count - 1; currentIndex >= 0; currentIndex--)
            {
                List<string> attributesFromFile = new List<string>();
                attributesFromFile.AddRange(eachLineInFile.ElementAt(currentIndex).Split(","));
                if (this.verifyFormatOfLine(attributesFromFile) && isAttributeXString(attributesFromFile, 1, "GA"))
                {
                    this.covidInformationList.addListToList(
                        new List<string>(eachLineInFile.ElementAt(currentIndex).Split(",")));
                }
            }

            generateCovidStatistics();
        }

        private Boolean isAttributeXString(List<string> theList, int attributeIndex, string attributeValue)
        {
            if (theList.Count > 1 && theList.ElementAt(attributeIndex).Equals(attributeValue))
            {
                return true;
            }
            return false;
        }

        private Boolean verifyFormatOfLine(List<string> theLineToVerify)
        {
            return int.TryParse(theLineToVerify.ElementAt(Date), out _) 
                   && int.TryParse(theLineToVerify.ElementAt(PositiveIncrease), out _) 
                   && int.TryParse(theLineToVerify.ElementAt(NegativeIncrease), out _) 
                   && int.TryParse(theLineToVerify.ElementAt(DeathIncrease), out _) 
                   && int.TryParse(theLineToVerify.ElementAt(HospitalizedIncrease), out _) 
                   && !int.TryParse(theLineToVerify.ElementAt(State), out _);
        }

        public DateTime getDate(int valueIndex)
        {
            return DateTime.Parse(this.covidInformationList.getString(Date, valueIndex), 
                CultureInfo.InvariantCulture);
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

        public MonthlyCovidStatistics getMonthlyCovidStatistics(int monthIndex)
        {
            return this.theProcessedMonthlyCovidStatisticses.ElementAt(monthIndex);
        }

        /**
         * Generates statistics regarding the affects of Covid by using the data provided.
         */
        public void generateCovidStatistics()
        {
            this.generateCovidStatistics(DefaultLowerBoundary, DefaultUpperBoundary);
        }

        public void generateCovidStatistics(int lowerBoundary, int upperBoundary)
        {
            this.findFirstTestDate();
            this.findFirstPositiveTestDate();
            this.highestNumberOfAttribute(PositiveIncrease);
            this.highestNumberOfAttribute(NegativeIncrease);
            this.highestNumberOfAttribute(DeathIncrease);
            this.highestNumberOfAttribute(HospitalizedIncrease);
            this.highestNumberOfTotalTests();
            this.averageNumberOfAttributePerDay(PositiveIncrease);
            this.numberOfDaysWithOverNumberOfPositiveTests(lowerBoundary);
            this.numberOfDaysWithUnderNumberOfPositiveTests(upperBoundary);
            this.highestPercentOfPositiveTests();
            this.overallPositivity();
            this.fillPositiveTestHistogram();

            this.iterateThroughEachMonth();
        }

        private void findFirstTestDate()
        {
            foreach (var currentList in this.getStackedStringList().Where(currentList => !currentList.ElementAt(PositiveIncrease).Equals("0") || !currentList.ElementAt(NegativeIncrease).Equals("0")))
            {
                this.firstTestDate = currentList.ElementAt(Date);
                break;
            }
        }

        private void findFirstPositiveTestDate()
        {
            foreach (var currentList in this.getStackedStringList().Where(currentList => !currentList.ElementAt(PositiveIncrease).Equals("0")))
            {
                this.theProcessedCovidStatistics.DateFirstPositiveTest = currentList.ElementAt(Date);
                this.firstPositiveTestDate = currentList.ElementAt(Date);
                break;
            }
        }

        private void fillPositiveTestHistogram()
        {
            for (int currentPositiveTestCount = 0; 
                currentPositiveTestCount < int.Parse(this.theProcessedCovidStatistics.NumberPositiveTestsHighest); 
                currentPositiveTestCount += 500)
            {
                this.theProcessedCovidStatistics.HistogramDataContents.Add(0);
            }
            foreach (var currentList in this.getStackedStringList().Where(currentList 
                => int.TryParse(currentList.ElementAt(Date), out _) 
                   && int.Parse(currentList.ElementAt(Date)) >= int.Parse(this.firstTestDate)))
            {
                int indexToAddTo = (int.Parse(currentList.ElementAt(PositiveIncrease)) + 1) / 500;
                this.theProcessedCovidStatistics.HistogramDataContents[indexToAddTo]++;
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

        private void highestNumberOfTotalTests()
        {
            var dateOfCurrentHighest = "00000000";
            var currentHighestNumber = int.MinValue;

            foreach (var currentList in this.getStackedStringList().Where(currentList =>
                int.Parse(currentList.ElementAt(PositiveIncrease)) + int.Parse(currentList.ElementAt(NegativeIncrease)) > currentHighestNumber))
            {
                dateOfCurrentHighest = currentList.ElementAt(Date);
                currentHighestNumber = int.Parse(currentList.ElementAt(PositiveIncrease)) + int.Parse(currentList.ElementAt(NegativeIncrease));
            }

            this.theProcessedCovidStatistics.NumberAllTestsHighest = currentHighestNumber.ToString();
            this.theProcessedCovidStatistics.DateAllTestsHighest = dateOfCurrentHighest;
        }

        private void averageNumberOfAttributePerDay(int attributeToFindAverageOf)
        {
            int totalNumberOfAttribute = 0;
            int numberOfDaysWithAttribute = 0;

            foreach (List<string> currentList in this.getStackedStringList())
            {
                if (int.Parse(currentList.ElementAt(attributeToFindAverageOf)) > 0 && int.Parse(currentList.ElementAt(Date)) >= int.Parse(this.firstPositiveTestDate))
                {
                    totalNumberOfAttribute += int.Parse(currentList.ElementAt(attributeToFindAverageOf));
                    numberOfDaysWithAttribute++;
                }
            }

            if (numberOfDaysWithAttribute != 0)
            {
                this.theProcessedCovidStatistics.AverageNumberOfPositiveTests = (totalNumberOfAttribute / numberOfDaysWithAttribute).ToString();
            }
            else
            {
                this.theProcessedCovidStatistics.AverageNumberOfPositiveTests = "0";
            }
        }

        private void numberOfDaysWithOverNumberOfPositiveTests(int lowerBoundary)
        {
            int numberOfDays = 0;
            foreach (List<string> currentList in this.getStackedStringList())
            {
                if (int.Parse(currentList.ElementAt(PositiveIncrease)) > lowerBoundary)
                {
                    numberOfDays++;
                }
            }
            this.theProcessedCovidStatistics.NumberOfDaysPositiveTestsAboveThreshold = numberOfDays.ToString();
        }

        private void numberOfDaysWithUnderNumberOfPositiveTests(int upperBoundary)
        {
            int numberOfDays = 0;
            foreach (List<string> currentList in this.getStackedStringList())
            {
                if (int.Parse(currentList.ElementAt(Date)) >= int.Parse(this.firstPositiveTestDate) && int.Parse(currentList.ElementAt(PositiveIncrease)) < upperBoundary)
                {
                    numberOfDays++;
                }
            }
            this.theProcessedCovidStatistics.NumberOfDaysPositiveTestsBelowThreshold = numberOfDays.ToString();
        }

        private void highestPercentOfPositiveTests()
        {
            string highestPercentageDay = "";
            double highestPercentage = int.MinValue;
            foreach (List<string> currentList in this.getStackedStringList())
            {
                if (double.Parse(currentList.ElementAt(PositiveIncrease)) / (double.Parse(currentList.ElementAt(PositiveIncrease))
                    + double.Parse(currentList.ElementAt(NegativeIncrease))) > highestPercentage)
                {
                    highestPercentageDay = currentList.ElementAt(Date);
                    highestPercentage = double.Parse(currentList.ElementAt(PositiveIncrease))
                       / (double.Parse(currentList.ElementAt(PositiveIncrease)) + double.Parse(currentList.ElementAt(NegativeIncrease)));
                }
            }
            highestPercentage *= 100;
            this.theProcessedCovidStatistics.DatePositiveTestsHighestPercent = highestPercentageDay;
            this.theProcessedCovidStatistics.NumberPositiveTestsHighestPercent = highestPercentage.ToString(CultureInfo.InvariantCulture);
        }

        private void overallPositivity()
        {
            double numberOfPositiveTests = 0;
            double numberOfNegativeTests = 0;

            foreach (List<string> currentList in this.getStackedStringList())
            {
                numberOfPositiveTests += int.Parse(currentList.ElementAt(PositiveIncrease));
                numberOfNegativeTests += int.Parse(currentList.ElementAt(NegativeIncrease));
            }
            double overallPercentage = numberOfPositiveTests / (numberOfNegativeTests + numberOfPositiveTests) * 100;
            overallPercentage = Math.Round(overallPercentage, 2);
            this.theProcessedCovidStatistics.OverallPositivityRate = overallPercentage.ToString(CultureInfo.InvariantCulture);
        }

        private void iterateThroughEachMonth()
        {
            int monthOfFirstTest = this.getMonthValue(this.firstTestDate);
            for (int currentMonth = monthOfFirstTest; currentMonth <= 12; currentMonth++)
            {
                StackedStringArray currentMonthCovidInformationList = new StackedStringArray();
                MonthlyCovidStatistics currentMonthlyCovidStatistics = new MonthlyCovidStatistics(currentMonth, 2020);

                foreach (List<string> currentList in this.getStackedStringList())
                {
                    if (int.TryParse(currentList.ElementAt(Date), out _) 
                        && this.getMonthValue(currentList.ElementAt(Date)) == currentMonth 
                        && int.Parse(currentList.ElementAt(Date)) >= int.Parse(this.firstTestDate))
                    {
                        currentMonthCovidInformationList.addListToList(currentList);
                        currentMonthlyCovidStatistics.NumberOfDaysContainingData++;
                    }

                }

                if (currentMonthlyCovidStatistics.NumberOfDaysContainingData > 0)
                {
                    this.highestNumberOfPositiveTestsMonthly(currentMonthCovidInformationList, currentMonthlyCovidStatistics);
                    this.lowestNumberOfPositiveTestsMonthly(currentMonthCovidInformationList, currentMonthlyCovidStatistics);
                    this.highestNumberOfTotalTestsMonthly(currentMonthCovidInformationList, currentMonthlyCovidStatistics);
                    this.highestNumberOfTotalTestsMonthly(currentMonthCovidInformationList, currentMonthlyCovidStatistics);
                    this.lowestNumberOfTotalTestsMonthly(currentMonthCovidInformationList, currentMonthlyCovidStatistics);
                    this.averageNumberOfPositiveTestsPerDayMonthly(currentMonthCovidInformationList, currentMonthlyCovidStatistics);
                    this.averageNumberOfTotalTestsPerDayMonthly(currentMonthCovidInformationList, currentMonthlyCovidStatistics);
                    this.theProcessedMonthlyCovidStatisticses.Add(currentMonthlyCovidStatistics);
                }
            }
        }

        private void highestNumberOfPositiveTestsMonthly(StackedStringArray givenList, MonthlyCovidStatistics statisticsToWriteTo)
        {
            List<string> unformattedDateOfCurrentHighest = new List<string>();
            int currentHighestNumberOfPositiveTests = int.MinValue;
            foreach (List<string> currentList in givenList.getListOfList())
            {
                if (int.Parse(currentList.ElementAt(PositiveIncrease)) > currentHighestNumberOfPositiveTests)
                {
                    unformattedDateOfCurrentHighest.Clear();
                    unformattedDateOfCurrentHighest.Add(currentList.ElementAt(Date));
                    currentHighestNumberOfPositiveTests = int.Parse(currentList.ElementAt(PositiveIncrease));
                } else if (int.Parse(currentList.ElementAt(PositiveIncrease)) == currentHighestNumberOfPositiveTests)
                {
                    unformattedDateOfCurrentHighest.Add(currentList.ElementAt(Date));
                }
            }
            if (currentHighestNumberOfPositiveTests == int.MinValue)
            {
                statisticsToWriteTo.NumberHighestPositiveTests = "-1";
            }
            else
            {
                statisticsToWriteTo.NumberHighestPositiveTests = currentHighestNumberOfPositiveTests.ToString();
                statisticsToWriteTo.DateHighestPositiveTests = unformattedDateOfCurrentHighest;
            }

        }
        
        private void lowestNumberOfPositiveTestsMonthly(StackedStringArray givenList, MonthlyCovidStatistics statisticsToWriteTo)
        {
            List<string> unformattedDateOfCurrentLowest = new List<string>();
            int currentLowestNumberOfPositiveTests = int.MaxValue;
            foreach (List<string> currentList in givenList.getListOfList())
            {
                if (int.Parse(currentList.ElementAt(PositiveIncrease)) < currentLowestNumberOfPositiveTests)
                {
                    unformattedDateOfCurrentLowest.Clear();
                    unformattedDateOfCurrentLowest.Add(currentList.ElementAt(Date));
                    currentLowestNumberOfPositiveTests = int.Parse(currentList.ElementAt(PositiveIncrease));
                } else if (int.Parse(currentList.ElementAt(PositiveIncrease)) == currentLowestNumberOfPositiveTests)
                {
                    unformattedDateOfCurrentLowest.Add(currentList.ElementAt(Date));
                }
            }

            if (currentLowestNumberOfPositiveTests == int.MaxValue)
            {
                statisticsToWriteTo.NumberLowestPositiveTests = "-1";
            }
            else
            {
                statisticsToWriteTo.NumberLowestPositiveTests = currentLowestNumberOfPositiveTests.ToString();
                statisticsToWriteTo.DateLowestPositiveTests = unformattedDateOfCurrentLowest;
            }
        }

        private void highestNumberOfTotalTestsMonthly(StackedStringArray givenList, MonthlyCovidStatistics statisticsToWriteTo)
        {
            List<string> unformattedDateOfCurrentHighest = new List<string>();
            int currentHighestNumberOfTests = int.MinValue;
            foreach (List<string> currentList in givenList.getListOfList())
            {
                if (int.Parse(currentList.ElementAt(PositiveIncrease)) + int.Parse(currentList.ElementAt(NegativeIncrease)) > currentHighestNumberOfTests)
                {
                    unformattedDateOfCurrentHighest.Clear();
                    unformattedDateOfCurrentHighest.Add(currentList.ElementAt(Date));
                    currentHighestNumberOfTests = int.Parse(currentList.ElementAt(PositiveIncrease)) + int.Parse(currentList.ElementAt(NegativeIncrease));
                } else if (int.Parse(currentList.ElementAt(PositiveIncrease)) +
                    int.Parse(currentList.ElementAt(NegativeIncrease)) == currentHighestNumberOfTests)
                {
                    unformattedDateOfCurrentHighest.Add(currentList.ElementAt(Date));
                }
            }

            if (currentHighestNumberOfTests == int.MinValue)
            {
                statisticsToWriteTo.NumberHighestTotalTests = "-1";
            }
            else
            {
                statisticsToWriteTo.NumberHighestTotalTests = currentHighestNumberOfTests.ToString();
                statisticsToWriteTo.DateHighestTotalTests = unformattedDateOfCurrentHighest;
            }
        }

        private void lowestNumberOfTotalTestsMonthly(StackedStringArray givenList, MonthlyCovidStatistics statisticsToWriteTo)
        {
            List<string> unformattedDateOfCurrentLowest = new List<string>();
            int currentLowestNumberOfTests = int.MaxValue;
            foreach (List<string> currentList in givenList.getListOfList())
            {
                if (int.Parse(currentList.ElementAt(PositiveIncrease)) + int.Parse(currentList.ElementAt(NegativeIncrease)) < currentLowestNumberOfTests)
                {
                    unformattedDateOfCurrentLowest.Clear();
                    unformattedDateOfCurrentLowest.Add(currentList.ElementAt(Date));
                    currentLowestNumberOfTests = int.Parse(currentList.ElementAt(PositiveIncrease)) + int.Parse(currentList.ElementAt(NegativeIncrease));
                } else if (int.Parse(currentList.ElementAt(PositiveIncrease)) +
                    int.Parse(currentList.ElementAt(NegativeIncrease)) < currentLowestNumberOfTests)
                {
                    unformattedDateOfCurrentLowest.Add(currentList.ElementAt(Date));
                }

            }

            if (currentLowestNumberOfTests == int.MaxValue)
            {
                statisticsToWriteTo.NumberLowestTotalTests = "-1";
            }
            else
            {
                statisticsToWriteTo.NumberLowestTotalTests = currentLowestNumberOfTests.ToString();
                statisticsToWriteTo.DateLowestTotalTests = unformattedDateOfCurrentLowest;
            }
        }

        private void averageNumberOfPositiveTestsPerDayMonthly(StackedStringArray givenList, MonthlyCovidStatistics statisticsToWriteTo)
        {
            int totalNumberOfAttribute = 0;
            int numberOfDaysCounted = 0;

            foreach (List<string> currentList in givenList.getListOfList())
            {
                totalNumberOfAttribute += int.Parse(currentList.ElementAt(PositiveIncrease));
                numberOfDaysCounted++;
            }

            if (numberOfDaysCounted == 0)
            {
                statisticsToWriteTo.NumberAveragePositiveTests = "-1";
            }
            else
            {
                double averageNumberOfPositiveTests = Convert.ToDouble(totalNumberOfAttribute) / Convert.ToDouble(numberOfDaysCounted);
                statisticsToWriteTo.NumberAveragePositiveTests = averageNumberOfPositiveTests.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void averageNumberOfTotalTestsPerDayMonthly(StackedStringArray givenList, MonthlyCovidStatistics statisticsToWriteTo)
        {
            int totalNumberOfTotalTests = 0;
            int numberOfDaysCounted = 0;

            foreach (List<string> currentList in givenList.getListOfList())
            {
                totalNumberOfTotalTests += int.Parse(currentList.ElementAt(PositiveIncrease)) + int.Parse(currentList.ElementAt(NegativeIncrease));
                numberOfDaysCounted++;
            }

            if (numberOfDaysCounted == 0)
            {
                statisticsToWriteTo.NumberAverageTotalTests = "-1";
            }
            else
            {
                double averageNumberOfTotalTests = Convert.ToDouble(totalNumberOfTotalTests) / Convert.ToDouble(numberOfDaysCounted);
                statisticsToWriteTo.NumberAverageTotalTests = averageNumberOfTotalTests.ToString(CultureInfo.InvariantCulture);
            }
        }
        
        private List<List<string>> getStackedStringList()
        {
            return this.covidInformationList.getListOfList();
        }

        private int getMonthValue(string date)
        {
            return int.Parse(date.Substring(4, 2));
        }

        private int getDayValue(string date)
        {
            return int.Parse(date.Substring(6, 2));
        }

        public int getNumberOfMonthsInData()
        {
            return this.theProcessedMonthlyCovidStatisticses.Count;
        }
    }
}