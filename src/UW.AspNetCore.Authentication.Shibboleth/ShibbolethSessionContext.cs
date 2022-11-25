using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace UW.AspNetCore.Authentication
{
    public class ShibbolethProcessorSelectionContext : ResultContext<ShibbolethOptions>
    {
        public ShibbolethProcessorSelectionContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ShibbolethOptions options)
            : base(context, scheme, options) 
        {

        }

    }
}
