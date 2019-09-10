using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Nereid
{
	namespace FinalFrontier
	{
		internal static class Persistence
		{
			private const string FILE_NAME = "halloffame.ksp";

			private const string PERSISTENCE_NODE_ENTRY_NAME = "ENTRY";
			private static readonly string ROOT_PATH = Utils.GetRootPath();
			private static readonly string SAVE_BASE_FOLDER = ROOT_PATH + "/saves/"; // suggestion/hint from Cydonian Monk

			/***************************************************************************************************************
		    * new persistence model
		    ***************************************************************************************************************/

			public static void SaveHallOfFame(List<LogbookEntry> logbook, ConfigNode node)
			{
				Log.Info("saving hall of fame (" + logbook.Count + " logbook entries)");

				var logbookCopy = new List<LogbookEntry>(logbook);

				var sw = new Stopwatch();
				sw.Start();

				try {
					foreach (var entry in logbookCopy) {
						var entryNode = new ConfigNode(PERSISTENCE_NODE_ENTRY_NAME);
						if (Log.IsLogable(Log.LEVEL.DETAIL)) Log.Detail("saving logbook entry " + entry);
						entryNode.AddValue(Constants.CONFIGNODE_KEY_TIME, entry.UniversalTime.ToString());
						entryNode.AddValue(Constants.CONFIGNODE_KEY_NAME, entry.Name);
						entryNode.AddValue(Constants.CONFIGNODE_KEY_CODE, entry.Code);
						entryNode.AddValue(Constants.CONFIGNODE_KEY_DATA, entry.Data);
						node.AddNode(entryNode);
					}
				} catch {
					Log.Error("exception while saving hall of fame detected; hall of fame may be corrupt");
				} finally {
					sw.Stop();
					Log.Info("hall of fame saved in " + sw.ElapsedMilliseconds + "ms");
				}
			}


			public static List<LogbookEntry> LoadHallOfFame(ConfigNode node)
			{
				Log.Info("loading hall of fame");

				// create a temporary logbook
				// unnecessary in the current concept, but kept to keep changes at a minimum
				var logbook = new List<LogbookEntry>();

				if (node == null) {
					Log.Warning("no config node found. hall of fame will not load");
					return logbook;
				}

				var sw = new Stopwatch();
				sw.Start();

				try {
					foreach (var childNode in node.GetNodes()) {
						if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("child node found: " + childNode.name);
						var sTime = childNode.GetValue(Constants.CONFIGNODE_KEY_TIME);
						var code = childNode.GetValue(Constants.CONFIGNODE_KEY_CODE);
						var name = childNode.GetValue(Constants.CONFIGNODE_KEY_NAME);
						var data = childNode.GetValue(Constants.CONFIGNODE_KEY_DATA);
						try {
							var time = double.Parse(sTime);
							logbook.Add(new LogbookEntry(time, code, name, data));
						} catch {
							Log.Error("corrupt data in child node");
						}
					}

					return logbook;
				} catch {
					Log.Error("exception while loading hall of fame detected; hall of fame may be corrupt");
					return logbook;
				} finally {
					sw.Stop();
					Log.Info("hall of fame loaded in " + sw.ElapsedMilliseconds + "ms");
				}
			}

			public static string[] GetSaveGameFolders()
			{
				return Directory.GetDirectories(SAVE_BASE_FOLDER);
			}

			/***************************************************************************************************************
		    * debugging
		    ***************************************************************************************************************/

			/*
		    * This method is called for testign purposes only. It should never be called in a public release
		    */
			public static void WriteSupersedeChain(Pool<Ribbon> ribbons)
			{
				var file = File.CreateText(ROOT_PATH + "/GameData/Nereid/FinalFrontier/supersede.txt");
				var sorted = new List<Ribbon>(ribbons);
				sorted.Sort(delegate(Ribbon left, Ribbon right) {
					return left.GetCode().CompareTo(right.GetCode());
				});
				try {
					foreach (var ribbon in sorted) {
						var code = ribbon.GetCode().PadRight(20);
						var supersede = ribbon.SupersedeRibbon();
						file.WriteLine(code + (supersede != null ? supersede.GetCode() : ""));
					}
				} finally {
					file.Close();
				}
			}
		}
	}
}