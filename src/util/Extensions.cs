﻿using System;
using System.IO;

namespace Nereid
{
	namespace FinalFrontier
	{
		public class Pair<T, U>
		{
			public Pair()
			{
			}

			public Pair(T first, U second)
			{
				this.first = first;
				this.second = second;
			}

			public T first { get; set; }
			public U second { get; set; }

			public override string ToString()
			{
				return "(" + first + "," + second + ")";
			}

			public override bool Equals(object right)
			{
				if (right == null) return false;
				var cmp = right as Pair<T, U>;
				if (cmp == null) return false;
				return first.Equals(cmp.first) && second.Equals(cmp.second);
			}


			public override int GetHashCode()
			{
				return first.GetHashCode() + 7 * second.GetHashCode();
			}
		}

		internal static class Extensions
		{
			public static string Envelope(this string s)
			{
				if (s.Trim().Length == 0) return " ";
				return " " + s.Trim() + " ";
			}

			public static bool ContainsIgnoringCase(this string s, string infix)
			{
				return s.IndexOf(infix, StringComparison.OrdinalIgnoreCase) >= 0;
			}

			public static bool isInStableOrbit(this Vessel vessel)
			{
				var orbit = vessel.orbit;
				var atmosphereAltitude = Math.Max(vessel.mainBody.MaxAtmosphereAltitude(), 0.0);
				return orbit.ApA > 0 && orbit.PeA > 0 && orbit.PeA > atmosphereAltitude && orbit.ApR < vessel.mainBody.sphereOfInfluence;
			}

			public static bool InCrewOfActiveFlight(this ProtoCrewMember kerbal)
			{
				var vessel = FlightGlobals.ActiveVessel;
				if (vessel == null) return false;
				foreach (var member in vessel.GetVesselCrew())
					if (kerbal.Equals(member))
						return true;
				return false;
			}

			public static bool IsSolidFuel(this PartResource resource)
			{
				return resource.resourceName.Equals("SolidFuel");
			}

			public static bool IsInOrbit(this Vessel vessel)
			{
				var orbit = vessel.orbit;
				return orbit.ApA > 0 && orbit.PeA > 0;
			}


			public static bool IsLanded(this Vessel vessel)
			{
				if (vessel.situation == Vessel.Situations.LANDED) return true;
				if (vessel.situation == Vessel.Situations.PRELAUNCH) return true;
				return false;
			}

			public static bool IsInAtmosphere(this Vessel vessel)
			{
				if (vessel.mainBody == null) return false;
				if (!vessel.mainBody.atmosphere) return false;
				if (vessel.altitude <= vessel.mainBody.MaxAtmosphereAltitude()) return true;
				return false;
			}

			public static bool IsTouchingGround(this Vessel vessel)
			{
				if (vessel.mainBody == null) return false;
				foreach (var part in vessel.Parts)
					if (part.GroundContact)
						return true;
				return false;
			}

			public static bool IsInAtmosphereWithOxygen(this Vessel vessel)
			{
				if (vessel.mainBody == null) return false;
				if (vessel.altitude > vessel.mainBody.MaxAtmosphereAltitude()) return false;
				return vessel.mainBody.atmosphereContainsOxygen;
			}

			public static bool IsFlag(this Vessel vessel)
			{
				if (vessel == null) return false;
				if (vessel.vesselType != VesselType.Flag) return false;
				return true;
			}

			public static ProtoCrewMember GetCrewMember(this Vessel vessel, string name)
			{
				foreach (var member in vessel.GetVesselCrew())
					if (member.name.Equals(name))
						return member;
				return null;
			}

			public static bool IsTourist(this ProtoCrewMember kerbal)
			{
				return kerbal.type == ProtoCrewMember.KerbalType.Tourist;
			}

			public static bool IsCrew(this ProtoCrewMember kerbal)
			{
				return kerbal.type == ProtoCrewMember.KerbalType.Crew;
			}

			public static bool IsApplicant(this ProtoCrewMember kerbal)
			{
				return kerbal.type == ProtoCrewMember.KerbalType.Applicant;
			}

			public static double MachNumber(this Vessel vessel)
			{
				if (vessel == null) return 0.0;
				if (vessel.mainBody == null) return 0.0;
				return GameUtils.ConvertSpeedToMachNumber(vessel.mainBody, vessel.atmDensity, vessel.altitude, vessel.srf_velocity);
			}

			public static double MachNumberHorizontal(this Vessel vessel)
			{
				var velocity = new Vector3d(vessel.horizontalSrfSpeed, 0, 0);
				return GameUtils.ConvertSpeedToMachNumber(vessel.mainBody, vessel.atmDensity, vessel.altitude, velocity);
			}

			public static bool IsKerbin(this CelestialBody body)
			{
				return body.isHomeWorld;
			}

			public static string ToString(this Vector3d v, string format)
			{
				return "(" + v[0].ToString(format) + "," + v[1].ToString(format) + "," + v[2].ToString(format) + ")";
			}

			public static Vessel GetVessel(this ProtoCrewMember kerbal)
			{
				if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("get vessel for kerbal " + kerbal.name);
				foreach (var vessel in FlightGlobals.fetch.vessels)
				foreach (var member in vessel.GetVesselCrew())
					if (kerbal.name.Equals(member.name))
						return vessel;
				return null;
			}


