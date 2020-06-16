using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemInfoConverter
{
    public class ItemInfo
    {
        public string Id { get; set; }

        public string UnidentifiedDisplayName { get; set; }

        public string UnidentifiedResourceName { get; set; }

        public IList<string> UnidentifiedDescriptionList { get; set; }

        public string UnidentifiedDescriptionName
        {
            get
            {
                if (UnidentifiedDescriptionList == null || UnidentifiedDescriptionList?.Count == 0) return "\"\"";
                var tabs = UnidentifiedDescriptionList?.Count == 1 ? string.Empty : "\t\t\t";
                var newLine = UnidentifiedDescriptionList?.Count > 1 ? Environment.NewLine : string.Empty;
                return $"{newLine}{string.Join($",{Environment.NewLine}", UnidentifiedDescriptionList.Select(x => $"{tabs}\"{x.Replace("\"", "'")}\""))}";
            }
        }

        public string UnformattedIdentifiedDisplayName { get; set; }

        public string IdentifiedDisplayName
        {
            get
            {
                return UnformattedIdentifiedDisplayName?.Replace("_", " ");
            }
        }

        public string IdentifiedResourceName { get; set; }

        public IList<string> IdentifiedDescriptionList { get; set; }

        public string IdentifiedDescriptionName
        {
            get
            {
                if (IdentifiedDescriptionList == null || IdentifiedDescriptionList?.Count == 0) return "\"\"";
                var tabs = IdentifiedDescriptionList?.Count > 1 ? "\t\t\t" : string.Empty;
                var newLine = IdentifiedDescriptionList?.Count > 1 ? Environment.NewLine : string.Empty;
                return $"{newLine}{string.Join($",{Environment.NewLine}", IdentifiedDescriptionList.Select(x => $"{tabs}\"{x.Replace("\"", "'").Replace("\n", "")}\""))}";
            }
        }

        public string SlotCount { get; set; } = "0";

        public string ClassNum { get; set; }

        public override string ToString()
        {
            var output = $"\t[{Id}] = {{{Environment.NewLine}";
            output += $"\t\tunidentifiedDisplayName = \"{UnidentifiedDisplayName}\",{Environment.NewLine}";
            output += $"\t\tunidentifiedResourceName = \"{UnidentifiedResourceName}\",{Environment.NewLine}";
            output += $"\t\tunidentifiedDescriptionName = {{ {UnidentifiedDescriptionName}{Environment.NewLine}\t\t}},{Environment.NewLine}";
            output += $"\t\tidentifiedDisplayName = \"{IdentifiedDisplayName}\",{Environment.NewLine}";
            output += $"\t\tidentifiedResourceName = \"{IdentifiedResourceName }\",{Environment.NewLine}";
            output += $"\t\tidentifiedDescriptionName = {{ {IdentifiedDescriptionName}{Environment.NewLine}\t\t}},{Environment.NewLine}";
            output += $"\t\tslotCount = {SlotCount},{Environment.NewLine}";
            output += $"\t\tClassNum = {ClassNum}{Environment.NewLine}\t}},";
            return output;
        }

    }
}
