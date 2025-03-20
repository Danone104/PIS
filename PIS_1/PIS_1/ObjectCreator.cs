using AIS1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AIS1
{
    public class ObjectParser()
    {

        private ObjectDescriptor descriptor;
        public ObjectParser(ObjectDescriptor descriptor) : this()
        {
            this.descriptor = descriptor;
        }
        public (MeterReadings, int) SetPropertyForMeterReadings(int position, string description)
        {
            MeterReadings prototype = new MeterReadings();
            ObjectDescriptor descriptor=new ObjectDescriptor();
            while (position + descriptor.CountSpaces(position, description) < description.Length)
            {
                if (descriptor.ParseWord(position, description).Item1 != "MeterReadings" & descriptor.ParseWord(position, description).Item1
                != "ManTemperature")
                {

                    string propertyName = descriptor.ParseWord(position, description).Item1;
                    PropertyInfo property = prototype.GetType().GetProperty(propertyName);
                    position = descriptor.ParseWord(position, description).Item2;
                    try
                    {
                        object propertyValue = Convert.ChangeType(descriptor.ParseWord(position, description).Item1, property.PropertyType, CultureInfo.InvariantCulture);
                        position = descriptor.ParseWord(position, description).Item2;
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

            return (prototype, position);


        }
        public (ManTemperature, int) SetPropertyForManTemperature(int position, string description)
        {
            ManTemperature prototype = new ManTemperature();
            ObjectDescriptor descriptor=new ObjectDescriptor();
            while (position + descriptor.CountSpaces(position, description) < description.Length)
            {

                if (descriptor.ParseWord(position, description).Item1 != "MeterReadings" & descriptor.ParseWord(position, description).Item1
                != "ManTemperature")
                {

                    string propertyName = descriptor.ParseWord(position, description).Item1;
                    position = descriptor.ParseWord(position, description).Item2;

                    if (propertyName == "Location")
                    {
                        try
                        {
                            (prototype.Location, position) = descriptor.ParseLocation(description, position);
                        }
                        catch (FormatException ex)
                        {
                            throw new FormatException($"Ошибка преобразования значения свойства '{propertyName}': {ex.Message}", ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            PropertyInfo property = prototype.GetType().GetProperty(propertyName);
                            object propertyValue = Convert.ChangeType(descriptor.ParseWord(position, description).Item1, property.PropertyType, CultureInfo.InvariantCulture);
                            position = descriptor.ParseWord(position, description).Item2;
                            property.SetValue(prototype, propertyValue, null);
                        }

                        catch (FormatException ex)
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
            return (prototype, position);


        }
        
    }
    public class ExceptionHandler
    {
        public ExceptionHandler(int pos, string descr,ObjectDescriptor descriptor)
        {
            char c = '\"';
            int wordsAfterSymbol = 0;
            int posAfterSymbol = pos;
            while (posAfterSymbol + descriptor.CountSpaces(posAfterSymbol, descr) < descr.Length)
            {

                posAfterSymbol = descriptor.ParseWord(posAfterSymbol, descr).Item2;
                wordsAfterSymbol++;

                if (wordsAfterSymbol > 5 || posAfterSymbol + descriptor.CountSpaces(posAfterSymbol, descr) >= descr.Length)
                {
                    string str = "";
                    while (pos <= posAfterSymbol)
                    {
                        str += descr[pos];
                        pos++;
                    }
                    throw new Exception($"Нет закрывающей ковычки на этом промежутке: {str}");
                }
                if (descriptor.ParseWord(posAfterSymbol, descr).Item1.Last() == c)
                {
                    break;
                }
            }
        }
    }
    public class ObjectDescriptor()
    {
       
        public (Location, int) ParseLocation(string description, int position)
        {

            string lat = ParseWord(position, description).Item1;
            position = ParseWord(position, description).Item2;
            string longitude = ParseWord(position, description).Item1;
            position = ParseWord(position, description).Item2;
            Location loc = new Location() { Lat = Convert.ToDouble(lat, CultureInfo.InvariantCulture), Longitude = Convert.ToDouble(longitude, CultureInfo.InvariantCulture) };
            return (loc, position);

        }
        public (string, int) ParseString(int pos, string descr)
        {
            {
                string str = "";
                char c = '\"';
                pos++;
                if (descr.Count(s => s == c) % 2 != 0)
                {
                    ExceptionHandler check=new ExceptionHandler(pos, descr,this);
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
        public (string, int) ParseWord(int pos, string descr)
        {
            pos += CountSpaces(pos, descr);
            string str = "";
            int index = pos;
            while (descr[pos] != ' ')
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
        public int CountObjects(string description, int position)
        {
            string name = "";
            int countObjects = 0;
            while (position + CountSpaces(position, description) < description.Length)
            {
                name = ParseWord(position, description).Item1;
                position = ParseWord(position, description).Item2;
                if (name == "MeterReadings" || name == "ManTemperature")
                {
                    countObjects++;
                }
            }
            return countObjects;


        }
        public int CountSpaces(int pos, string str)
        {
            int count = 0;

            while (str[pos + count] == ' ')
            {
                count++;
                if (pos + count >= str.Length)
                {
                    break;
                }

            }
            return count;
        }
    }

    }
    
    public class ObjectCreator
    {
        public List<BaseMeter> CreateObjects(string descr)
        {
            ObjectDescriptor descriptor=new ObjectDescriptor();
            ObjectParser parser = new ObjectParser(descriptor);
            int pos = 0;
            List<BaseMeter> objects = new List<BaseMeter>();   
            int currentCountObjects = 0;
            int countObjects= descriptor.CountObjects(descr, pos);
            while (currentCountObjects != countObjects)
            {
                string typeName = descriptor.ParseWord(pos, descr).Item1;
                pos= descriptor.ParseWord(pos, descr).Item2;
                if (typeName == "MeterReadings")
                {
                    BaseMeter obj = new BaseMeter();
                    (obj,pos) = parser.SetPropertyForMeterReadings(pos,descr);
                    objects.Add(obj);
                    currentCountObjects++;

                }
                if (typeName == "ManTemperature")
                {
                    BaseMeter obj = new BaseMeter();
                    (obj, pos) = parser.SetPropertyForManTemperature(pos, descr);
                    objects.Add(obj);
                    currentCountObjects++;

                }
                if (typeName != "MeterReadings" & typeName != "ManTemperature")

                {
                    throw new Exception($"Неизвестный тип: {typeName}");
                }
            }
            return objects;
            
        }
       
        
       
        
       

            }

        
    
   