			public static int BasePrestige(this CelestialBody body)
			{
				switch (body.GetName()) {
					case "Kerbin": return 1000;
					case "Mun": return 1200;
					case "Minmus": return 1400;
					case "Gilly": return 2000;
					case "Ike": return 2200;
					case "Duna": return 2400;
					case "Eve": return 3000;
					case "Moho": return 4000;
					case "Dres": return 5000;
					case "Jool": return 6000;
					case "Vall": return 6200;
					case "Tylo": return 6400;
					case "Bop": return 6600;
					case "Pol": return 6800;
					case "Laythe": return 7000;
					case "Eeloo": return 8000;
					case "Sun": return 9000;
					default:
						Log.Warning("no base prestige for celestial body " + body.GetName());
						return 99000;
				}
			}

			public static void Write(this BinaryWriter writer, HallOfFameBrowser.HallOfFameFilter filter)
			{
				Log.Detail("writing filter " + filter.GetScene() + ": dead=" + filter.showDead + " - assigned=" + filter.showAssigned + ", available=" + filter.showAvailable + ", undecorated=" + filter.showUndecorated + ", flight=" +
				           filter.showFlightOnly);
				writer.Write((long) filter.GetScene());
				writer.Write(filter.showDead);
				writer.Write(filter.showAssigned);
				writer.Write(filter.showAvailable);
				writer.Write(filter.showUndecorated);
				writer.Write(filter.showFlightOnly);
			}

			public static HallOfFameBrowser.HallOfFameFilter ReadFilter(this BinaryReader reader)
			{
				var sceneCode = reader.ReadInt64();
				var scene = (GameScenes) sceneCode;
				var filter = new HallOfFameBrowser.HallOfFameFilter(scene);
				filter.showDead = reader.ReadBoolean();
				filter.showAssigned = reader.ReadBoolean();
				filter.showAvailable = reader.ReadBoolean();
				filter.showUndecorated = reader.ReadBoolean();
				filter.showFlightOnly = reader.ReadBoolean();
				return filter;
			}

			public static void Write(this BinaryWriter writer, HallOfFameBrowser.HallOfFameSorter sorter)
			{
				Log.Detail("writing sorter " + sorter.GetScene() + ": " + sorter.GetMode() + " > " + sorter.GetDirection() + " - stats=" + sorter.GetStatsPredicate() + ", skill=" + sorter.GetSkillPredicate());
				writer.Write((long) sorter.GetScene());
				writer.Write((short) sorter.GetMode());
				writer.Write((short) sorter.GetDirection());
				writer.Write((short) sorter.GetStatsPredicate());
				writer.Write((short) sorter.GetSkillPredicate());
			}

			public static HallOfFameBrowser.HallOfFameSorter ReadSorter(this BinaryReader reader)
			{
				var sceneCode = reader.ReadInt64();
				var direction = (HallOfFameBrowser.HallOfFameSorter.DIRECTION) reader.ReadInt16();
				var mode = (HallOfFameBrowser.HallOfFameSorter.SORT_MODE) reader.ReadInt16();
				var stats_sort = (HallOfFameBrowser.HallOfFameSorter.STATS_SORTING) reader.ReadInt16();
				var skill_sort = (HallOfFameBrowser.HallOfFameSorter.SKILL_SORTING) reader.ReadInt16();
				//
				var scene = (GameScenes) sceneCode;
				//
				// check values
				// TODO: remove magic numbers
				if ((int) direction > 2 || (int) direction < 1) direction = HallOfFameBrowser.HallOfFameSorter.DIRECTION.ASCENDING;
				if ((int) mode > 2 || (int) mode < 1) mode = HallOfFameBrowser.HallOfFameSorter.SORT_MODE.STATISTIC;
				if ((int) stats_sort > 9 || (int) stats_sort < 1) stats_sort = HallOfFameBrowser.HallOfFameSorter.STATS_SORTING.NAME;
				if ((int) skill_sort > 4 || (int) skill_sort < 1) skill_sort = HallOfFameBrowser.HallOfFameSorter.SKILL_SORTING.SPECIALIST;
				if (Log.IsLogable(Log.LEVEL.DETAIL)) Log.Detail("Sorter fro scene " + scene + ": direction=" + direction + " / mode=" + mode + " / stats=" + stats_sort + " / skill=" + skill_sort);
				//
				var sorter = new HallOfFameBrowser.HallOfFameSorter(scene, direction, mode, stats_sort, skill_sort);
				return sorter;
			}

			public static bool IsSun(this CelestialBody body)
			{
				return GameUtils.IsSun(body);
			}

			public static bool IsSunOfHomeWorld(this CelestialBody body)
			{
				if (!body.IsSun()) return false;
				var homeworld = GameUtils.GetHomeworld();
				if (homeworld == null) return false;
				return homeworld.referenceBody.Equals(body);
			}

			public static double MaxAtmosphereAltitude(this CelestialBody body)
			{
				return body.atmosphereDepth;
			}


			/**
		    * Get innermost orbiting planet/moon
		    */
			public static CelestialBody Innermost(this CelestialBody body)
			{
				var minPeA = double.MaxValue;
				CelestialBody result = null;
				foreach (var orbiter in body.orbitingBodies) {
					var pea = orbiter.GetOrbit().PeA;
					if (orbiter.referenceBody != null && orbiter.referenceBody.Equals(body))
						if (pea < minPeA) {
							minPeA = pea;
							result = orbiter;
						}
				}

				return result;
			}


			/*public static String GetPath(this Game game)
		   {
		      String title = game.Title;
		      int index = title.LastIndexOf(" (");
		      String name = title.Remove(index);
  
		      return "saves/" + name;
		   }*/
		}
	}
}