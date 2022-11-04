using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequisitionForm
{
    public class PlantLocation
    {
        private const string PlantListLocation = ".\\Data\\PlantList.txt";

        public string LocationName;
        public string PlantNum;
        public string Address;
        public string CityState;
        public string ZipCode;
        public string plantEMail;
        public string PlantPhoneNumber;

        [System.ComponentModel.TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string DisplayString { get; set; }

        public override string ToString()
        {
            return DisplayString;
        }

        public static List<PlantLocation> LoadPlantLocations()
        {
            List<PlantLocation> plantLocations = new List<PlantLocation>();

            using (StreamReader sr = new StreamReader(File.OpenRead(PlantListLocation)))
            {
                int lineNum = 0;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] lineSplitByTab = line.Split('\t');

                    PlantLocation PL = new PlantLocation();
                    PL.LocationName = lineSplitByTab[0];
                    PL.PlantNum = lineSplitByTab[1];
                    PL.Address = lineSplitByTab[2];
                    PL.CityState = lineSplitByTab[3];
                    PL.ZipCode = lineSplitByTab[4];
                    PL.plantEMail = lineSplitByTab[5];
                    PL.PlantPhoneNumber = lineSplitByTab[6];
                    PL.DisplayString = PL.CityState + " " + PL.PlantNum + " " + PL.plantEMail;
                    plantLocations.Add(PL);

                    lineNum++;
                }
            }

            return plantLocations;
        }
    }

    public class Product
    {
        public static Dictionary<string, int> CategoryDictionary = new Dictionary<string, int>();
        public static Dictionary<string, int> UNITDictionary = new Dictionary<string, int>();

        public int category = -1;
        public string ItemNum = "NOI";
        public string description = "NULL";
        public string VendorPartNum = "NOI";
        public int OrderUNIT = -1;
        public int OrderAmount = 0;
    }
}
