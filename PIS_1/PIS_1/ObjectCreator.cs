using AIS1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AIS1
{
    public class ObjectCreator
    {

        public BaseMeter CreateObject(string descr)
        {
            string description = descr;
            string typeName = description.Split(' ')[0];

            BaseMeter obj=new BaseMeter();

            if (typeName == "MeterReadings") 
            {
               
                obj=SetPropertyForMeterReadings(descr);
                return obj;

            }
            if (typeName == "ManTemperature")
            {
                
                obj = SetPropertyForManTemperature(descr);
                return obj;
            }
            else
            {
                return obj;
            }
            
        }
        private MeterReadings SetPropertyForMeterReadings( string description)
        {
            MeterReadings prototype = new MeterReadings();
            int position = FindPosAfterTypeName(description, description.Split(' ')[0]);
            
            while (position + CountSpaces(position, description) < description.Length)
            {
                string propertyName = ParseWord(position, description).Item1;
                PropertyInfo property = prototype.GetType().GetProperty(propertyName);
                position = ParseWord(position, description).Item2;
                object propertyValue = Convert.ChangeType(ParseWord(position, description).Item1, property.PropertyType, CultureInfo.InvariantCulture);
                position = ParseWord(position, description).Item2;
                property.SetValue(prototype, propertyValue, null);
                
                

            }
            
            return prototype;


        }
        private ManTemperature SetPropertyForManTemperature(string description)
        {
            ManTemperature prototype = new ManTemperature();
            int position = FindPosAfterTypeName(description, description.Split(' ')[0]);
            while (position + CountSpaces(position, description) < description.Length)
            {
                string propertyName = ParseWord(position, description).Item1;
                position = ParseWord(position, description).Item2;
                if (propertyName == "Location")
                {
                    prototype.Location = ParseLocation(description,position).Item1;
                    position= ParseLocation(description,position).Item2;
                }
                else
                {
                    PropertyInfo property = prototype.GetType().GetProperty(propertyName);
                    object propertyValue = Convert.ChangeType(ParseWord(position, description).Item1, property.PropertyType,CultureInfo.InvariantCulture);
                    position = ParseWord(position, description).Item2;
                    property.SetValue(prototype, propertyValue, null);
                    
                }
            }
            return prototype;


        }
        private (Location,int) ParseLocation(string description,int position)
        {
            string lat = ParseWord(position, description).Item1;
            position = ParseWord(position, description).Item2;
            string longitude = ParseWord(position, description).Item1;
            position = ParseWord(position, description).Item2;
            Location loc = new Location() { Lat = Convert.ToDouble(lat, CultureInfo.InvariantCulture), Longitude = Convert.ToDouble(longitude, CultureInfo.InvariantCulture) };
            return (loc, position);
        }
        
        private int CountSpaces(int pos,string str)
        {
            int count = 0;
            
            while (str[pos+count] ==' ')
            {
                count++;
                if (pos + count >= str.Length)
                {
                    break;
                }
                
            }
            return count;
        }
        private (string,int) ParseString(int pos,string descr)
        {
            string str = "";
            char c = '\"';
            pos++;
            while (descr[pos] !=c)
            {
                str += descr[pos];
                pos++;
            }
            pos++;
            return (str, pos);
        }
        private (string,int) ParseWord(int pos,string descr)
        {
            pos += CountSpaces(pos, descr);
            string str = "";
            int index = pos;
            while (descr[pos]!= ' ')
            {
                if (descr[pos] != '\"')
                {
                    str += descr[pos];
                    pos++;
                    index = pos;
                    if (index >= descr.Length)
                    {
                        break;
                    }
                }

                else
                {
                    return ParseString(pos, descr);
                }

            }
            return (str, pos);
        }
        private int FindPosAfterTypeName(string descr,string typeName)
        {
            string str = "";
            int pos = 0;
            
            foreach (char c in descr)
            {
                if (str == typeName)
                {
                    
                    pos++;
                    break;

                }
                else
                {
                    str += c;
                    pos++;
                }
                
            }
            return pos;
        }
      

                


            }

        }
    
   

