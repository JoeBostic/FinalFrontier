using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Nereid
{
	namespace FinalFrontier
	{
		internal class RibbonPack : IEnumerable<Ribbon>
		{
			private readonly string baseFolder;
			private int baseId;
			private string ribbonFolder;

			private readonly List<Ribbon> ribbons = new List<Ribbon>();

			public RibbonPack(string config)
			{
				name = "unnamed";
				baseFolder = Path.GetDirectoryName(config).Substring(Constants.GAMEDATA_PATH.Length + 1).Replace("\\", "/");
				ribbonFolder = baseFolder;
				Load(config);
			}

			public string name { get; private set; }

			public IEnumerator GetEnumerator()
			{
				return ribbons.GetEnumerator();
			}

			IEnumerator<Ribbon> IEnumerable<Ribbon>.GetEnumerator()
			{
				return ribbons.GetEnumerator();
			}

			private void ReadLine(string line)
			{
				// ignore comments
				if (!line.StartsWith("#") && line.Length > 0) {
					var fields = line.Split(':');
					if (fields.Length > 0) {
						var what = fields[0];
						if (what.Equals("NAME") && fields.Length == 2) {
							name = fields[1];
						} else if (what.Equals("FOLDER") && fields.Length == 2) {
							ribbonFolder = baseFolder + "/" + fields[1];
							Log.Detail("changing ribbon folder of ribbon pack '" + name + "' to '" + ribbonFolder + "'");
						} else if (what.Equals("BASE") && fields.Length == 2) {
							try {
								baseId = int.Parse(fields[1]);
								Log.Detail("changing base id of ribbon pack '" + name + "' to " + baseId);
							} catch {
								Log.Error("failed to parse custom ribbon base id");
							}
						} else if (fields.Length == 4 || fields.Length == 5) {
							int id;
							try {
								id = baseId + int.Parse(fields[0]);
							} catch {
								Log.Error("failed to parse custom ribbon id");
								return;
							}

							var fileOfRibbon = ribbonFolder + "/" + fields[1];
							var nameOfRibbon = fields[2];
							var descOfRibbon = fields[3];
							var prestigeOfRibbon = id;
							if (fields.Length == 5)
								try {
									prestigeOfRibbon = int.Parse(fields[4]);
								} catch {
									Log.Error("failed to parse custom ribbon id");
								}

							try {
								Log.Detail("adding custom ribbon '" + nameOfRibbon + "' (id " + id + ") to ribbon pack '" + name + "'");
								Log.Trace("path of custom ribbon file is '" + fileOfRibbon + "'");
								var achievement = new CustomAchievement(id, prestigeOfRibbon);
								achievement.SetName(nameOfRibbon);
								achievement.SetDescription(descOfRibbon);
								var ribbon = new Ribbon(fileOfRibbon, achievement);
								ribbons.Add(ribbon);
							} catch {
								Log.Warning("failed to add custom ribbon '" + nameOfRibbon + "' to ribbon pack '" + name + "'");
							}
						} else {
							Log.Warning("invalid ribbon pack file for " + name + " custom ribbon pack. failed to parse line '" + line + "'");
						}
					}
				}
			}

			private void Load(string config)
			{
				using (TextReader reader = File.OpenText(config)) {
					string line;
					while ((line = reader.ReadLine()) != null) {
						line = line.Trim();
						ReadLine(line);
					}
				}
			}
		}
	}
}