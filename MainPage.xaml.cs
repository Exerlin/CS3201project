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

        private readonly CovidInformationInterpreter theCovidInformation;

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

            this.theCovidInformation = new CovidInformationInterpreter();
        }

        #endregion

        #region

        private async void loadFile_Click(object sender, RoutedEventArgs e)
        {
            this.summaryTextBox.Text = "Load file was invoked." + Environment.NewLine;
            FileOpenPicker theFilePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            theFilePicker.FileTypeFilter.Add(".csv");
            theFilePicker.FileTypeFilter.Add(".txt");

            StorageFile selectedFile = await theFilePicker.PickSingleFileAsync();
            string allTextInFile = await FileIO.ReadTextAsync(selectedFile);

            this.theCovidInformation.addCovidInformation(this.organizeTextByLineIntoList(allTextInFile));
            this.outputToSummaryBox();
        }

        private List<string> organizeTextByLineIntoList(string allTextInFile)
        {
            List<string> eachLineInFile = new List<string>();
            eachLineInFile.AddRange(allTextInFile.Split(Environment.NewLine));
            return eachLineInFile;
        }

        private void outputToSummaryBox()
        {
            this.summaryTextBox.Text = prepareOutputString();
        }

        private string prepareOutputString()
        {
            return this.formatStatistics(this.theCovidInformation.getCovidStatistics());
        }

        private string formatStatistics(CovidStatistics theStatisticsToFormat)
        {
            string outputString = "The first positive test occurred on " +
                                  stringToDateFormat(theStatisticsToFormat.DateFirstPositiveTest);
            outputString += Environment.NewLine + "The highest number of positive tests was " +
                            thousandsCommaPlacer(theStatisticsToFormat.NumberPositiveTestsHighest)
                            + " on " + stringToDateFormat(theStatisticsToFormat.DatePositiveTestsHighest);
            outputString += Environment.NewLine + "The highest number of negative tests was " +
                            thousandsCommaPlacer(theStatisticsToFormat.NumberNegativeTestsHighest)
                                                 + " on " + stringToDateFormat(theStatisticsToFormat.DateNegativeTestsHighest);
            outputString += Environment.NewLine + "The highest number of all tests was " +
                            thousandsCommaPlacer(theStatisticsToFormat.NumberAllTestsHighest)
                                                 + " on " + stringToDateFormat(theStatisticsToFormat.DateAllTestsHighest);
            outputString += Environment.NewLine + "The highest number of deaths was " +
                            thousandsCommaPlacer(theStatisticsToFormat.NumberDeathsHighest)
                                                 + " on " + stringToDateFormat(theStatisticsToFormat.DateDeathsHighest);
            outputString += Environment.NewLine + "The highest number of hospitalizations was " +
                            thousandsCommaPlacer(theStatisticsToFormat.NumberHospitalizationsHighest)
                                                 + " on " + stringToDateFormat(theStatisticsToFormat.DateHospitalizationsHighest);
            outputString += Environment.NewLine + "The highest percentage of positive tests was " +
                            theStatisticsToFormat.NumberPositiveTestsHighestPercent
                                                 + " on " + stringToDateFormat(theStatisticsToFormat.DatePositiveTestsHighestPercent);

            outputString += Environment.NewLine + "The average number of positive tests are " +
                            thousandsCommaPlacer(theStatisticsToFormat.AverageNumberOfPositiveTests);
            outputString += Environment.NewLine + "The overall positivity of the tests are " +
                            thousandsCommaPlacer(theStatisticsToFormat.OverallPositivityRate);
            outputString += Environment.NewLine + "The number of days with more than 2500 positive tests are " +
                            thousandsCommaPlacer(theStatisticsToFormat.NumberOfDaysPositiveTestsAboveThreshold);
            outputString += Environment.NewLine + "The number of days with less than 1000 positive tests are " +
                            thousandsCommaPlacer(theStatisticsToFormat.NumberOfDaysPositiveTestsBelowThreshold);
            outputString += formatHistogramData(theStatisticsToFormat);

            outputString += formatMonthlyStatistics();

            return outputString;
        }

        private string formatMonthlyStatistics()
        {
            string outputString = "";
            for (int currentMonthIndex = 0;
                currentMonthIndex < this.theCovidInformation.getNumberOfMonthsInData();
                currentMonthIndex++)
            {
                MonthlyCovidStatistics currentMonthlyCovidStatistics =
                    this.theCovidInformation.getMonthlyCovidStatistics(currentMonthIndex);
                outputString += Environment.NewLine + Environment.NewLine
                                + getMonthStringFromInt(currentMonthlyCovidStatistics.Month)
                                + " " + currentMonthlyCovidStatistics.Year + " (" 
                                + currentMonthlyCovidStatistics.NumberOfDaysContainingData 
                                + " days of data):";

                outputString += Environment.NewLine
                                + "Highest number of positive tests: " 
                                + thousandsCommaPlacer(currentMonthlyCovidStatistics.NumberHighestPositiveTests)
                                + " occurred on the " + multipleDaysFormat(currentMonthlyCovidStatistics.DateHighestPositiveTests) 
                                + ".";
                outputString += Environment.NewLine
                                + "Lowest number of positive tests: " 
                                + thousandsCommaPlacer(currentMonthlyCovidStatistics.NumberLowestPositiveTests)
                                + " occurred on the " + multipleDaysFormat(currentMonthlyCovidStatistics.DateLowestPositiveTests)
                                + ".";
                outputString += Environment.NewLine
                                + "Highest number of total tests: " 
                                + thousandsCommaPlacer(currentMonthlyCovidStatistics.NumberHighestTotalTests)
                                + " occurred on the " + multipleDaysFormat(currentMonthlyCovidStatistics.DateHighestTotalTests)
                                + ".";
                outputString += Environment.NewLine
                                + "Lowest number of total tests: " 
                                + thousandsCommaPlacer(currentMonthlyCovidStatistics.NumberLowestTotalTests)
                                + " occurred on the " + multipleDaysFormat(currentMonthlyCovidStatistics.DateLowestTotalTests)
                                + ".";

                outputString += Environment.NewLine
                                + "Average number of positive tests: " 
                                + thousandsCommaPlacer(currentMonthlyCovidStatistics.NumberAveragePositiveTests);
                outputString += Environment.NewLine
                                + "Average number of all tests: " 
                                + thousandsCommaPlacer(currentMonthlyCovidStatistics.NumberAverageTotalTests);
            }
            return outputString;
        }

        private string formatHistogramData(CovidStatistics theStatisticsToFormat)
        {
            string outputString = Environment.NewLine;
            int currentIndex = 0;
            foreach (var currentNumberOfDays in theStatisticsToFormat.HistogramDataContents)
            {
                if (currentNumberOfDays == theStatisticsToFormat.HistogramDataContents.First())
                {
                    outputString += Environment.NewLine 
                                    + "0 - 500: " 
                                    + thousandsCommaPlacer(currentNumberOfDays);
                }
                else
                {

                    outputString += Environment.NewLine 
                                    + thousandsCommaPlacer((currentIndex * 500) + 1) + " - " 
                                    + thousandsCommaPlacer((currentIndex * 500) + 500) + ": " 
                                    + thousandsCommaPlacer(currentNumberOfDays);
                }

                currentIndex++;
            }

            return outputString;
        }

        private string multipleDaysFormat(List<string> theDaysToFormat)
        {
            if (theDaysToFormat.Count == 1)
            {
                return numberSuffixPlacer(this.getDayValue(theDaysToFormat.ElementAt(0)));
            }

            string outputString = "";
            foreach (var currentDate in theDaysToFormat)
            {
                if (theDaysToFormat.First() == currentDate)
                {
                    outputString += numberSuffixPlacer(this.getDayValue(currentDate));
                }
                else
                {
                    outputString += " and " + numberSuffixPlacer(this.getDayValue(currentDate));
                }
            }

            return outputString;
        }

        private string stringToDateFormat(string theString)
        {
            try
            {
                theString = theString.Insert(4, "/");
                theString = theString.Insert(7, "/");
                theString = DateTime.Parse(theString).Date.ToString("d");
                return theString;
            }
            catch (ArgumentOutOfRangeException)
            {
                return "[No Date]";
            }

        }

        private string thousandsCommaPlacer(int intToPlaceCommaIn)
        {
            return $"{intToPlaceCommaIn:n0}";
        }

        private string thousandsCommaPlacer(string stringNumberToPlaceCommaIn)
        {
            if (stringNumberToPlaceCommaIn.Equals(""))
            {
                return "0";
            }
            if (int.TryParse(stringNumberToPlaceCommaIn, out _))
            {
                return $"{double.Parse(stringNumberToPlaceCommaIn):n0}";
            } 
            return $"{double.Parse(stringNumberToPlaceCommaIn):n}";
        }

        private int getMonthValue(string date)
        {
            return int.Parse(date.Substring(4, 2));
        }

        private int getDayValue(string date)
        {
            return int.Parse(date.Substring(6, 2));
        }

        private string numberSuffixPlacer(int numberToAddPrefixTo)
        {
            if (numberToAddPrefixTo.ToString().EndsWith("11") ||
                numberToAddPrefixTo.ToString().EndsWith("12") ||
                numberToAddPrefixTo.ToString().EndsWith("13"))
            {
                return numberToAddPrefixTo + "th";
            }
            else if (numberToAddPrefixTo.ToString().EndsWith("1"))
            {
                return numberToAddPrefixTo + "st";
            }
            else if (numberToAddPrefixTo.ToString().EndsWith("2"))
            {
                return numberToAddPrefixTo + "nd";
            }
            else if (numberToAddPrefixTo.ToString().EndsWith("3"))
            {
                return numberToAddPrefixTo + "rd";
            }
            else
            {
                return numberToAddPrefixTo + "th";
            }
        }

        private string getMonthStringFromInt(int monthNumber)
        {
            if (monthNumber == 1)
            {
                return "January";
            }
            else if (monthNumber == 2)
            {
                return "February";
            }
            else if (monthNumber == 3)
            {
                return "March";
            }
            else if (monthNumber == 4)
            {
                return "April";
            }
            else if (monthNumber == 5)
            {
                return "May";
            }
            else if (monthNumber == 6)
            {
                return "June";
            }
            else if (monthNumber == 7)
            {
                return "July";
            }
            else if (monthNumber == 8)
            {
                return "August";
            }
            else if (monthNumber == 9)
            {
                return "September";
            }
            else if (monthNumber == 10)
            {
                return "October";
            }
            else if (monthNumber == 11)
            {
                return "November";
            }
            else
            {
                return "December";
            }
        }

        #endregion
    }
}
