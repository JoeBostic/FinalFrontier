namespace Nereid
{
	namespace FinalFrontier
	{
		public class CelestialBodyMapper
		{
			private const string PLUGIN = "FinalFrontier";

			private readonly CelestialBodyInfo info = new CelestialBodyInfo();

			public CelestialBodyMapper()
			{
				info.ScanGameData();
			}

			public bool IsLegacyBody(CelestialBody body)
			{
				if (info.Contains(body)) return false;
				return true;
			}

			public bool IsGasGiant(CelestialBody body)
			{
				if (IsLegacyBody(body)) {
					if (body.name.Equals("Jool")) return true;
					return false;
				}

				return info.GetBool(body, "", "gas giant");
			}

			public string GetRibbonPath(CelestialBody body, string defaultpath)
			{
				var path = info.GetString(body, PLUGIN, "ribbon path", defaultpath);
				if (!path.EndsWith("/") && !path.EndsWith("\\")) path = path + "/";
				return path;
			}

			public int GetBasePrestige(CelestialBody body)
			{
				const int NO_BASE_PRESTIGE = -1;
				var basePrestige = info.GetInt(body, PLUGIN, "base prestige", NO_BASE_PRESTIGE);
				if (basePrestige == NO_BASE_PRESTIGE) return body.BasePrestige();
				return basePrestige;
			}
		}
	}
}