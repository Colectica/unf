using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

[assembly: InternalsVisibleTo("Colectica.Unf.Tests")]
namespace Colectica.Unf
{
    public class Unf
    {

        private static readonly long NegativeZeroBits = BitConverter.DoubleToInt64Bits(-0.0);

        internal static string GetNormalization(double? value, int significantDigits = 7)
        {
            if (value == null) { return "\0\0\0"; }

            if (double.IsNaN(value.Value)) { return "+nan\n\0"; }

            if (double.IsNegativeInfinity(value.Value)) { return "-inf\n\0"; }
            if (double.IsInfinity(value.Value)) { return "+inf\n\0"; }

            var longBits = BitConverter.DoubleToInt64Bits(value.Value);
            if (longBits == NegativeZeroBits) { return "-0.e+\n\0"; }
            if (value.Equals(0.0d)) { return "+0.e+\n\0"; }



            double abs = Math.Abs(value.Value);
            var builder = new StringBuilder();
            builder = value.Value > 0 ? builder.Append("+") : builder.Append("-");

            string formatted;
            if(significantDigits == 7)
            {
                formatted = abs.ToString("0.######e+0").TrimEnd('0');
            }
            else
            {
                var f = new StringBuilder();
                f.Append("0.");
                for(int i = 1; i < significantDigits; ++i)
                {
                    f.Append("#");
                }
                f.Append("e+0");
                formatted = abs.ToString(f.ToString()).TrimEnd('0');
            }
            if (formatted[1] != '.')
            {
                formatted = formatted.Insert(1, ".");
            }
            builder.Append(formatted);
            builder.Append("\n\0");
            return builder.ToString();

            //var rounded = Math.Round(abs, significantDigits, MidpointRounding.ToEven);
            //double log10 = Math.Log10(abs);
            //double e = Math.Floor(log10);
            //double significand = abs / Math.Pow(10, e);
        }

        public static string CalculateDatasetSha(List<string> variableShas)
        {
            variableShas.Sort(StringComparer.Ordinal);
            return CalculateSha(variableShas);
        }

        internal static string GetNormalization(string value, int truncateTo = 128)
        {
            if (value == null) { return "\0\0\0"; }

            if (value.Length > truncateTo)
            {
                value = value.Substring(0, truncateTo);
            }

            return value + "\n\0";
        }

