using AIS1;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.AccessControl;
namespace ObjectCreatorTests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void ParsingCorrectDescription()
        {
            string descr = "MeterReadings ResourseType \"Cold       Water\" Date 2013.11.12 Value 2003.2";
            ObjectCreator creator = new ObjectCreator();
            List<BaseMeter> objects=creator.CreateObjects(descr);
            Assert.AreEqual(objects[0].Write(), $"Название: Cold       Water Дата: 12.11.2013 Значение: 2003,2");
        }
        [TestMethod]
        public void ParsingIncorrectDescription()
        {
            string descr = "MeterReadings ResourseType \"Cold       Water\" Date 2013.11.12 Value value";
            ObjectCreator creator = new ObjectCreator();
             
            Assert.ThrowsException<System.FormatException>(()=> creator.CreateObjects(descr));
        }
    }
}
