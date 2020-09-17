using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Covid19Analysis
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        #region Data members

        /// <summary>
        ///     The application height
        /// </summary>
        public const int ApplicationHeight = 355;

        /// <summary>
        ///     The application width
        /// </summary>
        public const int ApplicationWidth = 625;

        private readonly StackedStringArray parsedLinesFromFile;
        private String allTextInFile;
        private StorageFile selectedFile;
        private int monthOfFirstPositiveTest;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size { Width = ApplicationWidth, Height = ApplicationHeight };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(ApplicationWidth, ApplicationHeight));

            this.parsedLinesFromFile = new StackedStringArray();
            this.allTextInFile = "";
            this.monthOfFirstPositiveTest = 0;
        }

        #endregion

        #region

        private async void loadFile_Click(object sender, RoutedEventArgs e)
        {
            this.summaryTextBox.Text = "Load file was invoked." + Environment.NewLine;
            FileOpenPicker theFilePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail, SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            theFilePicker.FileTypeFilter.Add(".csv");
            theFilePicker.FileTypeFilter.Add(".txt");

            this.selectedFile = await theFilePicker.PickSingleFileAsync();
            this.allTextInFile = await FileIO.ReadTextAsync(this.selectedFile);
            
            this.organizeListOfStringsIntoListOfAttributesByCommas(this.organizeTextByLineIntoList());
            this.outputToSummaryBox();
        }

        private List<string> organizeTextByLineIntoList()
        {
            List<String> eachLineInFile = new List<string>();
            eachLineInFile.AddRange(this.allTextInFile.Split(Environment.NewLine));
            return eachLineInFile;
        }

        private void organizeListOfStringsIntoListOfAttributesByCommas(List<String> eachLineInFile)
        {
            for (int currentIndex = eachLineInFile.Count - 1; currentIndex >= 0; currentIndex--)
            {
                List<String> attributesFromFile = new List<string>();
                attributesFromFile.AddRange(eachLineInFile.ElementAt(currentIndex).Split(","));
                if (isAttributeXString(attributesFromFile, 1, "GA"))
                {
                    this.parsedLinesFromFile.addListToList(
                        new List<string>(eachLineInFile.ElementAt(currentIndex).Split(",")));
                }
            }
        }

        private Boolean isAttributeXString(List<String> theList, int attributeIndex, String attributeValue)
        {
            if (theList.Count > 1 && theList.ElementAt(attributeIndex).Equals(attributeValue))
            {
                return true;
            }
            return false;
        }

        private void outputToSummaryBox()
        {
            this.summaryTextBox.Text = prepareOutputString();
        }

        private String prepareOutputString()
        {
            String outputString = "";

            outputString += firstPositiveTestDate();
            outputString += Environment.NewLine + highestNumberOfPositiveTests();
            outputString += Environment.NewLine + highestNumberOfNegativeTests();
            outputString += Environment.NewLine + highestNumberOfDeaths();
            outputString += Environment.NewLine + highestNumberOfHospitalizations();
            outputString += Environment.NewLine + highestPercentOfPositiveTests();
            outputString += Environment.NewLine + averageNumberOfPositiveTestsPerDay();
            outputString += Environment.NewLine + overallPositivity();
            outputString += Environment.NewLine + numberOfDaysWithOverTwoThousandFiftyPositiveTests();
            outputString += Environment.NewLine + numberOfDaysWithUnderOneThousandPositiveTests();
            outputString += Environment.NewLine + iterateThroughEachMonth();

            return outputString;
        }

        private String stringToDateFormat(String theString)
        {
            theString = theString.Insert(4, "/");
            theString = theString.Insert(7, "/");
            return theString;
        }

        private String thousandsCommaPlacer(int theInt)
        {
            return theInt.ToString();
        }

        private int getMonthValue(String date)
        {
            return int.Parse(date.Substring(4, 2));
        }

        private int getDayValue(String date)
        {
            return int.Parse(date.Substring(6, 2));
        }

        private String numberEndingPlacer(int numberToAddPrefixTo)
        {
            if (numberToAddPrefixTo.ToString().EndsWith("11") || 
                numberToAddPrefixTo.ToString().EndsWith("12") || 
                numberToAddPrefixTo.ToString().EndsWith("13"))
            {
                return numberToAddPrefixTo + "th";
            } else if (numberToAddPrefixTo.ToString().EndsWith("1"))
            {
                return numberToAddPrefixTo + "st";
            } else if (numberToAddPrefixTo.ToString().EndsWith("2"))
            {
                return numberToAddPrefixTo + "nd";
            } else if (numberToAddPrefixTo.ToString().EndsWith("3"))
            {
                return numberToAddPrefixTo + "rd";
            } else
            {
                return numberToAddPrefixTo + "th";
            }
        }

        private String getMonthStringFromInt(int monthNumber)
        {
            if (monthNumber == 1)
            {
                return "January";
            } else if (monthNumber == 2)
            {
                return "February";
            } else if (monthNumber == 3)
            {
                return "March";
            } else if (monthNumber == 4)
            {
                return "April";
            } else if (monthNumber == 5)
            {
                return "May";
            } else if (monthNumber == 6)
            {
                return "June";
            } else if (monthNumber == 7)
            {
                return "July";
            } else if (monthNumber == 8)
            {
                return "August";
            } else if (monthNumber == 9)
            {
                return "September";
            } else if (monthNumber == 10)
            {
                return "October";
            } else if (monthNumber == 11)
            {
                return "November";
            } else
            {
                return "December";
            }
        }

        private String firstPositiveTestDate()
        {
            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
            {
                if (!currentList.ElementAt(2).Equals("0"))
                {
                    this.monthOfFirstPositiveTest = getMonthValue(currentList.ElementAt(0));
                    return this.stringToDateFormat(currentList.ElementAt(0)) 
                        + " is the date with the first instance of a positive test.";
                }
            }
            return "There has not been a positive test.";
        }

        private String highestNumberOfPositiveTests()
        {
            String unformattedDateOfCurrentHighest = "00000000";
            int currentHighestNumberOfPositiveTests = int.MinValue;
            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
            {
                if (int.Parse(currentList.ElementAt(2)) > currentHighestNumberOfPositiveTests)
                {
                    unformattedDateOfCurrentHighest = currentList.ElementAt(0);
                    currentHighestNumberOfPositiveTests = int.Parse(currentList.ElementAt(2));
                }
            }
            return this.stringToDateFormat(unformattedDateOfCurrentHighest) 
                + " has the highest number of positive tests: " + thousandsCommaPlacer(currentHighestNumberOfPositiveTests);
        }

        private String highestNumberOfNegativeTests()
        {
            String unformattedDateOfCurrentLowest = "00000000";
            int currentHighestNumberOfNegativeTests = int.MinValue;
            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
            {
                if (int.Parse(currentList.ElementAt(3)) > currentHighestNumberOfNegativeTests)
                {
                    unformattedDateOfCurrentLowest = currentList.ElementAt(0);
                    currentHighestNumberOfNegativeTests = int.Parse(currentList.ElementAt(3));
                }
            }
            return this.stringToDateFormat(unformattedDateOfCurrentLowest)
                + " has the highest number of negative tests: " + thousandsCommaPlacer(currentHighestNumberOfNegativeTests);
        }

        private String highestNumberOfDeaths()
        {
            String unformattedDateOfCurrentHighest = "00000000";
            int currentHighestNumber = int.MinValue;
            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
            {
                if (int.Parse(currentList.ElementAt(4)) > currentHighestNumber)
                {
                    unformattedDateOfCurrentHighest = currentList.ElementAt(0);
                    currentHighestNumber = int.Parse(currentList.ElementAt(4));
                }
            }
            return this.stringToDateFormat(unformattedDateOfCurrentHighest)
                + " has the highest number of deaths: " + thousandsCommaPlacer(currentHighestNumber);
        }

        private String highestNumberOfHospitalizations()
        {
            String unformattedDateOfCurrentHighest = "00000000";
            int currentHighestNumber = int.MinValue;
            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
            {
                if (int.Parse(currentList.ElementAt(5)) > currentHighestNumber)
                {
                    unformattedDateOfCurrentHighest = currentList.ElementAt(0);
                    currentHighestNumber = int.Parse(currentList.ElementAt(5));
                }
            }
            return this.stringToDateFormat(unformattedDateOfCurrentHighest)
                + " has the highest number of hospitalizations: " + thousandsCommaPlacer(currentHighestNumber);
        }

        private String averageNumberOfPositiveTestsPerDay()
        {
            int firstDayOfPositiveTests = 0;
            int totalNumberOfPositiveTests = 0;

            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
            {
                if (int.Parse(currentList.ElementAt(2)) > 0 && firstDayOfPositiveTests == 0)
                {
                    firstDayOfPositiveTests = int.Parse(currentList.ElementAt(0));
                    totalNumberOfPositiveTests += int.Parse(currentList.ElementAt(2));
                } else if (int.Parse(currentList.ElementAt(2)) > 0)
                {
                    totalNumberOfPositiveTests += int.Parse(currentList.ElementAt(2));
                }
            }

            int numberOfDaysCounted =
                int.Parse(this.parsedLinesFromFile.getListOfList().ElementAt(this.parsedLinesFromFile.getListOfList().Count - 1).ElementAt(0))
                - firstDayOfPositiveTests;

            return "The average number of positive tests since the first recorded positive test: " + (totalNumberOfPositiveTests / numberOfDaysCounted).ToString();
        }

        private String numberOfDaysWithOverTwoThousandFiftyPositiveTests()
        {
            int numberOfDays = 0;
            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
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
            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
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
            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
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
            return this.stringToDateFormat(highestPercentageDay) + " has the highest percentage of positive tests: " 
                + highestPercentage;
        }

        private String overallPositivity()
        {
            double numberOfPositiveTests = 0;
            double numberOfNegativeTests = 0;

            foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
            {
                numberOfPositiveTests += int.Parse(currentList.ElementAt(2));
                numberOfNegativeTests += int.Parse(currentList.ElementAt(3));
            }
            double overallPercentage = (numberOfPositiveTests / (numberOfNegativeTests + numberOfPositiveTests)) * 100;
            overallPercentage = Math.Round(overallPercentage, 2);
            return "The overall positivity of all tests: " + overallPercentage;
        }

        private String iterateThroughEachMonth()
        {
            String finalString = "";
            for (int currentMonth = this.monthOfFirstPositiveTest; currentMonth <= 12; currentMonth++)
            {
                List<List<String>> currentMonthList = new List<List<String>>();

                foreach (List<String> currentList in this.parsedLinesFromFile.getListOfList())
                {
                    if (this.getMonthValue(currentList.ElementAt(0)) == currentMonth)
                    {
                        currentMonthList.Add(currentList);
                    }
                    
                }
                if (!highestNumberOfPositiveTestsMonthly(currentMonthList).Equals("0")) 
                {
                    finalString += Environment.NewLine + Environment.NewLine + this.getMonthStringFromInt(currentMonth);
                    finalString += Environment.NewLine + highestNumberOfPositiveTestsMonthly(currentMonthList);
                    finalString += Environment.NewLine + lowestNumberOfPositiveTestsMonthly(currentMonthList);
                    finalString += Environment.NewLine + highestNumberOfTotalTestsMonthly(currentMonthList);
                    finalString += Environment.NewLine + lowestNumberOfTotalTestsMonthly(currentMonthList);
                    finalString += Environment.NewLine + averageNumberOfPositiveTestsPerDayMonthly(currentMonthList);
                    finalString += Environment.NewLine + averageNumberOfTotalTestsPerDayMonthly(currentMonthList);

                }               
            }
            return finalString;
        }

        private String highestNumberOfPositiveTestsMonthly(List<List<String>> givenList)
        {
            String unformattedDateOfCurrentHighest = "00000000";
            int currentHighestNumberOfPositiveTests = int.MinValue;
            foreach (List<String> currentList in givenList)
            {
                if (int.Parse(currentList.ElementAt(2)) > currentHighestNumberOfPositiveTests)
                {
                    unformattedDateOfCurrentHighest = currentList.ElementAt(0);
                    currentHighestNumberOfPositiveTests = int.Parse(currentList.ElementAt(2));
                }
            }
            if (currentHighestNumberOfPositiveTests == int.MinValue)
            {
                return "0";
            }
            return "The " + this.numberEndingPlacer(this.getDayValue(unformattedDateOfCurrentHighest))
                + " has the highest number of positive tests: " + thousandsCommaPlacer(currentHighestNumberOfPositiveTests);
        }

        private String lowestNumberOfPositiveTestsMonthly(List<List<String>> givenList)
        {
            String unformattedDateOfCurrentLowest = "00000000";
            int currentLowestNumberOfPositiveTests = int.MaxValue;
            foreach (List<String> currentList in givenList)
            {
                if (int.Parse(currentList.ElementAt(2)) < currentLowestNumberOfPositiveTests)
                {
                    unformattedDateOfCurrentLowest = currentList.ElementAt(0);
                    currentLowestNumberOfPositiveTests = int.Parse(currentList.ElementAt(2));
                }
            }
            
            return "The " + this.numberEndingPlacer(this.getDayValue(unformattedDateOfCurrentLowest))
                + " has the lowest number of positive tests: " + thousandsCommaPlacer(currentLowestNumberOfPositiveTests);
        }

        private String highestNumberOfTotalTestsMonthly(List<List<String>> givenList)
        {
            String unformattedDateOfCurrentHighest = "00000000";
            int currentHighestNumberOfTests = int.MinValue;
            foreach (List<String> currentList in givenList)
            {
                if (int.Parse(currentList.ElementAt(2)) + int.Parse(currentList.ElementAt(3)) > currentHighestNumberOfTests)
                {
                    unformattedDateOfCurrentHighest = currentList.ElementAt(0);
                    currentHighestNumberOfTests = int.Parse(currentList.ElementAt(2)) + int.Parse(currentList.ElementAt(3));
                }
            }
            
            return "The " + this.numberEndingPlacer(this.getDayValue(unformattedDateOfCurrentHighest))
                + " has the highest number of total tests: " + thousandsCommaPlacer(currentHighestNumberOfTests);
        }

        private String lowestNumberOfTotalTestsMonthly(List<List<String>> givenList)
        {
            String unformattedDateOfCurrentLowest = "00000000";
            int currentLowestNumberOfTests = int.MaxValue;
            foreach (List<String> currentList in givenList)
            {
                if (int.Parse(currentList.ElementAt(2)) + int.Parse(currentList.ElementAt(3)) < currentLowestNumberOfTests)
                {
                    unformattedDateOfCurrentLowest = currentList.ElementAt(0);
                    currentLowestNumberOfTests = int.Parse(currentList.ElementAt(2)) + int.Parse(currentList.ElementAt(3));
                }
            }

            return "The " + this.numberEndingPlacer(this.getDayValue(unformattedDateOfCurrentLowest))
                + " has the lowest number of total tests: " + thousandsCommaPlacer(currentLowestNumberOfTests);
        }

        private String averageNumberOfPositiveTestsPerDayMonthly(List<List<String>> givenList)
        {
            int totalNumberOfPositiveTests = 0;
            int numberOfDaysCounted = 0;

            foreach (List<String> currentList in givenList)
            {
                totalNumberOfPositiveTests += int.Parse(currentList.ElementAt(2));
                numberOfDaysCounted++;
            }

            if (numberOfDaysCounted == 0)
            {
                return "There have been no tests.";
            }
            return "The average number of positive tests: " +
                   (totalNumberOfPositiveTests / numberOfDaysCounted).ToString();
        }

        private String averageNumberOfTotalTestsPerDayMonthly(List<List<String>> givenList)
        {
            int totalNumberOfTotalTests = 0;
            int numberOfDaysCounted = 0;

            foreach (List<String> currentList in givenList)
            {
                totalNumberOfTotalTests += int.Parse(currentList.ElementAt(2)) + int.Parse(currentList.ElementAt(3));
                numberOfDaysCounted++;
            }

            if (numberOfDaysCounted == 0)
            {
                return "There have been no tests.";
            }
            return "The average number of total tests: " + (totalNumberOfTotalTests / numberOfDaysCounted).ToString();
        }


        #endregion
    }
}
