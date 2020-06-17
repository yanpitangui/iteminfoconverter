using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItemInfoConverter
{
    public class Converter
    {
        private readonly List<ItemInfo> items;
        private readonly string folder;
        private const string IdentifiedDescriptionFilePath = "idnum2itemdesctable.txt";
        private const string IdentifiedResourceFilePath = "idnum2itemresnametable.txt";
        private const string IdentifiedDisplayFilePath = "idnum2itemdisplaynametable.txt";
        private const string UnidentifiedDescriptionFilePath = "num2itemdesctable.txt";
        private const string UnidentifiedResourceFilePath = "num2itemresnametable.txt";
        private const string UnidentifiedDisplayFilePath = "num2itemdisplaynametable.txt";
        private const string SlotCountFilePath = "itemslotcounttable.txt";
        private const string ItemDbConfPath = "item_db.conf";
        public Converter(string folder)
        {
            items = new List<ItemInfo>();
            this.folder = folder;
        }

        public async Task ReadItemDbConf()
        {
            var bytes = await ReadBytesAsync(ItemDbConfPath);
            var dbConf = GetEncodedString(bytes, Encoding.GetEncoding(1252)).ApplyDbPattern();
            items.AddRange(dbConf.Select(x => new ItemInfo { Id = x.GetItemId(), ClassNum = x.GetViewSprite() }));
        }

        public async Task ReadSlotCountFile()
        {
            var bytes = await ReadBytesAsync(SlotCountFilePath);
            foreach (var token in GetEncodedString(bytes, Encoding.GetEncoding(1252)).ApplyPattern())
            {
                var info = token.Split("#", StringSplitOptions.RemoveEmptyEntries);
                if (info.Length > 1)
                {
                    var item = items.FirstOrDefault(x => x.Id == info[0]);
                    if (item != null)
                    {
                        item.SlotCount = info[1];
                    }
                }

            }
        }

        public async Task ReadUnidentifiedDisplayFile()
        {
            var bytes = await ReadBytesAsync(UnidentifiedDisplayFilePath);
            foreach (var token in GetEncodedString(bytes, Encoding.GetEncoding(1252)).ApplyPattern())
            {
                var info = token.Split("#", StringSplitOptions.RemoveEmptyEntries);
                if (info.Length > 1)
                {
                    var item = items.FirstOrDefault(x => x.Id == info[0]);
                    if (item != null)
                    {
                        item.UnidentifiedDisplayName = info[1];
                    }
                }
            }
        }

        public async Task ReadUnidentifiedResourceFile()
        {
            var bytes = await ReadBytesAsync(UnidentifiedResourceFilePath);
            foreach (var token in GetEncodedString(bytes, Encoding.GetEncoding(949)).ApplyPattern())
            {
                var info = token.Split("#", StringSplitOptions.RemoveEmptyEntries);
                if (info.Length > 1)
                {
                    var item = items.FirstOrDefault(x => x.Id == info[0]);
                    if (item != null)
                    {
                        item.UnidentifiedResourceName = info[1];
                    }
                }

            }
        }

        public async Task ReadUnidentifiedDescriptionFile()
        {
            var bytes = await ReadBytesAsync(UnidentifiedDescriptionFilePath);
            foreach (var token in GetEncodedString(bytes, Encoding.GetEncoding(1252)).ApplyPattern())
            {
                var info = token.Split("#", StringSplitOptions.RemoveEmptyEntries);
                if (info.Length > 1)
                {
                    var item = items.FirstOrDefault(x => x.Id == info[0]);
                    if (item != null)
                    {
                        item.UnidentifiedDescriptionList = info[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
            }
        }

        public async Task ReadIdentifiedDisplayFile()
        {
            var bytes = await ReadBytesAsync(IdentifiedDisplayFilePath);
            foreach (var token in GetEncodedString(bytes, Encoding.GetEncoding(1252)).ApplyPattern())
            {
                var info = token.Split("#", StringSplitOptions.RemoveEmptyEntries);
                if (info.Length > 1)
                {
                    var item = items.FirstOrDefault(x => x.Id == info[0]);
                    if (item != null)
                    {
                        item.UnformattedIdentifiedDisplayName = info[1];
                    }
                }
            }
        }

        public async Task ReadIdentifiedResourceFile()
        {
            var bytes = await ReadBytesAsync(IdentifiedResourceFilePath);
            foreach (var token in GetEncodedString(bytes, Encoding.GetEncoding(949)).ApplyPattern())
            {
                var info = token.Split("#", StringSplitOptions.RemoveEmptyEntries);
                if (info.Length > 1)
                {
                    var item = items.FirstOrDefault(x => x.Id == info[0]);
                    if (item != null)
                    {
                        item.IdentifiedResourceName = info[1];
                    }
                }
            }
        }

        public async Task ReadIdentifiedDescriptionFile()
        {
            var bytes = await ReadBytesAsync(IdentifiedDescriptionFilePath);
            foreach (var token in GetEncodedString(bytes, Encoding.GetEncoding(1252)).ApplyPattern())
            {
                var info = token.Split("#", StringSplitOptions.RemoveEmptyEntries);
                if (info.Length > 1)
                {
                    var item = items.FirstOrDefault(x => x.Id == info[0]);
                    if (item != null)
                    {
                        item.IdentifiedDescriptionList = info[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                    }
                }

            }
        }

        public async Task GenerateItemInfo()
        {
            var path = $"{Path.Combine(folder, "itemInfo.lua")}";
            if (File.Exists(path))
                File.Delete(path);

            using (StreamWriter Writer = new StreamWriter(
                new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite),
                Encoding.GetEncoding(949)
                ))
            {
                await Writer.WriteAsync($"tbl = {{{Environment.NewLine}");
                int maxlen = items.Any() ? items.Max(x => x.Id.Length) : 0;
                foreach (var item in items.OrderBy(x => x.Id.PadLeft(maxlen, '0')))
                {
                    await Writer.WriteAsync($"{item}{Environment.NewLine}");
                }
                await Writer.WriteAsync($"}}{Environment.NewLine}{Environment.NewLine}");

                var resourceName = "ItemInfoConverter.Resources.luaFunction.lua";
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    await Writer.WriteAsync(await reader.ReadToEndAsync());
                    reader.Close();
                }
                Writer.Close();
            }
        }

        private async Task<byte[]> ReadBytesAsync(string filePath)
        {
            var filePathConcat = Path.Combine(folder, filePath);
            if (!string.IsNullOrEmpty(filePathConcat) && File.Exists(filePathConcat))
                return await File.ReadAllBytesAsync(filePathConcat);
            return new byte[] { };
        }

        private string GetEncodedString(byte[] text, Encoding encoding)
        {
            return encoding.GetString(text);
        }

    }

    public static class RegexHelper
    {
        private const string REGEX = @"(^(?<!\\\\)\d{1,}[#][\W\w]*?#)";
        private const string REGEXDBCONF = @"{\s*Id\s*:\s+\d*[\s\S]*?},";
        private const string REGEXVIEWSPRITE = @"ViewSprite\s*:\s*\d*";
        private const string REGEXSUBTYPE = @"Subtype\s*:\s*""\w*""";
        private const string REGEXITEMID = @"Id\s*:\s+\d*";
        private static readonly Dictionary<string, string> subTypes = new Dictionary<string, string> {
            {"W_FIST" , "0"},
            {"W_DAGGER" , "1"},
            {"W_1HSWORD" , "2"},
            {"W_2HSWORD" , "3"},
            {"W_1HSPEAR" , "4"},
            {"W_2HSPEAR" , "5"},
            {"W_1HAXE" , "6"},
            {"W_2HAXE" , "7"},
            {"W_MACE" , "8"},
            {"W_2HMACE" , "9"},
            {"W_STAFF" , "10"},
            {"W_BOW" , "11"},
            {"W_KNUCKLE" , "12"},
            {"W_MUSICAL" , "13"},
            {"W_WHIP" , "14"},
            {"W_BOOK" , "15"},
            {"W_KATAR" , "16"},
            {"W_REVOLVER" , "17"},
            {"W_RIFLE" , "18"},
            {"W_GATLING" , "19"},
            {"W_SHOTGUN" , "20"},
            {"W_GRENADE" , "21"},
            {"W_HUUMA" , "22"},
            {"W_2HSTAFF" , "23"}
        };
        public static string[] ApplyPattern(this string fileContent)
        {
            var regex = new Regex(REGEX, RegexOptions.Multiline);
            return regex.Matches(fileContent).Cast<Match>().Select(m => m.Value).ToArray();
        }

        public static IEnumerable<string> ApplyDbPattern(this string fileContent)
        {
            var regex = new Regex(REGEXDBCONF, RegexOptions.Multiline);
            return regex.Matches(fileContent).Select(x => x.Value);
        }

        public static string GetViewSprite(this string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent)) return "0";
            var regex = new Regex(REGEXVIEWSPRITE, RegexOptions.Multiline);
            var match = regex.Match(fileContent)?.Value;
            if (!string.IsNullOrEmpty(match))
            {
                string[] parts = Array.ConvertAll(match.Split(':'), p => p.Trim());
                if (parts?.Length > 1)
                {
                    return parts[1];
                }
            }
            else
            {
                regex = new Regex(REGEXSUBTYPE, RegexOptions.Multiline);
                match = regex.Match(fileContent)?.Value;
                if (!string.IsNullOrEmpty(match))
                {
                    string[] parts = Array.ConvertAll(match.Split(':'), p => p.Replace("\"", "").Trim());
                    if (parts?.Length > 1)
                    {
                        subTypes.TryGetValue(parts[1], out var output);
                        return output ?? "0";
                    }
                }
            }
            return "0";
        }

        public static string GetItemId(this string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent)) return "0";
            var regex = new Regex(REGEXITEMID, RegexOptions.Multiline);
            var match = regex.Match(fileContent)?.Value;
            if (!string.IsNullOrEmpty(match))
            {
                string[] parts = Array.ConvertAll(match.Split(':'), p => p.Trim());
                if (parts?.Length > 1)
                {
                    return parts[1];
                }
            }
            return "0";
        }
    }
}
