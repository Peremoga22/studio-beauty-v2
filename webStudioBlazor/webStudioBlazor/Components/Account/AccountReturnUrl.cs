namespace webStudioBlazor.Components.Account;

/// <summary>Після входу без явного ReturnUrl — особистий кабінет.</summary>
internal static class AccountReturnUrl
{
    public const string DefaultAfterLogin = "/account/profile";

    public static string GetSafeLocalReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
            return DefaultAfterLogin;

        var t = returnUrl.Trim();
        if (t.Contains("://", StringComparison.OrdinalIgnoreCase) || t.StartsWith("//", StringComparison.Ordinal))
            return DefaultAfterLogin;
        if (!t.StartsWith('/'))
            return DefaultAfterLogin;

        return t;
    }
}
