using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.Web;
using Http.TestLibrary;

namespace UW.Authentication.AspNet.Tests
{
    public class ShibbolethHeaderHttpModuleTest
    {
        [Fact]
        public void IsShibbolethSession_ShibSessionIndexPopulated_ReturnsTrue()
        {
            using (var simulator = new HttpSimulator())
            {
                simulator.SetHeader("ShibSessionIndex", "_sdf239489sd9923482929");

                simulator.SimulateRequest();


                var module = new ShibbolethHeaderHttpModule();

                var expected = true;

                var actual = module.IsShibbolethSession(HttpContext.Current.Request);

                Assert.Equal(expected, actual);
            }


        }

        [Fact]
        public void IsShibbolethSession_ShibSessionIndexNull_ReturnsFalse()
        {

            using (var simulator = new HttpSimulator())
            {
                simulator.SimulateRequest();

                var module = new ShibbolethHeaderHttpModule();

                var expected = false;

                var actual = module.IsShibbolethSession(HttpContext.Current.Request);

                Assert.Equal(expected, actual);
            }


        }
    }
}
