﻿using System;
using UnityEngine;

namespace Nereid
{
	namespace FinalFrontier
	{
		public class GameUtils
		{
			public static void SetPermadeathEnabled(bool enabled)
			{
				if (HighLogic.CurrentGame == null) return;
				if (HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn == enabled) {
					HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn = !enabled;
					HighLogic.CurrentGame.Updated();
					Log.Detail("permadeath " + (enabled ? "enabled" : "disabled"));
				}
			}

			public static bool IsPermadeathEnabled()
			{
				if (HighLogic.CurrentGame == null) return false;
				return !HighLogic.CurrentGame.Parameters.Difficulty.MissingCrewsRespawn;
			}

			public static void SetKerbinTimeEnabled(bool enabled)
			{
				if (GameSettings.KERBIN_TIME != enabled) {
					GameSettings.KERBIN_TIME = enabled;
					Log.Detail("kerbin time " + (enabled ? "enabled" : "disabled"));
				}
			}

			public static bool IsKerbinTimeEnabled()
			{
				return GameSettings.KERBIN_TIME;
			}

			public static CelestialBody GetCelestialBody(string name)
			{
				var body = PSystemManager.Instance.localBodies.Find(x => x.name.Equals(name));
				if (body == null) Log.Warning("celestial body '" + name + "' not found");
				return body;
			}

			public static double ConvertSpeedToMachNumber(CelestialBody body, double atmDensity, double altitude, Vector3 velocity)
			{
				if (FinalFrontier.farAdapter.IsInstalled()) return FinalFrontier.farAdapter.GetMachNumber(body, atmDensity, velocity);
				return ApproximateMachNumber(body, atmDensity, altitude, velocity);
			}

			public static double ApproximateMachNumber(CelestialBody body, double atmDensity, double altitude, Vector3 velocity)
			{
				if (altitude < 0) altitude = 0;
				// a technical constant for speed of sound appromixation 
				// experimental resolved; feel free to make better suggestions
				var c1 = altitude / 16000;
				var c2 = altitude / 39000;
				var c = 1.05 + altitude / 15000 * (1 + altitude / 10000) + Math.Pow(c1, 4.15) + Math.Pow(c2, 5.58);

				return velocity.magnitude / (300 * Math.Sqrt(atmDensity)) / c;
			}

			public static bool IsSun(CelestialBody body)
			{
				return body.orbit == null || body.orbit.referenceBody == null;
			}

			public static CelestialBody GetHomeworld()
			{
				return PSystemManager.Instance.localBodies.Find(x => x.isHomeWorld);
			}

			public static CelestialBody GetSun()
			{
				return PSystemManager.Instance.localBodies.Find(x => x.orbit == null || x.orbit.referenceBody == null);
			}

			public static CelestialBody GetOutermostPlanet()
			{
				CelestialBody result = null;
				double distanceFromSun = 0;

				var center = GetSun();
				if (center == null) {
					Log.Error("no central star found; cant get outermost planet");
					return null;
				}

				if (PSystemManager.Instance == null || PSystemManager.Instance.localBodies == null) return null;
				foreach (var body in PSystemManager.Instance.localBodies)
					//Log.Test("testing "+body.name);
					if (body.orbit != null) {
						var orbit = body.orbit;
						if (center.Equals(orbit.referenceBody)) {
							//Log.Test("OUTMOST " + body.name + " " + body.orbit);
							//Log.Test("ORBIT");
							var d = body.orbit.ApA;
							//Log.Test("outmost APA=" + d);
							if (d > distanceFromSun) {
								result = body;
								distanceFromSun = d;
							}
						}
					}

				if (result == null) Log.Error("no outmost celestial body found");
				return result;
			}

			public static double GetDistanceToSun(Vessel vessel)
			{
				if (vessel == null) return 0.0;
				var sun = GetSun();
				if (sun == null) return 0.0;
				var posVessel = vessel.GetWorldPos3D();
				var posSun = sun.GetWorldSurfacePosition(0.0, 0.0, 0.0);
				return Vector3d.Distance(posVessel, posSun);
			}

			public static CelestialBody GetBodyForName(string name)
			{
				Log.Detail("searching celestial body " + name);
				foreach (var body in PSystemManager.Instance.localBodies)
					if (body.name.Equals(name))
						return body;
				Log.Detail("celestial body " + name + " not found");
				return null;
			}

			public static ProtoCrewMember GetKerbalForName(Game game, string name)
			{
				if (game == null) return null;
				foreach (var kerbal in game.CrewRoster.Crew)
					if (kerbal.name.Equals(name))
						return kerbal;
				return null;
			}

			public static ProtoCrewMember GetKerbalForName(string name)
			{
				return GetKerbalForName(HighLogic.CurrentGame, name);
			}

			public static void DumpCrewRosterToLog()
			{
				if (Log.IsLogable(Log.LEVEL.DETAIL)) {
					Log.Detail("CrewRoster:");
					if (HighLogic.CurrentGame != null)
						foreach (var kerbal in HighLogic.CurrentGame.CrewRoster.Crew)
							Log.Detail(kerbal.name + " " + kerbal.rosterStatus + " " + kerbal.type);
					else
						Log.Detail("NO CREW ROSTER");
					Log.Detail("End of CrewRoster -");
				}
			}

			public static double GetResourcePercentageLeft(Vessel vessel, string resourceName)
			{
				var amount = 0.0;
				var capacity = 0.0;

				foreach (var part in vessel.Parts)
				foreach (var r in part.Resources)
					if (r.info.name.Equals(resourceName)) {
						amount += r.amount;
						capacity += r.maxAmount;
					}

				if (capacity > 0) return amount / capacity;
				return double.NaN;
			}
		}
	}
}