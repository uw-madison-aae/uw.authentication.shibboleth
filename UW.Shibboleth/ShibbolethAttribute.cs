using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UW.Shibboleth
{
    /// <summary>
    /// concrete implementation of <see cref="IShibbolethAttribute"/>
    /// </summary>
    public class ShibbolethAttribute : IShibbolethAttribute
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
