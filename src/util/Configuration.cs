using System;
using System.Collections.Generic;
using System.IO;
using FinalFrontierAdapter;
using UnityEngine;

namespace Nereid
{
	namespace FinalFrontier
	{
		public class Configuration
		{
			private static readonly string ROOT_PATH = Utils.GetRootPath();
			private static readonly string CONFIG_BASE_FOLDER = ROOT_PATH + "/GameData/";
			private static readonly string FILE_NAME = "FinalFrontier.dat";
			private static readonly short FILE_MARKER = 0x7A7A;
			private static readonly short FILE_VERSION = 1;

			private readonly Pair<int, int> ORIGIN = new Pair<int, int>(0, 0);
			private string decorationBoardWindowTitle = "Kerbal Decoration Board";

			private readonly Dictionary<GameScenes, HallOfFameBrowser.HallOfFameFilter> hallOfFameFilter = new Dictionary<GameScenes, HallOfFameBrowser.HallOfFameFilter>();
			private readonly Dictionary<GameScenes, HallOfFameBrowser.HallOfFameSorter> hallOfFameSorter = new Dictionary<GameScenes, HallOfFameBrowser.HallOfFameSorter>();

			// configurable window titles
			private string hallOfFameWindowTitle = "Final Frontier Hall of Fame";
			private string missionSummaryWindowTitle = "Final Frontier Mission Summary";


			// FAR used?
			public bool UseFARCalculations;
			private readonly Dictionary<int, Pair<int, int>> windowPositions = new Dictionary<int, Pair<int, int>>();

			public Configuration()
			{
				// default window positions
				ResetWindowPositions();

				// Deafults
				customRibbonAtSpaceCenterEnabled = false;
				logLevel = Log.LEVEL.INFO;
				autoExpandEnabled = true;
				hotkeyEnabled = true;
				removalOfRibbonsEnabled = false;
				missonSummaryPopup = true;
				useStockToolbar = !ToolbarManager.ToolbarAvailable;
				convertGames = true;
				logRibbonAwards = false;
				hotkey = Utils.GetKeyCode('F');

				// 
				// Default filter/sorts
				foreach (GameScenes scene in Enum.GetValues(typeof(GameScenes))) {
					hallOfFameFilter[scene] = new HallOfFameBrowser.HallOfFameFilter(scene);
					hallOfFameSorter[scene] = new HallOfFameBrowser.HallOfFameSorter(scene);
				}
			}

			public bool customRibbonAtSpaceCenterEnabled { get; set; }
			public Log.LEVEL logLevel { get; set; }
			public bool autoExpandEnabled { get; set; }
			public bool hotkeyEnabled { get; set; }
			public bool removalOfRibbonsEnabled { get; set; }
			public bool missonSummaryPopup { get; set; }
			public bool useStockToolbar { get; set; }
			public bool convertGames { get; set; }
			public bool logRibbonAwards { get; set; }
			public KeyCode hotkey { get; set; } // for use with LEFT-ALT

			public string GetHallOfFameWindowTitle()
			{
				return hallOfFameWindowTitle;
			}

			public void SetHallOfFameWindowTitle(string title)
			{
				hallOfFameWindowTitle = title;
			}

			public string GetDecorationBoardWindowTitle()
			{
				return decorationBoardWindowTitle;
			}


			public void SetDecorationBoardWindowTitle(string title)
			{
				decorationBoardWindowTitle = title;
			}


			public string GetMissionSummaryWindowTitle()
			{
				return missionSummaryWindowTitle;
			}

			public void SetMissionSummaryWindowTitle(string title)
			{
				missionSummaryWindowTitle = title;
			}

			public HallOfFameBrowser.HallOfFameFilter GetDisplayFilterForSzene(GameScenes scene)
			{
				var filter = hallOfFameFilter[scene];
				if (filter != null) return filter;
				hallOfFameFilter[scene] = new HallOfFameBrowser.HallOfFameFilter(scene);
				return hallOfFameFilter[scene];
			}

			public HallOfFameBrowser.HallOfFameSorter GetHallOfFameSorterForScene(GameScenes scene)
			{
				var sorter = hallOfFameSorter[scene];
				if (sorter != null) return sorter;
				hallOfFameSorter[scene] = new HallOfFameBrowser.HallOfFameSorter(scene);
				return hallOfFameSorter[scene];
			}

			public void ResetWindowPositions()
			{
				SetWindowPosition(Constants.WINDOW_ID_HALLOFFAMEBROWSER, 150, 50);
				SetWindowPosition(Constants.WINDOW_ID_DISPLAY, 810, 50);
				SetWindowPosition(Constants.WINDOW_ID_ABOUT, 50, 50);
				SetWindowPosition(Constants.WINDOW_ID_CONFIG, 150, 50);
				SetWindowPosition(Constants.WINDOW_ID_MISSION_SUMMARY, AbstractWindow.CENTER_HORIZONTALLY, AbstractWindow.CENTER_VERTICALLY);
			}

