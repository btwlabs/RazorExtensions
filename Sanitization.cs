using System;
using System.Text.RegularExpressions;
using System.Globalization;

public class Sanitization
{
    
    public static string CleanCssIdentifier(string identifier, Dictionary<string, string> filter = null)
    {
        if (filter == null) 
        {
            filter = new Dictionary<string, string>
            {
                {" ", "-"},
                {"_", "-"},
                {"/", "-"},
                {"[", "-"},
                {"]", ""},
            };
        }

        int doubleUnderscoreReplacements = 0;
        if (!filter.ContainsKey("__"))
        {
            identifier = identifier.Replace("__", "##");
            doubleUnderscoreReplacements++;
        }

        foreach(var pair in filter)
        {
            identifier = identifier.Replace(pair.Key, pair.Value);
        }

        // Replace temporary placeholder '##' with '__' only if the original
        // identifier contained '__'.
        if (doubleUnderscoreReplacements > 0)
        {
            identifier = identifier.Replace("##", "__");
        }

        // Valid characters in a CSS identifier are:
        // - the hyphen (U+002D)
        // - a-z (U+0030 - U+0039)
        // - A-Z (U+0041 - U+005A)
        // - the underscore (U+005F)
        // - 0-9 (U+0061 - U+007A)
        // - ISO 10646 characters U+00A1 and higher
        // We strip out any character not in the above list.
        identifier = Regex.Replace(identifier, @"[^\x002D\x0030-\x0039\x0041-\x005A\x005F\x0061-\x007A\x00A1-\uFFFF]", "");
        
        // Identifiers cannot start with a digit, two hyphens, or a hyphen followed by a digit.
        identifier = Regex.Replace(identifier, "^[0-9]|^(-[0-9])|^(--)", "_");
        return identifier;
    }
    
    public static string CleanID(string id)
    {
        id = id.Replace(" ", "-").
            Replace("_", "-").
            Replace("[", "-").
            Replace("]", "-").
            ToLower(new CultureInfo("en-US", false));

        // As defined in http://www.w3.org/TR/html4/types.html#type-name, HTML IDs can
        // only contain letters, digits ([0-9]), hyphens ("-"), underscores ("_"),
        // colons (":"), and periods ("."). We strip out any character not in that
        // list. Note that the CSS spec doesn't allow colons or periods in identifiers
        // (http://www.w3.org/TR/CSS21/syndata.html#characters), so we strip those two
        // characters as well.
        id = Regex.Replace(id, "[^A-Za-z0-9\\-_]", "");

        // Removing multiple consecutive hyphens.
        id = Regex.Replace(id, "-+", "-");

        return id;
    }
}