using Orchard.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Orchard
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="camel"></param>
        /// <returns></returns>
        public static string CamelFriendly(this string camel)
        {
            if (String.IsNullOrWhiteSpace(camel))
                return "";

            var sb = new StringBuilder(camel);

            for (int i = camel.Length - 1; i > 0; i--)
            {
                if (char.IsUpper(sb[i]))
                {
                    sb.Insert(i, ' ');
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="characterCount"></param>
        /// <returns></returns>
        public static string Ellipsize(this string text, int characterCount)
        {
            return text.Ellipsize(characterCount, "\u00A0\u2026");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="characterCount"></param>
        /// <param name="ellipsis"></param>
        /// <param name="wordBoundary"></param>
        /// <returns></returns>
        public static string Ellipsize(this string text, int characterCount, string ellipsis, bool wordBoundary = false)
        {
            if (String.IsNullOrWhiteSpace(text))
                return "";

            if (characterCount < 0 || text.Length <= characterCount)
                return text;

            // search beginning of word
            var backup = characterCount;
            while (characterCount > 0 && text[characterCount - 1].IsLetter())
            {
                characterCount--;
            }

            // search previous word
            while (characterCount > 0 && text[characterCount - 1].IsSpace())
            {
                characterCount--;
            }

            // if it was the last word, recover it, unless boundary is requested
            if (characterCount == 0 && !wordBoundary)
            {
                characterCount = backup;
            }

            var trimmed = text.Substring(0, characterCount);
            return trimmed + ellipsis;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HtmlClassify(this string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return "";

            var friendlier = text.CamelFriendly();

            var result = new char[friendlier.Length];

            var cursor = 0;
            var previousIsNotLetter = false;
            for (var i = 0; i < friendlier.Length; i++)
            {
                char current = friendlier[i];
                if (IsLetter(current) || (Char.IsDigit(current) && cursor > 0))
                {
                    if (previousIsNotLetter && i != 0 && cursor > 0)
                    {
                        result[cursor++] = '-';
                    }

                    result[cursor++] = Char.ToLowerInvariant(current);
                    previousIsNotLetter = false;
                }
                else
                {
                    previousIsNotLetter = true;
                }
            }

            return new string(result, 0, cursor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static LocalizedString OrDefault(this string text, LocalizedString defaultValue)
        {
            return String.IsNullOrEmpty(text)
                ? defaultValue
                : new LocalizedString(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="htmlDecode"></param>
        /// <returns></returns>
        public static string RemoveTags(this string html, bool htmlDecode = false)
        {
            if (String.IsNullOrEmpty(html))
            {
                return String.Empty;
            }

            var result = new char[html.Length];

            var cursor = 0;
            var inside = false;
            for (var i = 0; i < html.Length; i++)
            {
                char current = html[i];

                switch (current)
                {
                    case '<':
                        inside = true;
                        continue;
                    case '>':
                        inside = false;
                        continue;
                }

                if (!inside)
                {
                    result[cursor++] = current;
                }
            }

            var stringResult = new string(result, 0, cursor);

            if (htmlDecode)
            {
                stringResult = HttpUtility.HtmlDecode(stringResult);
            }

            return stringResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        // not accounting for only \r (e.g. Apple OS 9 carriage return only new lines)
        public static string ReplaceNewLinesWith(this string text, string replacement)
        {
            return String.IsNullOrWhiteSpace(text)
                       ? String.Empty
                       : text
                             .Replace("\r\n", "\r\r")
                             .Replace("\n", String.Format(replacement, "\r\n"))
                             .Replace("\r\r", String.Format(replacement, "\r\n"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length).
                Where(x => 0 == x % 2).
                Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).
                ToArray();
        }

        private static readonly char[] validSegmentChars = "/?#[]@\"^{}|`<>\t\r\n\f ".ToCharArray();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="segment"></param>
        /// <returns></returns>
        public static bool IsValidUrlSegment(this string segment)
        {
            // valid isegment from rfc3987 - http://tools.ietf.org/html/rfc3987#page-8
            // the relevant bits:
            // isegment    = *ipchar
            // ipchar      = iunreserved / pct-encoded / sub-delims / ":" / "@"
            // iunreserved = ALPHA / DIGIT / "-" / "." / "_" / "~" / ucschar
            // pct-encoded = "%" HEXDIG HEXDIG
            // sub-delims  = "!" / "$" / "&" / "'" / "(" / ")" / "*" / "+" / "," / ";" / "="
            // ucschar     = %xA0-D7FF / %xF900-FDCF / %xFDF0-FFEF / %x10000-1FFFD / %x20000-2FFFD / %x30000-3FFFD / %x40000-4FFFD / %x50000-5FFFD / %x60000-6FFFD / %x70000-7FFFD / %x80000-8FFFD / %x90000-9FFFD / %xA0000-AFFFD / %xB0000-BFFFD / %xC0000-CFFFD / %xD0000-DFFFD / %xE1000-EFFFD
            // 
            // rough blacklist regex == m/^[^/?#[]@"^{}|\s`<>]+$/ (leaving off % to keep the regex simple)

            return !segment.Any(validSegmentChars);
        }

        /// <summary>
        /// Generates a valid technical name.
        /// </summary>
        /// <remarks>
        /// Uses a white list set of chars.
        /// </remarks>
        public static string ToSafeName(this string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return String.Empty;

            name = RemoveDiacritics(name);
            name = name.Strip(c =>
                !c.IsLetter()
                && !Char.IsDigit(c)
                );

            name = name.Trim();

            // don't allow non A-Z chars as first letter, as they are not allowed in prefixes
            while (name.Length > 0 && !IsLetter(name[0]))
            {
                name = name.Substring(1);
            }

            if (name.Length > 128)
                name = name.Substring(0, 128);

            return name;
        }

        /// <summary>
        /// Generates a valid Html name.
        /// </summary>
        /// <remarks>
        /// Uses a white list set of chars.
        /// </remarks>
        public static string ToHtmlName(this string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return String.Empty;

            name = RemoveDiacritics(name);
            name = name.Strip(c =>
                c != '-'
                && c != '_'
                && !c.IsLetter()
                && !Char.IsDigit(c)
                );

            name = name.Trim();

            // don't allow non A-Z chars as first letter, as they are not allowed in prefixes
            while (name.Length > 0 && !IsLetter(name[0]))
            {
                name = name.Substring(1);
            }

            return name;
        }

        /// <summary>
        /// Whether the char is a letter between A and Z or not
        /// </summary>
        public static bool IsLetter(this char c)
        {
            return ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z');
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsSpace(this char c)
        {
            return (c == '\r' || c == '\n' || c == '\t' || c == '\f' || c == ' ');
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string name)
        {
            string stFormD = name.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char t in stFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(t);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(t);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="stripped"></param>
        /// <returns></returns>
        public static string Strip(this string subject, params char[] stripped)
        {
            if (stripped == null || stripped.Length == 0 || String.IsNullOrEmpty(subject))
            {
                return subject;
            }

            var result = new char[subject.Length];

            var cursor = 0;
            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (Array.IndexOf(stripped, current) < 0)
                {
                    result[cursor++] = current;
                }
            }

            return new string(result, 0, cursor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static string Strip(this string subject, Func<char, bool> predicate)
        {

            var result = new char[subject.Length];

            var cursor = 0;
            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (!predicate(current))
                {
                    result[cursor++] = current;
                }
            }

            return new string(result, 0, cursor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool Any(this string subject, params char[] chars)
        {
            if (string.IsNullOrEmpty(subject) || chars == null || chars.Length == 0)
            {
                return false;
            }

            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (Array.IndexOf(chars, current) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool All(this string subject, params char[] chars)
        {
            if (string.IsNullOrEmpty(subject))
            {
                return true;
            }

            if (chars == null || chars.Length == 0)
            {
                return false;
            }

            for (var i = 0; i < subject.Length; i++)
            {
                char current = subject[i];
                if (Array.IndexOf(chars, current) < 0)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static string Translate(this string subject, char[] from, char[] to)
        {
            if (string.IsNullOrEmpty(subject))
            {
                return subject;
            }

            if (from == null || to == null)
            {
                throw new ArgumentNullException();
            }

            if (from.Length != to.Length)
            {
                throw new ArgumentNullException("from", "Parameters must have the same length");
            }

            var map = new Dictionary<char, char>(from.Length);
            for (var i = 0; i < from.Length; i++)
            {
                map[from[i]] = to[i];
            }

            var result = new char[subject.Length];

            for (var i = 0; i < subject.Length; i++)
            {
                var current = subject[i];
                if (map.ContainsKey(current))
                {
                    result[i] = map[current];
                }
                else
                {
                    result[i] = current;
                }
            }

            return new string(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="original"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string ReplaceAll(this string original, IDictionary<string, string> replacements)
        {
            var pattern = String.Format("{0}", String.Join("|", replacements.Keys));
            return Regex.Replace(original, pattern, match => replacements[match.Value]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToBase64(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FromBase64(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }
        /// <summary>
        /// Adds a char to end of given string if it does not ends with the char.
        /// </summary>
        public static string EnsureEndsWith(this string str, char c)
        {
            return EnsureEndsWith(str, c, StringComparison.Ordinal);
        }

        /// <summary>
        /// Adds a char to end of given string if it does not ends with the char.
        /// </summary>
        public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType)
        {
            if (str == null)
            {
                throw new ArgumentNullException();
            }

            if (str.EndsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return str + c;
        }

        /// <summary>
        /// Adds a char to end of given string if it does not ends with the char.
        /// </summary>
        public static string EnsureEndsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
        {
            if (str == null)
            {
                throw new ArgumentNullException();
            }

            if (str.EndsWith(c.ToString(culture), ignoreCase, culture))
            {
                return str;
            }

            return str + c;
        }

        /// <summary>
        /// Adds a char to beginning of given string if it does not starts with the char.
        /// </summary>
        public static string EnsureStartsWith(this string str, char c)
        {
            return EnsureStartsWith(str, c, StringComparison.Ordinal);
        }

        /// <summary>
        /// Adds a char to beginning of given string if it does not starts with the char.
        /// </summary>
        public static string EnsureStartsWith(this string str, char c, StringComparison comparisonType)
        {
            if (str == null)
            {
                throw new ArgumentNullException();
            }

            if (str.StartsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return c + str;
        }

        /// <summary>
        /// Adds a char to beginning of given string if it does not starts with the char.
        /// </summary>
        public static string EnsureStartsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.StartsWith(c.ToString(culture), ignoreCase, culture))
            {
                return str;
            }

            return c + str;
        }

        /// <summary>
        /// Indicates whether this string is null or an System.String.Empty string.
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// indicates whether this string is null, empty, or consists only of white-space characters.
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Gets a substring of a string from beginning of the string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
        public static string Left(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(0, len);
        }

        /// <summary>
        /// Converts line endings in the string to <see cref="Environment.NewLine"/>.
        /// </summary>
        public static string NormalizeLineEndings(this string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", System.Environment.NewLine);
        }

        /// <summary>
        /// Gets index of nth occurence of a char in a string.
        /// </summary>
        /// <param name="str">source string to be searched</param>
        /// <param name="c">Char to search in <see cref="str"/></param>
        /// <param name="n">Count of the occurence</param>
        public static int NthIndexOf(this string str, char c, int n)
        {
            if (str == null)
            {
                throw new ArgumentNullException();
            }

            var count = 0;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] != c)
                {
                    continue;
                }

                if ((++count) == n)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes first occurrence of the given postfixes from end of the given string.
        /// Ordering is important. If one of the postFixes is matched, others will not be tested.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="postFixes">one or more postfix.</param>
        /// <returns>Modified string or the same string if it has not any of given postfixes</returns>
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty)
            {
                return string.Empty;
            }

            if (postFixes == null || postFixes.Length == 0)
            {
                return str;
            }

            foreach (var postFix in postFixes)
            {
                if (str.EndsWith(postFix))
                {
                    return str.Left(str.Length - postFix.Length);
                }
            }

            return str;
        }

        /// <summary>
        /// Removes first occurrence of the given prefixes from beginning of the given string.
        /// Ordering is important. If one of the preFixes is matched, others will not be tested.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="preFixes">one or more prefix.</param>
        /// <returns>Modified string or the same string if it has not any of given prefixes</returns>
        public static string RemovePreFix(this string str, params string[] preFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty)
            {
                return string.Empty;
            }

            if (preFixes == null || preFixes.Length == 0)
            {
                return str;
            }

            foreach (var preFix in preFixes)
            {
                if (str.StartsWith(preFix))
                {
                    return str.Right(str.Length - preFix.Length);
                }
            }

            return str;
        }

        /// <summary>
        /// Gets a substring of a string from end of the string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
        public static string Right(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// Uses string.Split method to split given string by given separator.
        /// </summary>
        public static string[] Split(this string str, string separator)
        {
            return str.Split(new[] { separator }, StringSplitOptions.None);
        }

        /// <summary>
        /// Uses string.Split method to split given string by given separator.
        /// </summary>
        public static string[] Split(this string str, string separator, StringSplitOptions options)
        {
            return str.Split(new[] { separator }, options);
        }

        /// <summary>
        /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
        /// </summary>
        public static string[] SplitToLines(this string str)
        {
            return str.Split(System.Environment.NewLine);
        }

        /// <summary>
        /// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
        /// </summary>
        public static string[] SplitToLines(this string str, StringSplitOptions options)
        {
            return str.Split(System.Environment.NewLine, options);
        }

        /// <summary>
        /// Converts PascalCase string to camelCase string.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="invariantCulture">Invariant culture</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string str, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return invariantCulture ? str.ToLowerInvariant() : str.ToLower();
            }

            return (invariantCulture ? char.ToLowerInvariant(str[0]) : char.ToLower(str[0])) + str.Substring(1);
        }

        /// <summary>
        /// Converts PascalCase string to camelCase string in specified culture.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="culture">An object that supplies culture-specific casing rules</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToLower(culture);
            }

            return char.ToLower(str[0], culture) + str.Substring(1);
        }

        /// <summary>
        /// Converts given PascalCase/camelCase string to sentence (by splitting words by space).
        /// Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <param name="invariantCulture">Invariant culture</param>
        public static string ToSentenceCase(this string str, bool invariantCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return Regex.Replace(
                str,
                "[a-z][A-Z]",
                m => m.Value[0] + " " + (invariantCulture ? char.ToLowerInvariant(m.Value[1]) : char.ToLower(m.Value[1]))
            );
        }

        /// <summary>
        /// Converts given PascalCase/camelCase string to sentence (by splitting words by space).
        /// Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
        /// </summary>
        /// <param name="str">String to convert.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        public static string ToSentenceCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1], culture));
        }

        /// <summary>
        /// Converts string to enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Converts string to enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="value">String value to convert</param>
        /// <param name="ignoreCase">Ignore case</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static string ToMd5(this string str)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(str);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var hashByte in hashBytes)
                {
                    sb.Append(hashByte.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Converts camelCase string to PascalCase string.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="invariantCulture">Invariant culture</param>
        /// <returns>PascalCase of the string</returns>
        public static string ToPascalCase(this string str, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return invariantCulture ? str.ToUpperInvariant() : str.ToUpper();
            }

            return (invariantCulture ? char.ToUpperInvariant(str[0]) : char.ToUpper(str[0])) + str.Substring(1);
        }

        /// <summary>
        /// Converts camelCase string to PascalCase string in specified culture.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="culture">An object that supplies culture-specific casing rules</param>
        /// <returns>PascalCase of the string</returns>
        public static string ToPascalCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToUpper(culture);
            }

            return char.ToUpper(str[0], culture) + str.Substring(1);
        }

        /// <summary>
        /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        public static string Truncate(this string str, int maxLength)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            return str.Left(maxLength);
        }

        /// <summary>
        /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        /// It adds a "..." postfix to end of the string if it's truncated.
        /// Returning string can not be longer than maxLength.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        public static string TruncateWithPostfix(this string str, int maxLength)
        {
            return TruncateWithPostfix(str, maxLength, "...");
        }

        /// <summary>
        /// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        /// It adds given <paramref name="postfix"/> to end of the string if it's truncated.
        /// Returning string can not be longer than maxLength.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        public static string TruncateWithPostfix(this string str, int maxLength, string postfix)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty || maxLength == 0)
            {
                return string.Empty;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            if (maxLength <= postfix.Length)
            {
                return postfix.Left(maxLength);
            }

            return str.Left(maxLength - postfix.Length) + postfix;
        }

    }
}
