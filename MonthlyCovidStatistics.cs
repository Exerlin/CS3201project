using System.Collections.Generic;

namespace Covid19Analysis
{
    public class MonthlyCovidStatistics
    {

        public MonthlyCovidStatistics(int month, int year)
        {
            this.Month = month;
            this.Year = year;
            this.NumberOfDaysContainingData = 0;

            this.NumberHighestPositiveTests = "";
            this.DateHighestPositiveTests = new List<string>();
            this.NumberLowestPositiveTests = "";
            this.DateLowestPositiveTests = new List<string>();
            this.NumberHighestTotalTests = "";
            this.DateHighestTotalTests = new List<string>();
            this.NumberLowestTotalTests = "";
            this.DateLowestTotalTests = new List<string>();

            this.NumberAveragePositiveTests = "";
            this.NumberAverageTotalTests = "";
        }

        public int Month { get; set; }

        public int Year { get; set; }

        public int NumberOfDaysContainingData { get; set; }

        public string NumberHighestPositiveTests { get; set; }

        public List<string> DateHighestPositiveTests { get; set; }

        public string NumberLowestPositiveTests { get; set; }

        public List<string> DateLowestPositiveTests { get; set; }

        public string NumberHighestTotalTests { get; set; }

        public List<string> DateHighestTotalTests { get; set; }

        public string NumberLowestTotalTests { get; set; }

        public List<string> DateLowestTotalTests { get; set; }

        public string NumberAveragePositiveTests { get; set; }

        public string NumberAverageTotalTests { get; set; }
    }
}