        internal static string GetNormalization(DateTimeOffset? value)
        {
            if (value == null) { return "\0\0\0"; }

            var result = new StringBuilder();
            if(value.Value.Offset.Ticks == 0)
            {
                result.Append(value.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFF", CultureInfo.InvariantCulture).TrimEnd('.'));
                result.Append("Z");
            }
            else
            {
                result.Append(value.Value.UtcDateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFF", CultureInfo.InvariantCulture).TrimEnd('.')); 
                result.Append("Z");
            }
            result.Append("\n\0");
            return result.ToString();
        }

        internal static string GetNormalization(DateTime? value)
        {
            if (value == null) { return "\0\0\0"; }

            var result = new StringBuilder();
            if (value.Value.Kind == DateTimeKind.Unspecified)
            {
                result.Append(value.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFF", CultureInfo.InvariantCulture).TrimEnd('.'));
            }
            else if(value.Value.Kind == DateTimeKind.Utc)
            {
                result.Append(value.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFF", CultureInfo.InvariantCulture).TrimEnd('.'));
                result.Append("Z");
            }
            else if(value.Value.Kind == DateTimeKind.Local)
            {
                result.Append(value.Value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFF", CultureInfo.InvariantCulture).TrimEnd('.'));
                result.Append("Z");
            }

            result.Append("\n\0");
            return result.ToString();
        }
        internal static string GetNormalization(bool? value)
        {
            if (value == null) { return "\0\0\0"; }

            if (value == true)
            {
                return "+1.e+\n\0";
            }
            return "+0.e+\n\0";
        }

#if NET6_0_OR_GREATER
        internal static string GetNormalization(DateOnly? value)
        {
            if (value == null) { return "\0\0\0"; }
            string result = value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return $"{result}\n\0";
        }

        internal static string GetNormalization(TimeOnly? value)
        {
            if (value == null) { return "\0\0\0"; }
            string result = value.Value.ToString("HH':'mm':'ss'.'FFFFF", CultureInfo.InvariantCulture).TrimEnd('.');
            return $"{result}\n\0";
        }

        public static string CalculateSha(IEnumerable<TimeOnly?> values, int truncateShaTo = 128)
        {
            if (truncateShaTo != 128 && truncateShaTo != 192 && truncateShaTo != 256)
            {
                throw new ArgumentException("Invalid sha truncation length", nameof(truncateShaTo));
            }

            var hasher = SHA256.Create();
            foreach (var value in values)
            {
                var normalized = GetNormalization(value);
                var bytes = Encoding.UTF8.GetBytes(normalized);
                hasher.TransformBlock(bytes, 0, bytes.Length, null, 0);
            }
            hasher.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hash = hasher.Hash;
            return Convert.ToBase64String(hash, 0, truncateShaTo / 8, Base64FormattingOptions.None);
        }
        public static string CalculateSha(IEnumerable<DateOnly?> values, int truncateShaTo = 128)
        {
            if (truncateShaTo != 128 && truncateShaTo != 192 && truncateShaTo != 256)
            {
                throw new ArgumentException("Invalid sha truncation length", nameof(truncateShaTo));
            }

            var hasher = SHA256.Create();
            foreach (var value in values)
            {
                var normalized = GetNormalization(value);
                var bytes = Encoding.UTF8.GetBytes(normalized);
                hasher.TransformBlock(bytes, 0, bytes.Length, null, 0);
            }
            hasher.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hash = hasher.Hash;
            return Convert.ToBase64String(hash, 0, truncateShaTo / 8, Base64FormattingOptions.None);
        }
#endif



        public static string CalculateSha(IEnumerable<DateTimeOffset?> values, int truncateShaTo = 128)
        {
            if (truncateShaTo != 128 && truncateShaTo != 192 && truncateShaTo != 256)
            {
                throw new ArgumentException("Invalid sha truncation length", nameof(truncateShaTo));
            }

            var hasher = SHA256.Create();
            foreach (var value in values)
            {
                var normalized = GetNormalization(value);
                var bytes = Encoding.UTF8.GetBytes(normalized);
                hasher.TransformBlock(bytes, 0, bytes.Length, null, 0);
            }
            hasher.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hash = hasher.Hash;
            return Convert.ToBase64String(hash, 0, truncateShaTo / 8, Base64FormattingOptions.None);
        }

        public static string CalculateSha(IEnumerable<DateTime?> values, int truncateShaTo = 128)
        {
            if (truncateShaTo != 128 && truncateShaTo != 192 && truncateShaTo != 256)
            {
                throw new ArgumentException("Invalid sha truncation length", nameof(truncateShaTo));
            }

            var hasher = SHA256.Create();
            foreach (var value in values)
            {
                var normalized = GetNormalization(value);
                var bytes = Encoding.UTF8.GetBytes(normalized);
                hasher.TransformBlock(bytes, 0, bytes.Length, null, 0);
            }
            hasher.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hash = hasher.Hash;
            return Convert.ToBase64String(hash, 0, truncateShaTo / 8, Base64FormattingOptions.None);
        }

        public static string CalculateSha(IEnumerable<string> values, int truncateTo = 128, int truncateShaTo = 128)
        {
            if(truncateShaTo != 128 && truncateShaTo != 192 && truncateShaTo != 256)
            {
                throw new ArgumentException("Invalid sha truncation length", nameof(truncateShaTo));
            }

            var hasher = SHA256.Create();
            foreach (var value in values)
            {
                var normalized = GetNormalization(value, truncateTo);
                var bytes = Encoding.UTF8.GetBytes(normalized);
                hasher.TransformBlock(bytes, 0, bytes.Length, null, 0);
            }
            hasher.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hash = hasher.Hash;
            return Convert.ToBase64String(hash, 0, truncateShaTo / 8, Base64FormattingOptions.None);
        }

        public static string CalculateSha(IEnumerable<double?> values, int significantDigits = 7, int truncateShaTo = 128)
        {
            if (truncateShaTo != 128 && truncateShaTo != 192 && truncateShaTo != 256)
            {
                throw new ArgumentException("Invalid sha truncation length", nameof(truncateShaTo));
            }

            var hasher = SHA256.Create();
            foreach (var value in values)
            {
                var normalized = GetNormalization(value, significantDigits);
                var bytes = Encoding.UTF8.GetBytes(normalized);
                hasher.TransformBlock(bytes, 0, bytes.Length, null, 0);
            }
            hasher.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hash = hasher.Hash;
            return Convert.ToBase64String(hash, 0, truncateShaTo / 8, Base64FormattingOptions.None);
        }

        public static string CalculateSha(IEnumerable<bool?> values, int truncateShaTo = 128)
        {
            if (truncateShaTo != 128 && truncateShaTo != 192 && truncateShaTo != 256)
            {
                throw new ArgumentException("Invalid sha truncation length", nameof(truncateShaTo));
            }

            var hasher = SHA256.Create();
            foreach (var value in values)
            {
                var normalized = GetNormalization(value);
                var bytes = Encoding.UTF8.GetBytes(normalized);
                hasher.TransformBlock(bytes, 0, bytes.Length, null, 0);
            }
            hasher.TransformFinalBlock(new byte[0], 0, 0);
            byte[] hash = hasher.Hash;
            return Convert.ToBase64String(hash, 0, truncateShaTo / 8, Base64FormattingOptions.None);
        }
    }

    
}