			public Pair<int, int> GetWindowPosition(AbstractWindow window)
			{
				return GetWindowPosition(window.GetWindowId());
			}

			public Pair<int, int> GetWindowPosition(int windowId)
			{
				try {
					return windowPositions[windowId];
				} catch (KeyNotFoundException) {
					Log.Warning("no initial position found for window " + windowId + " in configuration");
					return ORIGIN;
				}
			}

			public void SetWindowPosition(AbstractWindow window)
			{
				SetWindowPosition(window.GetWindowId(), window.GetX(), window.GetY());
			}

			public void SetWindowPosition(int windowId, Pair<int, int> position)
			{
				Log.Trace("set window position for window id " + windowId + ": " + position);
				if (windowPositions.ContainsKey(windowId))
					windowPositions[windowId] = position;
				else
					windowPositions.Add(windowId, position);
			}

			public void SetWindowPosition(int windowId, int x, int y)
			{
				SetWindowPosition(windowId, new Pair<int, int>(x, y));
			}

			public bool UseStockToolbar()
			{
				// use of stock toolbar or blizzys toolbar not available?
				return useStockToolbar || !ToolbarManager.ToolbarAvailable;
			}

			public void SetUseStockToolbar(bool enabled)
			{
				useStockToolbar = enabled;
			}

			public bool IsCustomRibbonAtSpaceCenterEnabled()
			{
				return customRibbonAtSpaceCenterEnabled;
			}

			public void SetCustomRibbonAtSpaceCenterEnabled(bool enabled)
			{
				customRibbonAtSpaceCenterEnabled = enabled;
			}

			public bool IsAutoExpandEnabled()
			{
				return autoExpandEnabled;
			}

			public void SetAutoExpandEnabled(bool enabled)
			{
				autoExpandEnabled = enabled;
			}

			public bool IsHotkeyEnabled()
			{
				return hotkeyEnabled;
			}

			public void SetHotkeyEnabled(bool enabled)
			{
				hotkeyEnabled = enabled;
			}

			public bool IsMissionSummaryEnabled()
			{
				return missonSummaryPopup;
			}

			public void SetMissionSummaryEnabled(bool enabled)
			{
				missonSummaryPopup = enabled;
			}

			public bool IsRevocationOfRibbonsEnabled()
			{
				return removalOfRibbonsEnabled;
			}

			public void SetRevocationOfRibbonsEnabled(bool enabled)
			{
				removalOfRibbonsEnabled = enabled;
			}

			public Log.LEVEL GetLogLevel()
			{
				return logLevel;
			}

			public void SetLogLevel(Log.LEVEL level)
			{
				logLevel = level;
			}

			private void WriteWindowPositions(BinaryWriter writer)
			{
				Log.Info("storing window positions");
				writer.Write((short) windowPositions.Count);
				Log.Info("writing " + windowPositions.Count + " window positions");
				foreach (var id in windowPositions.Keys) {
					var position = windowPositions[id];
					Log.Trace("window position for window id " + id + " written: " + position.first + "/" + position.second);
					writer.Write(id);
					writer.Write((short) position.first);
					writer.Write((short) position.second);
				}
			}

			private void ReadWindowPositions(BinaryReader reader)
			{
				Log.Detail("loading window positions");
				int count = reader.ReadInt16();
				Log.Detail("loading " + count + " window positions");
				for (var i = 0; i < count; i++) {
					var id = reader.ReadInt32();
					int x = reader.ReadInt16();
					int y = reader.ReadInt16();
					Log.Trace("read window position for window id " + id + ": " + x + "/" + y);
					SetWindowPosition(id, x, y);
				}
			}

			private void WriteHallOfFameFilter(BinaryWriter writer)
			{
				var cnt = (short) hallOfFameFilter.Count;
				Log.Detail("writing " + cnt + " hall of fame filter to config");
				writer.Write(cnt);
				foreach (var entry in hallOfFameFilter) writer.Write(entry.Value);
			}

			private void WriteHallOfFameSorter(BinaryWriter writer)
			{
				var cnt = (short) hallOfFameSorter.Count;
				Log.Detail("writing " + cnt + " hall of fame sorter to config");
				writer.Write(cnt);
				foreach (var entry in hallOfFameSorter) writer.Write(entry.Value);
			}


