using System;
using System.Collections.Generic;

namespace Covid19Analysis
{
    public class CovidStatistics
    {
        public CovidStatistics()
        {
            this.DateFirstPositiveTest = "";
            this.DatePositiveTestsHighest = "";
            this.DateNegativeTestsHighest = "";
            this.DateAllTestsHighest = "";
            this.DateDeathsHighest = "";
            this.DateHospitalizationsHighest = "";

            this.NumberPositiveTestsHighest = "";
            this.NumberNegativeTestsHighest = "";
            this.NumberAllTestsHighest = "";
            this.NumberDeathsHighest = "";
            this.NumberHospitalizationsHighest = "";

            this.DatePositiveTestsHighestPercent = "";
            this.NumberPositiveTestsHighestPercent = "";
            this.AverageNumberOfPositiveTests = "";
            this.OverallPositivityRate = "";
            this.NumberOfDaysPositiveTestsAboveThreshold = "";
            this.NumberOfDaysPositiveTestsBelowThreshold = "";

            this.HistogramDataContents = new List<string>();
        }

        public string DateFirstPositiveTest { get; set; }

        public string DatePositiveTestsHighest { get; set; }

        public string DateNegativeTestsHighest { get; set; }

        public string DateAllTestsHighest { get; set; }

        public string DateDeathsHighest { get; set; }

        public string DateHospitalizationsHighest { get; set; }

        public string NumberPositiveTestsHighest { get; set; }

        public string NumberNegativeTestsHighest { get; set; }

        public string NumberAllTestsHighest { get; set; }

        public string NumberDeathsHighest { get; set; }

        public string NumberHospitalizationsHighest { get; set; }

        public string DatePositiveTestsHighestPercent { get; set; }

        public string NumberPositiveTestsHighestPercent { get; set; }

        public string AverageNumberOfPositiveTests { get; set; }

        public string OverallPositivityRate { get; set; }

        public string NumberOfDaysPositiveTestsAboveThreshold { get; set; }

        public string NumberOfDaysPositiveTestsBelowThreshold { get; set; }

        public List<string> HistogramDataContents { get; set; }

        public string toString()
        {
            string stringToReturn = "";

            stringToReturn += this.DateFirstPositiveTest;
            stringToReturn += Environment.NewLine + this.DatePositiveTestsHighest + " " + this.NumberPositiveTestsHighest;
            stringToReturn += Environment.NewLine + this.DateNegativeTestsHighest + " " + this.NumberNegativeTestsHighest;
            stringToReturn += Environment.NewLine + this.DateDeathsHighest + " " + this.NumberDeathsHighest;
            stringToReturn += Environment.NewLine + this.DateHospitalizationsHighest + " " + this.NumberHospitalizationsHighest;

            return stringToReturn;
        }
    }
}
