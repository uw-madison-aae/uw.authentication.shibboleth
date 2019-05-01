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

        [Fact]
        public void GetAttributesFromRequest_5AttributesNoExtras_Returns5Attributes()
        {
            using (var simulator = new HttpSimulator())
            {

                simulator.SetHeader("uid", "bbadger");
                simulator.SetHeader("mail", "bucky.badger@wisc.edu");
                simulator.SetHeader("givenName", "Bucky");
                simulator.SetHeader("sn", "Badger");
                simulator.SetHeader("wiscEduPVI", "UW999B999");

                simulator.SimulateRequest();

                var module = new ShibbolethHeaderHttpModule();

                var expected = 5;

                var actual = module.GetAttributesFromRequest(HttpContext.Current.Request).Count;

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void GetAttributesFromRequest_5AttributesWithExtras_Returns5Attributes()
        {
            using (var simulator = new HttpSimulator())
            {

                simulator.SetHeader("uid", "bbadger");
                simulator.SetHeader("mail", "bucky.badger@wisc.edu");
                simulator.SetHeader("givenName", "Bucky");
                simulator.SetHeader("sn", "Badger");
                simulator.SetHeader("wiscEduPVI", "UW999B999");
                simulator.SetHeader("SERVER_NAME", "rocking.wisc.edu");
                simulator.SetHeader("REQUEST_METHOD", "GET");

                simulator.SimulateRequest();

                var module = new ShibbolethHeaderHttpModule();

                var expected = 5;

                var actual = module.GetAttributesFromRequest(HttpContext.Current.Request).Count;

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void GetAttributesFromRequest_CommonAttributes_EqualsHeaderValues()
        {
            using (var simulator = new HttpSimulator())
            {

                simulator.SetHeader("uid", "bbadger");
                simulator.SetHeader("mail", "bucky.badger@wisc.edu");
                simulator.SetHeader("givenName", "Bucky");
                simulator.SetHeader("sn", "Badger");
                simulator.SetHeader("wiscEduPVI", "UW999B999");

                simulator.SimulateRequest();

                var module = new ShibbolethHeaderHttpModule();


                var attribs = module.GetAttributesFromRequest(HttpContext.Current.Request);

                Assert.Equal(HttpContext.Current.Request.Headers["uid"], attribs["uid"].Value);
                Assert.Equal(HttpContext.Current.Request.Headers["mail"], attribs["mail"].Value);
                Assert.Equal(HttpContext.Current.Request.Headers["givenName"], attribs["givenName"].Value);
                Assert.Equal(HttpContext.Current.Request.Headers["sn"], attribs["sn"].Value);
                Assert.Equal(HttpContext.Current.Request.Headers["wiscEduPVI"], attribs["wiscEduPVI"].Value);
            }
        }

    }
}