			private void ReadHallOfFameFilter(BinaryReader reader)
			{
				Log.Detail("reading hall of fame filter from config");
				var cnt = reader.ReadInt16();
				for (var i = 0; i < cnt; i++) {
					var filter = reader.ReadFilter();
					if (filter != null) {
						hallOfFameFilter[filter.GetScene()] = filter;
						Log.Detail("hall of fame display filter loaded: " + filter);
					}
				}
			}

			private void ReadHallOfFameSorter(BinaryReader reader)
			{
				Log.Detail("reading hall of fame sorter from config");
				var cnt = reader.ReadInt16();
				for (var i = 0; i < cnt; i++) {
					var sorter = reader.ReadSorter();
					if (sorter != null) {
						hallOfFameSorter[sorter.GetScene()] = sorter;
						Log.Detail("hall of fame display sorter loaded: " + sorter);
					}
				}
			}

			public void Save()
			{
				var filename = CONFIG_BASE_FOLDER + FILE_NAME;
				Log.Info("storing configuration in " + filename);
				try {
					using (var writer = new BinaryWriter(File.Open(filename, FileMode.Create))) {
						writer.Write((short) logLevel);
						writer.Write(customRibbonAtSpaceCenterEnabled);
						// File Version
						writer.Write(FILE_MARKER); // marker
						writer.Write(FILE_VERSION);
						//
						WriteWindowPositions(writer);
						//
						writer.Write(autoExpandEnabled);
						//
						writer.Write(hotkeyEnabled);
						//
						writer.Write(removalOfRibbonsEnabled);
						//
						// filter
						WriteHallOfFameFilter(writer);
						//
						// sorter
						WriteHallOfFameSorter(writer);
						//
						// window titles
						writer.Write(hallOfFameWindowTitle);
						writer.Write(decorationBoardWindowTitle);
						writer.Write(missionSummaryWindowTitle);
						writer.Write("reserved");
						writer.Write("reserved");
						writer.Write("reserved");
						//
						// use FAR
						writer.Write(UseFARCalculations);
						//
						// Popup window when recovering vessel
						writer.Write(missonSummaryPopup);
						//
						// reserved
						writer.Write((ushort) 0);
						// use stock toolbar
						writer.Write(useStockToolbar);
						//
						// reserved
						writer.Write((short) 0);
						//
						// convert games
						writer.Write(convertGames);
						//
						// log ribbon awards
						writer.Write(logRibbonAwards);
						//
						// hotkey
						writer.Write((ushort) hotkey);
					}
				} catch {
					Log.Error("saving configuration failed");
				}
			}

			public void Load()
			{
				var filename = CONFIG_BASE_FOLDER + FILE_NAME;
				try {
					if (File.Exists(filename)) {
						Log.Info("loading configuration from " + filename);
						using (var reader = new BinaryReader(File.OpenRead(filename))) {
							logLevel = (Log.LEVEL) reader.ReadInt16();
							Log.Info("log level loaded: " + logLevel);
							customRibbonAtSpaceCenterEnabled = reader.ReadBoolean();
							// File Version
							var marker = reader.ReadInt16();
							if (marker != FILE_MARKER) {
								Log.Error("invalid file structure");
								throw new IOException("invalid file structure");
							}

							var version = reader.ReadInt16();
							if (version != FILE_VERSION) {
								Log.Error("incompatible file version");
								throw new IOException("incompatible file version");
							}

							//
							ReadWindowPositions(reader);
							//
							autoExpandEnabled = reader.ReadBoolean();
							//
							hotkeyEnabled = reader.ReadBoolean();
							//
							removalOfRibbonsEnabled = reader.ReadBoolean();
							//
							ReadHallOfFameFilter(reader);
							//
							ReadHallOfFameSorter(reader);
							//
							hallOfFameWindowTitle = reader.ReadString();
							decorationBoardWindowTitle = reader.ReadString();
							missionSummaryWindowTitle = reader.ReadString();
							reader.ReadString(); // reserved
							reader.ReadString(); // reserved
							reader.ReadString(); // reserved
							//
							// use FAR
							UseFARCalculations = reader.ReadBoolean();
							//
							// Popup window when recovering vessel
							missonSummaryPopup = reader.ReadBoolean();
							//
							// reserved
							reader.ReadUInt16();
							// use stock toolbar
							useStockToolbar = reader.ReadBoolean();
							//
							// reserved
							reader.ReadInt16();
							//
							// convert games
							convertGames = reader.ReadBoolean();
							//
							// log ribbon awards
							logRibbonAwards = reader.ReadBoolean();
							//
							// hotkey
							hotkey = (KeyCode) reader.ReadInt16();
						}
					} else {
						Log.Info("no config file: default configuration");
					}
				} catch {
					Log.Warning("loading configuration failed or incompatible file");
				}
			}
		}
	}
}