using AIS1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AIS1
{
    public class ObjectCreator
    {

        public List<BaseMeter> CreateObjects(string descr)
        {
            int pos = 0;
            List<BaseMeter> objects = new List<BaseMeter>();   
            int currentCountObjects = 0;
            int countObjects=CountObjects(descr, pos);
            while (currentCountObjects != countObjects)
            {
                string typeName = ParseWord(pos, descr).Item1;
                pos=ParseWord(pos, descr).Item2;
                if (typeName == "MeterReadings")
                {
                    BaseMeter obj = new BaseMeter();
                    (obj,pos) = SetPropertyForMeterReadings(pos,descr);
                    objects.Add(obj);
                    currentCountObjects++;

                }
                if (typeName == "ManTemperature")
                {
                    BaseMeter obj = new BaseMeter();
                    (obj, pos) = SetPropertyForManTemperature(pos, descr);
                    objects.Add(obj);
                    currentCountObjects++;

                }
                else
                {
                    throw new Exception($"Неизвестный тип");
                }
            }
            return objects;
            
        }
        private int CountObjects(string description,int position)
        {
            string name = "";
            int countObjects = 0;
            while (position + CountSpaces(position, description) < description.Length)
            {
                name=ParseWord(position, description).Item1;
                position=ParseWord(position, description).Item2;
                if (name == "MeterReadings" || name == "ManTemperature")
                {
                    countObjects++; 
                }
            }
            return countObjects;
             

        }
        private (MeterReadings,int) SetPropertyForMeterReadings(int position, string description)
        {
            MeterReadings prototype = new MeterReadings();
            
            while (position + CountSpaces(position, description) < description.Length)
            {
                if (ParseWord(position, description).Item1 != "MeterReadings" & ParseWord(position, description).Item1
                != "ManTemperature")
                {
                    
                        string propertyName = ParseWord(position, description).Item1;
                        PropertyInfo property = prototype.GetType().GetProperty(propertyName);
                        position = ParseWord(position, description).Item2;
                        try
                        {
                            object propertyValue = Convert.ChangeType(ParseWord(position, description).Item1, property.PropertyType, CultureInfo.InvariantCulture);
                        position = ParseWord(position, description).Item2;
                        property.SetValue(prototype, propertyValue, null);
                        }
                        catch (FormatException ex)
                        {
                        throw new FormatException($"Ошибка преобразования значения свойства '{propertyName}': {ex.Message}", ex);
                        }

                }
                else
                {
                    break;
                }
                
                

            }
            
            return (prototype,position);


        }
        private (ManTemperature,int) SetPropertyForManTemperature(int position,string description)
        {
            ManTemperature prototype = new ManTemperature();
            
            while (position + CountSpaces(position, description) < description.Length)
            {
                
                if (ParseWord(position, description).Item1 != "MeterReadings" & ParseWord(position, description).Item1
                != "ManTemperature")
                {
                    
                    string propertyName = ParseWord(position, description).Item1;
                    position = ParseWord(position, description).Item2;
                    
                    if (propertyName == "Location")
                    {
                        try
                        {
                            (prototype.Location, position) = ParseLocation(description, position);
                        }
                        catch(FormatException ex)
                        {
                            throw new FormatException($"Ошибка преобразования значения свойства '{propertyName}': {ex.Message}", ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            PropertyInfo property = prototype.GetType().GetProperty(propertyName);
                            object propertyValue = Convert.ChangeType(ParseWord(position, description).Item1, property.PropertyType, CultureInfo.InvariantCulture);
                            position = ParseWord(position, description).Item2;
                            property.SetValue(prototype, propertyValue, null);
                        }
                        
                        catch(FormatException ex)
                        {
                            throw new FormatException($"Ошибка преобразования значения свойства '{propertyName}': {ex.Message}", ex);
                        }

                    }
                }
                else
                {
                    break;
                }
                    
            }
            return (prototype,position);


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
            {
                string str = "";
                char c = '\"';
                pos++;
                if (descr.Count(s => s == c) % 2 != 0)
                {
                    CheckForException(pos, descr);
                }
                while (descr[pos] != c)
                {
                    str += descr[pos];
                    pos++;
                }
                pos++;
                return (str, pos);
            }
            
            
        }
        private void CheckForException(int pos,string descr)
        {
            char c = '\"';
            int wordsAfterSymbol = 0;
            int posAfterSymbol = pos;
            while (posAfterSymbol + CountSpaces(posAfterSymbol, descr) < descr.Length)
            {

                posAfterSymbol = ParseWord(posAfterSymbol, descr).Item2;
                wordsAfterSymbol++;

                if (wordsAfterSymbol > 5 || posAfterSymbol + CountSpaces(posAfterSymbol, descr) >= descr.Length)
                {
                    string str = "";
                    while (pos <= posAfterSymbol)
                    {
                        str += descr[pos];
                        pos++;
                    }
                    throw new Exception($"Нет закрывающей ковычки на этом промежутке: {str}");
                }
                if (ParseWord(posAfterSymbol, descr).Item1.Last() == c)
                {
                    break;
                }
            }
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
       
      

                


            }

        }
    
   

