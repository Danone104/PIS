using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.AccessControl;
using System.Reflection;

namespace AIS1
{
    internal class Program
    {

        static void Main(string[] args)
        {

            ObjectCreator creator = new ObjectCreator();

            List<BaseMeter> list = new List<BaseMeter>();

            string descr1 = "MeterReadings ResourseType \"Cold       Water\" Date 2013.11.12 Value 2003.2 ManTemperature Name \"Pasha \" Date 2015.10.11 Value 36.7 Location 20.2 30.2 ";
            list = creator.CreateObjects(descr1);
            for(int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(list[i].Write());
            }
            //string descr2 = "ManTemperature Name \"Pasha \" Date 2015.10.11 Value 36.7 Location 20.2 30.2 ";
            //list.Add(creator.CreateObject(descr2));

            //string descr3 = "MeterReadings Date 2013.8.17 ResourseType \"Light\" Value 456789.2 ";
            
            //list.Add(creator.CreateObject(descr3));




          
        




        }
    }
}
