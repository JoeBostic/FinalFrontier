﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Nereid
{
	namespace FinalFrontier
	{
		/**
		 * This class scans all subfolders of GameData for files with the name "CelestialBodies.info" and reads them to provide external ribbon information.
		 */
		public class CelestialBodyInfo
		{
			private static readonly string INFO_FILE = "CelestialBodies.info";

			private readonly Dictionary<CelestialBody, Info> infos = new Dictionary<CelestialBody, Info>();

			private readonly string plugin;

			public CelestialBodyInfo()
				: this("FinalFrontier")
			{
				// default constructor
			}

			public CelestialBodyInfo(string plugin)
			{
				this.plugin = plugin;
			}

			private void ReadCelestialBodyData(StreamReader file, string name)
			{
				Log.Info("loading celestial body info for " + name);
				var info = new Info();
				info.ReadSection(file);

				var body = GameUtils.GetBodyForName(name);
				if (body == null) {
					Log.Warning("celestial body '" + name + "' not found. celestial body info ignored");
					return;
				}

				if (!infos.ContainsKey(body)) {
					infos.Add(body, info);
					Log.Detail("celestial body stored");
				} else {
					Log.Warning("celestial body info for '" + name + "' already present");
					info = infos[body];
				}
			}

			private void ReadInfoFile(string filename)
			{
				StreamReader file = null;
				try {
					file = File.OpenText(filename);
					string line;
					while ((line = file.ReadLine()) != null) {
						line = line.Trim();
						if (!line.StartsWith("#") && line.Length > 0) ReadCelestialBodyData(file, line);
					}
				} catch (Exception e) {
					Log.Error(e.Message);
				}
			}

			public void ScanGameData()
			{
				Log.Info("scanning " + Constants.GAMEDATA_PATH + " for celestial body info in " + plugin);
				ScanGameData(Constants.GAMEDATA_PATH);
				Log.Detail("scan for celestial body info completed");
			}

			public void ScanGameData(string basefolder)
			{
				if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("scanning folder " + basefolder + " for celestial body info");
				try {
					foreach (var folder in Directory.GetDirectories(basefolder)) {
						var filename = folder + "/" + INFO_FILE;
						if (File.Exists(filename)) {
							Log.Info("celestial body infos found in " + folder);
							ReadInfoFile(filename);
						}

						ScanGameData(folder);
					}
				} catch (Exception) {
					Log.Error("failed to scan for celestial body info");
				}
			}

			public bool Contains(CelestialBody body)
			{
				return infos.ContainsKey(body);
			}

			public string GetString(CelestialBody body, string plugin, string key, string defaultValue = "")
			{
				if (infos.ContainsKey(body)) {
					var info = infos[body];
					return info.GetString(plugin, key, defaultValue);
				}

				return defaultValue;
			}

			public int GetInt(CelestialBody body, string plugin, string key, int defaultValue = 0)
			{
				if (infos.ContainsKey(body)) {
					var info = infos[body];
					return info.GetInt(plugin, key, defaultValue);
				}

				return defaultValue;
			}

			public bool GetBool(CelestialBody body, string plugin, string key, bool defaultValue = false)
			{
				if (infos.ContainsKey(body)) {
					var info = infos[body];
					return info.GetBool(plugin, key, defaultValue);
				}

				return defaultValue;
			}

			private class CelestialBodyInfoSyntaxException : Exception
			{
				public CelestialBodyInfoSyntaxException(string msg)
					: base(msg)
				{
				}
			}

			private class Info : Dictionary<string, string>
			{
				private string AsPrefix(string s)
				{
					if (s == null || s.Length == 0) return "";
					return s + ".";
				}

				public string GetString(string plugin, string key, string defaultValue = "")
				{
					key = AsPrefix(plugin) + key;
					if (ContainsKey(key)) return this[key];
					return defaultValue;
				}

				public int GetInt(string plugin, string key, int defaultValue = 0)
				{
					key = AsPrefix(plugin) + key;
					if (ContainsKey(key))
						try {
							return short.Parse(this[key]);
						} catch (FormatException) {
							Log.Warning("invalid value for key " + key + ": integer required");
							return 0;
						}

					return defaultValue;
				}

				public bool GetBool(string plugin, string key, bool defaultValue = false)
				{
					key = AsPrefix(plugin) + key;
					if (ContainsKey(key))
						try {
							return bool.Parse(this[key]);
						} catch (FormatException) {
							Log.Warning("invalid value for key " + key + ": bool required");
							return false;
						}

					return defaultValue;
				}

				private void ReadAttribute(string plugin, string line)
				{
					var p = line.IndexOf('=');
					if (p > 0) {
						var key = AsPrefix(plugin) + line.Substring(0, p - 1).Compress();
						var value = line.Substring(p + 1).Trim();
						Log.Info("celestial body attribute " + key + " is " + value);
						if (ContainsKey(key)) {
							// TODO: Exception
							Log.Warning("key " + key + " found twice in " + plugin);
							Remove(key);
						}

						Add(key, value);
					} else {
						throw new CelestialBodyInfoSyntaxException("syntax error while reading attribute");
					}
				}

				public void ReadSection(StreamReader file, string plugin = "")
				{
					Log.Info("reading section for plugin '" + plugin + "'");
					var openingBraceFound = false;
					string line;
					while ((line = file.ReadLine()) != null) {
						line = line.Trim();
						if (!line.StartsWith("#")) {
							if (openingBraceFound) {
								if (line.StartsWith("}")) return;
								if (line.Contains("="))
									ReadAttribute(plugin, line);
								else
									ReadSection(file, line);
							}

							if (line.StartsWith("{")) openingBraceFound = true;
						}
					}

					throw new CelestialBodyInfoSyntaxException("missing closing brace");
				}
			}
		}
	}
}