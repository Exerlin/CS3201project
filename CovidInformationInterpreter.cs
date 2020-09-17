
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

        private StackedStringArray covidInformationList;
        public CovidInformationInterpreter()
        {
            this.covidInformationList = new StackedStringArray();
        }

        public void organizeListOfStringsIntoListOfAttributesByCommas(List<String> eachLineInFile)
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
        }

        private Boolean isAttributeXString(List<String> theList, int attributeIndex, String attributeValue)
        {
            if (theList.Count > 1 && theList.ElementAt(attributeIndex).Equals(attributeValue))
            {
                return true;
            }
            return false;
        }

        /**
         * Provides the CovidInformationInterpreter with a StackedStringArray containing
         * information about Covid.
         */
        public void addCovidInformation(StackedStringArray covidInformation)
        {
            this.covidInformationList = covidInformation;
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

        public string getHispitalizedIncrease(int valueIndex)
        {
            return this.covidInformationList.getString(HospitalizedIncrease, valueIndex);
        }
    }
}