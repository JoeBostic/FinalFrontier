using System;
using System.Linq;

namespace Nereid
{
	namespace FinalFrontier
	{
		public abstract class DataChange : Activity
		{
			private static ActivityPool ACTIVITIES = ActivityPool.Instance();

			public static DataChange DATACHANGE_CUSTOMRIBBON = new CustomRibbonDataChange();

			public DataChange(string code, string name)
				: base(code, name)
			{
			}

			public abstract void DoDataChange(object[] parameter);

			// output line in logbook
			public override string CreateLogBookEntry(LogbookEntry entry)
			{
				Log.Error("could not add datachanges to personal logbooks");
				throw new InvalidOperationException();
			}
		}

		public class CustomRibbonDataChange : DataChange
		{
			public CustomRibbonDataChange()
				: base("X+", "Custom Ribbon")
			{
			}


			public override void DoDataChange(object[] parameter)
			{
				if (parameter.Count() < 3) {
					Log.Warning("invalid parameter count for custom ribbon change");
					return;
				}

				var s0 = parameter[0] as string;
				var s1 = parameter[1] as string;
				var s2 = parameter[2] as string;

				if (s0 == null || s1 == null || s2 == null) {
					Log.Warning("invalid parameter type for custom ribbon change");
					return;
				}

				var code = "X" + s0.Substring(2);
				var name = s1;
				var text = s2;

				if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("changing custom ribbon for code " + code);

				var ribbon = RibbonPool.Instance().GetRibbonForCode(code);
				if (ribbon == null) {
					Log.Error("invalid custom ribbon code: " + code);
					return;
				}

				//
				var achievement = ribbon.GetAchievement() as CustomAchievement;
				if (achievement == null) {
					Log.Error("invalid custom ribbon achievement");
					return;
				}

				achievement.SetName(name);
				achievement.SetDescription(text);
				if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("custom ribbon changed");
			}
		}
	}
}