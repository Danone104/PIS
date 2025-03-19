using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace AIS1
{
    public class Location
    {
        private double lat, longitude;
        public double Lat { get { return lat; } set { lat = value; } }
        public double Longitude { get { return longitude; } set { longitude = value; } }

    }

    public class ManTemperature : BaseMeter
    {
        private Location loc;
        public Location Location { get { return loc; } set { loc = value; } }
        private string name;
        public string Name { get { return name; } set { name = value; } }

        private DateTime date;
        public DateTime Date { get { return date; } set { date = value; } }
        private double value { get; set; }
        public double Value { get { return value; } set { this.value = value; } }
        public override string Write()
        {
            string Data = $"Имя:{Name} Широта: {Location.Lat} Долгота:{Location.Longitude}  Дата:{Date.ToString("d")} Значение :{Value}";
            return Data;
        }
    }
}
