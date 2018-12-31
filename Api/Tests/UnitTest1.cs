using Api.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    public class MovingXmlTests
    {
        [Fact]
        public void GetSetTests()
        {
            MovingXml xml = new MovingXml();
            xml.Chanse = generateValues();
            Assert.NotEmpty(xml.positions);
            Assert.NotEmpty(xml.chanses);
            var dic = xml.Chanse;
            dic["10"] = 21;
            xml.Chanse = dic;
            Assert.Equal(21, xml.Chanse["10"]);
        }
        private Dictionary<string, int> generateValues()
        {
            Dictionary<string, int> test = new Dictionary<string, int>();
            for (int i = 0; i < 100; i++)
            {
                test.Add(i.ToString(), i);
            }
            return test;
        }
    }
}
