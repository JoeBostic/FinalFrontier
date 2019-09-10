using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Nereid
{
	namespace FinalFrontier
	{
		internal static class Utils
		{
			private static readonly char[] SINGLE_SPACE_ARRAY = {' '};

			public static bool CheckInstallationPath()
			{
				var ROOT_PATH = GetRootPath();

				if (!File.Exists(ROOT_PATH + "/GameData/Nereid/FinalFrontier/FinalFrontier.version")) return false;

				return true;
			}

			public static string ToString<T>(List<T> list)
			{
				var result = "";
				foreach (var x in list) {
					if (result.Length > 0) result = result + ",";
					result = result + x;
				}

				return result + " (" + list.Count + " entries)";
			}

			public static string Roman(int value)
			{
				switch (value) {
					case 0: return "";
					case 1: return "I";
					case 2: return "II";
					case 3: return "III";
					case 4: return "IV";
					case 5: return "V";
					case 6: return "VI";
					case 7: return "VII";
					case 8: return "VIII";
					case 9: return "IX";
					case 10: return "X";
					case 11: return "XI";
					case 12: return "XII";
					case 13: return "XIII";
					case 14: return "XIV";
					case 15: return "XV";
					case 16: return "XVI";
					case 17: return "XVII";
					case 18: return "XVIII";
					case 19: return "XIX";
					case 20: return "XX";
				}

				return "?";
			}

			public static CelestialBody GetCelestialBody(string name)
			{
				foreach (var body in PSystemManager.Instance.localBodies)
					if (body.GetName().Equals(name))
						return body;
				return null;
			}


			public static int ConvertDaysToSeconds(int days)
			{
				return days * 60 * 60 * 24;
			}

			public static int ConvertHoursToSeconds(int hours)
			{
				return hours * 60 * 60;
			}

			public static string GetRootPath()
			{
				var path = KSPUtil.ApplicationRootPath;
				path = path.Replace("\\", "/");
				if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
				//
				return path;
			}

			public static string ConvertToKerbinDuration(double ut)
			{
				var hours = ut / 60.0 / 60.0;
				var kHours = Math.Floor(hours % 24.0);
				var kMinutes = Math.Floor(ut / 60.0 % 60.0);
				var kSeconds = Math.Floor(ut % 60.0);


				var kYears = Math.Floor(hours / 2556.5402); // Kerbin year is 2556.5402 hours
				var kDays = Math.Floor(hours % 2556.5402 / 24.0);
				return (kYears > 0 ? kYears + " Years " : "")
				       + (kDays > 0 ? kDays + " Days " : "")
				       + (kHours > 0 ? kHours + " Hours " : "")
				       + (kMinutes > 0 ? kMinutes + " Minutes " : "")
				       + (kSeconds > 0 ? kSeconds + " Seconds " : "");
			}

			public static string TimeAsString(double ut)
			{
				if (GameSettings.KERBIN_TIME) return ConvertToKerbinTime(ut);
				return ConvertToEarthTime(ut);
			}

			public static string ConvertToKerbinTime(double ut)
			{
				var hours = ut / 60.0 / 60.0;
				var kHours = Math.Floor(hours % 6.0);
				var kMinutes = Math.Floor(ut / 60.0 % 60.0);
				var kSeconds = Math.Floor(ut % 60.0);


				var kYears = Math.Floor(hours / 2556.5402) + 1; // Kerbin year is 2556.5402 hours
				var kDays = Math.Floor(hours % 2556.5402 / 6.0) + 1;

				return "Year " + kYears + ", Day " + kDays + " " + " " + kHours.ToString("00") + ":" + kMinutes.ToString("00") + ":" + kSeconds.ToString("00");
			}

			public static string ConvertToEarthTime(double ut)
			{
				var hours = ut / 60.0 / 60.0;
				var eHours = Math.Floor(hours % 24.0);
				var eMinutes = Math.Floor(ut / 60.0 % 60.0);
				var eSeconds = Math.Floor(ut % 60.0);


				var eYears = Math.Floor(hours / (365 * 24)) + 1;
				var eDays = Math.Floor(hours % (365 * 24) / 24.0) + 1;

				return "Year " + eYears + ", Day " + eDays + " " + " " + eHours.ToString("00") + ":" + eMinutes.ToString("00") + ":" + eSeconds.ToString("00");
			}

			public static double GameTimeInDays(double time)
			{
				if (GameUtils.IsKerbinTimeEnabled())
					return time / 6 / 60 / 60;
				return time / 24 / 60 / 60;
			}


			/**
		    * Remove multiple spaces
		    */
			public static string Compress(this string s)
			{
				if (s == null) return "";
				return string.Join(" ", s.Trim().Split(SINGLE_SPACE_ARRAY, StringSplitOptions.RemoveEmptyEntries));
			}

			public static string GameTimeInDaysAsString(double time)
			{
				var inDays = GameTimeInDays(time);
				if (inDays >= 1000)
					return (inDays / 1000).ToString("0") + "k";
				if (inDays >= 1000000) return (inDays / 1000000).ToString("0") + "m";
				return inDays.ToString("0.00");
			}

			public static KeyCode GetKeyCode(char c)
			{
				var codes = (KeyCode[]) Enum.GetValues(typeof(KeyCode));
				foreach (var code in codes) {
					var name = code.ToString();
					if (name.Length > 0 && name == c.ToString()) return code;
				}

				throw new ArgumentException("no keycode for '" + c + "'");
			}

			public static KeyCode[] GetPressedKeys()
			{
				var pressed = new List<KeyCode>();
				var keycodes = (KeyCode[]) Enum.GetValues(typeof(KeyCode));
				foreach (var key in keycodes)
					if (Input.GetKeyDown(key))
						pressed.Add(key);
				var result = new KeyCode[pressed.Count];
				pressed.CopyTo(result);
				return result;
			}

			public static string GameTimeAsString(double time)
			{
				var seconds = (long) time;
				var days = (long) GameTimeInDays(time);
				long hours_per_day = GameUtils.IsKerbinTimeEnabled() ? Constants.HOURS_PER_KERBIN_DAY : Constants.HOURS_PER_EARTH_DAY;
				var seconds_per_day = Constants.SECONDS_PER_HOUR * hours_per_day;
				var hours = seconds % seconds_per_day / Constants.SECONDS_PER_HOUR;
				var minutes = seconds % Constants.SECONDS_PER_HOUR / Constants.SECONDS_PER_MINUTE;

				var hhmm = hours.ToString("00") + ":" + minutes.ToString("00");

				if (days == 0)
					return hhmm;
				if (days < 1000)
					return days.ToString("0") + "d " + hhmm;
				if (days >= 1000)
					return (days / 1000).ToString("0") + "kd " + hhmm;
				return (days / 1000000).ToString("0") + "md " + hhmm;
			}
		}
	}
}