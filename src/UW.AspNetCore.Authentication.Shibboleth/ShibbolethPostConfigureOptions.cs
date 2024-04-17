using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace UW.AspNetCore.Authentication;

public class ShibbolethPostConfigureOptions : IPostConfigureOptions<ShibbolethOptions>
{
    private readonly IDataProtectionProvider _dp;
    private readonly AuthenticationOptions _authOptions;

    /// <summary>
    /// Initializes the <see cref="ShibbolethPostConfigureOptions"/>
    /// </summary>
    /// <param name="authOptions"></param>
    /// <param name="dataProtection">The <see cref="IDataProtectionProvider"/></param>
    public ShibbolethPostConfigureOptions(
        IOptions<AuthenticationOptions> authOptions,
        IDataProtectionProvider dataProtection)
    {
        _dp = dataProtection;
        _authOptions = authOptions.Value;
    }

    public void PostConfigure(string? name, ShibbolethOptions options)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(name);
#else
        if (name == null) throw new ArgumentNullException(nameof(name));
#endif
        options.SignInScheme ??= _authOptions.DefaultSignInScheme ?? _authOptions.DefaultScheme;

        options.DataProtectionProvider ??= _dp;

        if (options.StateDataFormat == null)
        {
            IDataProtector dataProtector = options.DataProtectionProvider.CreateProtector(
                typeof(ShibbolethHandler).FullName!, name, "v1");
            options.StateDataFormat = new PropertiesDataFormat(dataProtector);
        }
    }
}
