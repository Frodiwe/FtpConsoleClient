using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ftpConsoleClient.Methods
{
    /// <summary>
    /// Provides FTP LIST method
    /// </summary>
    class DirectoryList : AbstractFtpMethod
    {
        /// <summary>
        /// Attributes of items received from ftp LIST method (Unix: ls -l).
        /// </summary>
        /// <remarks>
        /// Only for Apache? servers, IIS returns data in other format (probably?)
        /// </remarks>
        private enum ItemAttributes
        {
            TypeMod = 0,
            RefCount = 1,
            OwnerName = 2,
            GroupName = 3,
            Size = 4,
            MonthStamp = 5,
            DayStamp = 6,
            TimeOrYearStamp = 7,
            Name = 8,
        }

        /// <summary>
        /// Captions to ItemAttributes
        /// </summary>
        private Dictionary<ItemAttributes, string> captionToItemAttributes = new Dictionary<ItemAttributes, string>
        {
            { ItemAttributes.DayStamp, "Day" },
            { ItemAttributes.MonthStamp, "Month" },
            { ItemAttributes.GroupName, "Groupname" },
            { ItemAttributes.Name, "Name" },
            { ItemAttributes.OwnerName, "Owner's name" },
            { ItemAttributes.RefCount, "Count of refs" },
            { ItemAttributes.Size, "Size" },
            { ItemAttributes.TimeOrYearStamp, "Time\\Year" },
            { ItemAttributes.TypeMod, "Mod" },
        };

        /// <summary>
        /// Extra function to display caption for attributes
        /// </summary>
        /// <param name="keys">List of attributes which caption to display</param>
        private void PrintCaptionTo(params ItemAttributes[] keys)
        {
            if (0 == keys.Length)
                return;
            for (int keyIndex = 0; keyIndex < keys.Length; keyIndex++)
                Console.Write("{0}\t", captionToItemAttributes[keys[keyIndex]]);
        }

        /// <summary>
        /// Gets list of all items in current directory with expanded attributes.
        /// Default attributes are day + month + year + size + name.
        /// User can choose what attributes to show by using keys specified in function body
        /// </summary>
        /// <param name="consoleArgs">List of attributes' keys to show </param>
        public override void SendRequest(params string[] consoleArgs)
        {
            request = CreateFtpRequest(WebRequestMethods.Ftp.ListDirectoryDetails, ftpUri);
            Console.Write("Connecting to {0}...\n\n", ftpUri);

            FtpWebResponse response = null;

            try
            {
                response = (FtpWebResponse)request.GetResponse();
            }
            catch (System.Net.WebException e)
            {
                // There's odd thing happens when exceptions raises and displays: next function reading from output stream
                // doesn't read... anything, just passes by. 
                Console.Write("Error: The remote name {0} couldn't be resolved\nShow full message? (y/n) ", ftpUri);

                if ('y' == (char)Console.Read())
                    // This oddity happens here
                    Console.Write("{0}\n", e.ToString());

                Console.WriteLine();
                // I decided to add this line to fix the problem above
                Console.ReadLine();

                return;
            }

            using (response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        // Dictionary for func's keys
                        Dictionary<string, ItemAttributes> arguments = new Dictionary<string, ItemAttributes>();
                        // Separates response to lines
                        string[] directoryItemsNotSeparated = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        // Then separate lines on items and place into a string array
                        string[,] directoryItems = new string[directoryItemsNotSeparated.Length, 9];

                        // Adds keys to dictionary
                        arguments.Add("-ds", ItemAttributes.DayStamp);
                        arguments.Add("-ms", ItemAttributes.MonthStamp);
                        arguments.Add("-gn", ItemAttributes.GroupName);
                        arguments.Add("-n", ItemAttributes.Name);
                        arguments.Add("-on", ItemAttributes.OwnerName);
                        arguments.Add("-rc", ItemAttributes.RefCount);
                        arguments.Add("-s", ItemAttributes.Size);
                        arguments.Add("-tys", ItemAttributes.TimeOrYearStamp);
                        arguments.Add("-tm", ItemAttributes.TypeMod);

                        // Finally separate items to attributes
                        for (int itemIndex = 0; itemIndex < directoryItems.GetLength(0); itemIndex++)
                        {
                            int attributeIndex = 0;
                            string[] directoryItem = directoryItemsNotSeparated[itemIndex].Split(new char[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string attribute in directoryItem)
                                directoryItems[itemIndex, attributeIndex++] = attribute;
                        }

                        // Display specified attributes
                        // If attributes are not specified show default scheme
                        if (0 == consoleArgs.Length || (1 == consoleArgs.Length && !arguments.ContainsKey(consoleArgs[0])))
                        {
                            Console.Write("Last modified\tSize\tName\n\n");
                            for (int itemIndex = 0; itemIndex < directoryItems.GetLength(0); itemIndex++)
                            {
                                Console.Write("{0} {1} {2}\t{3}\t{4}", directoryItems[itemIndex, (int)ItemAttributes.DayStamp],
                                                                       directoryItems[itemIndex, (int)ItemAttributes.MonthStamp],
                                                                       directoryItems[itemIndex, (int)ItemAttributes.TimeOrYearStamp],
                                                                       directoryItems[itemIndex, (int)ItemAttributes.Size],
                                                                       directoryItems[itemIndex, (int)ItemAttributes.Name]);
                                Console.WriteLine(directoryItems[itemIndex, (int)ItemAttributes.TypeMod][0] == '-' ? "" : "/");
                            }
                        }
                        else
                        {
                            // Displays caption
                            for (int argumentIndex = 0; argumentIndex < consoleArgs.Length; argumentIndex++)
                                if (arguments.ContainsKey(consoleArgs[argumentIndex]))
                                    PrintCaptionTo(arguments[consoleArgs[argumentIndex]]);

                            Console.WriteLine();

                            // Displays selected attributes
                            for (int itemIndex = 0; itemIndex < directoryItems.GetLength(0); itemIndex++)
                            {
                                for (int argumentIndex = 0; argumentIndex < consoleArgs.Length; argumentIndex++)
                                    if (arguments.ContainsKey(consoleArgs[argumentIndex]))
                                        Console.Write("{0}\t", directoryItems[itemIndex, (int)arguments[consoleArgs[argumentIndex]]]);
                                Console.WriteLine();
                            }
                        }
                        Console.WriteLine("\nDirectory List Complete, status {0}", response.StatusDescription);
                    }
                }
            }
        }
    }
}
