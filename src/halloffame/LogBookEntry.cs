using System.Linq;

namespace Nereid
{
	namespace FinalFrontier
	{
		public class LogbookEntry
		{
			// serializion
			public static readonly char TEXT_DELIM = '~';
			private static readonly char[] TEXT_SEPARATORS = {TEXT_DELIM};
			private static readonly char[] FIELD_SEPARATORS = {' '};

			public LogbookEntry(double time, string code, string name, string text = "")
			{
				UniversalTime = time;
				Code = code;
				Name = name != null ? name : "";
				Data = text;
				//
				if (Name.Contains(TEXT_DELIM)) {
					Log.Error("name field contains invalid character '" + TEXT_DELIM + "': " + Name);
					Name.Replace(TEXT_DELIM, '_');
				}
			}

			public double UniversalTime { get; set; }
			public string Code { get; set; }
			public string Name { get; set; }
			public string Data { get; set; }

			public override string ToString()
			{
				var timestamp = Utils.TimeAsString(UniversalTime) + ": ";
				var action = ActionPool.Instance().GetActionForCode(Code);
				if (action != null) return timestamp + action.CreateLogBookEntry(this);

				var ribbon = RibbonPool.Instance().GetRibbonForCode(Code);
				if (ribbon != null) {
					var achievement = ribbon.GetAchievement();
					return timestamp + achievement.CreateLogBookEntry(this);
				}

				return "unknown logbook entry (code " + Code + ")";
			}

			public string AsString()
			{
				return Utils.ConvertToKerbinTime(UniversalTime) + ": " + Name + " " + Code;
			}

			public string Serialize()
			{
				var line = UniversalTime + " " + Code + " " + Name;
				if (Data != null && Data.Length > 0) line = line + TEXT_DELIM + Data;
				return line;
			}

			public static LogbookEntry Deserialize(string line)
			{
				var field = line.Split(FIELD_SEPARATORS, 3);
				if (field.Length == 3) {
					var time = double.Parse(field[0]);
					var code = field[1];
					var name = field[2];
					var text = "";
					if (name.Contains(TEXT_DELIM)) {
						var subfields = field[2].Split(TEXT_SEPARATORS, 2);
						name = subfields[0];
						text = subfields.Length == 2 ? subfields[1] : "";
					}

					return new LogbookEntry(time, code, name, text);
				}

				Log.Warning("invalid logbook entry: " + line);
				return null;
			}
		}
	}
}