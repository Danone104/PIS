using AIS1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS1
{
    public class MeterReadings : BaseMeter
    {
        private string resourseType;
        public string ResourseType { get { return resourseType; } set { resourseType = value; } } // public?
        private DateTime date;
        public DateTime Date { get { return date; } set { date = value; } }
        private double value;
        public double Value { get { return value; } set { this.value = value; } }
        //public override void Read(string descr)
        //{

        //}
        public override string Write()
        {
            string Data = $"Название: {ResourseType} Дата: {Date.ToString("d")} Значение: {Value}";
            return Data;
        }
    }
}
