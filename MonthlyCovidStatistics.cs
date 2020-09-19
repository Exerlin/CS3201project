using System;
using System.Collections.Generic;

namespace Covid19Analysis
{
    public class MonthlyCovidStatistics
    {
        public MonthlyCovidStatistics()
        {
            this.Month = 0;
            this.Year = 0;
            this.NumberOfDaysContainingData = 0;

            this.NumberHighestPositiveTests = "";
            this.DateHighestPositiveTests = "";
            this.NumberLowestPositiveTests = "";
            this.DateLowestPositiveTests = "";
            this.NumberHighestTotalTests = "";
            this.DateHighestTotalTests = "";
            this.NumberLowestTotalTests = "";
            this.DateLowestTotalTests = "";

            this.NumberAveragePositiveTests = "";
            this.NumberAverageTotalTests = "";
        }

        public MonthlyCovidStatistics(int month, int year)
        {
            this.Month = month;
            this.Year = year;
            this.NumberOfDaysContainingData = 0;

            this.NumberHighestPositiveTests = "";
            this.DateHighestPositiveTests = "";
            this.NumberLowestPositiveTests = "";
            this.DateLowestPositiveTests = "";
            this.NumberHighestTotalTests = "";
            this.DateHighestTotalTests = "";
            this.NumberLowestTotalTests = "";
            this.DateLowestTotalTests = "";

            this.NumberAveragePositiveTests = "";
            this.NumberAverageTotalTests = "";
        }

        public int Month { get; set; }

        public int Year { get; set; }

        public int NumberOfDaysContainingData { get; set; }

        public string NumberHighestPositiveTests { get; set; }

        public string DateHighestPositiveTests { get; set; }

        public string NumberLowestPositiveTests { get; set; }

        public string DateLowestPositiveTests { get; set; }

        public string NumberHighestTotalTests { get; set; }

        public string DateHighestTotalTests { get; set; }

        public string NumberLowestTotalTests { get; set; }

        public string DateLowestTotalTests { get; set; }

        public string NumberAveragePositiveTests { get; set; }

        public string NumberAverageTotalTests { get; set; }
    }
}
