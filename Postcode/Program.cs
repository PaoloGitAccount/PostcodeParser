using CsvHelper;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Postcode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting the postcode Parser");
            Postcode.Parser();
            //IConsoleWriter console = new ConsoleLog();
        }
    }

    public static class Postcode
    {
        private const string Pattern = "(^[A-Z]{1,2})([0-9]{1,2}[A-Z]{0,1})\\s?([0-9])([A-Z]{2}$)";
        private static readonly Lazy<Regex> LazyRegex = new Lazy<Regex>(() => new Regex(Pattern, RegexOptions.Compiled));

        private static readonly Regex Matcher = LazyRegex.Value;

        public static DestructuredPostcode Destructure(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode))
            {
                throw new ArgumentException("Postcode should not be empty", nameof(postcode));
            }

            var match = Matcher.Match(postcode);
            if (!IsValid(match))
            {
                throw new ArgumentException($"{postcode} is not a valid postcode", nameof(postcode));
            }

            return new DestructuredPostcode()
            {
                Outward = new Outward() { Area = match.Groups[1].Value, District = match.Groups[2].Value },
                Inward = new Inward() { Sector = Convert.ToInt16(match.Groups[3].Value), Unit = match.Groups[4].Value }
            };
        }

        private static bool IsValid(Match postcode)
        {
            if (postcode.Groups.Count != 5)
            {
                return false;
            }
            return true;
        }

        public static bool IsValid(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode)) return false;
            var match = Matcher.Match(postcode);
            return IsValid(match);
        }

        public static void Parser()
        {
            //Console.WriteLine("Starting the postcode Parser");
            Console.WriteLine($"Started at {DateTime.Now}");

            int counter = 0;
            using (var filestream = File.OpenText(Directory.GetCurrentDirectory() + @"..\..\..\..\Data\ukpostcodes.csv"))
            using (var csvReader = new CsvParser(filestream))
            using (var outstream = File.CreateText(Directory.GetCurrentDirectory() + @"..\..\..\..\Results\outfile.csv"))

            using (var csvWriter = new CsvWriter(outstream))
            {
                csvWriter.Configuration.Delimiter = "\r\n";
                csvWriter.Configuration.QuoteNoFields = true;
                var row = csvReader.Read(); // Header row
                while (true)
                {
                    row = csvReader.Read();
                    if (row == null) break;
                    counter++;
                    if (counter % 1000 == 0)
                    {
                        Console.CursorLeft = 0;
                        Console.Write(counter.ToString("N0"));
                    }
                    var postcode = row[0].ToUpper();
                    try
                    {
                        var destructured = Postcode.Destructure(postcode);
                        var outwardCode = destructured.Postcode.Substring(0, destructured.Postcode.Length - 3);
                        var outwardLetter = Regex.Match(outwardCode, @"^[^0-9]*").Value;
                        var outwardNumber = outwardCode.Substring(outwardLetter.Length); //Regex.Match(outwardCode, @"^*[^a-zA-Z]").Value;
                        csvWriter.WriteField("# POSTCODE:" + destructured.Postcode.Replace(" ", ""));
                        csvWriter.WriteField("        OUTWARD CODE:" + outwardCode);// destructured.Postcode.Substring(0, destructured.Postcode.Length - 3));
                        csvWriter.WriteField("             OUTWARD LETTER:" + outwardLetter);  //destructured.Outward.District);
                        csvWriter.WriteField("             OUTWARD NUMBER:" + outwardNumber);      //destructured.Inward.Sector);
                        csvWriter.WriteField("        INWARD CODE:" + destructured.Postcode[^3..]);  // .Substring(destructured.Postcode.Length - 3)
                        csvWriter.WriteField("");
                        csvWriter.NextRecord();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    outstream.Flush();
                }
            }
            Console.WriteLine("");
            Console.WriteLine("--- Done ---");
            Console.WriteLine("--- Completed the parsing of the ukpostcodes.csv file from the Data folder ---");
            Console.WriteLine("--- Added the formatted content in the outfile.csv file in the Results folder ---");
            Console.ReadKey();
        }
    }
}
