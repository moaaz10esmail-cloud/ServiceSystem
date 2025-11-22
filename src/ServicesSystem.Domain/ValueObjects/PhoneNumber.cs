using System.Text.RegularExpressions;

namespace ServicesSystem.Domain.ValueObjects;

public class PhoneNumber
{
    public string CountryCode { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;

    public PhoneNumber() { }

    public PhoneNumber(string countryCode, string number)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            throw new ArgumentException("Country code cannot be empty", nameof(countryCode));

        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be empty", nameof(number));

        // Remove any non-digit characters
        number = Regex.Replace(number, @"[^\d]", "");

        if (number.Length < 7 || number.Length > 15)
            throw new ArgumentException("Phone number must be between 7 and 15 digits", nameof(number));

        CountryCode = countryCode;
        Number = number;
    }

    public override string ToString()
    {
        return $"+{CountryCode} {Number}";
    }

    public string ToInternationalFormat()
    {
        return $"+{CountryCode}{Number}";
    }
}
