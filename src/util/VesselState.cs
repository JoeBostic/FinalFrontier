namespace Nereid
{
	namespace FinalFrontier
	{
		public class VesselState
		{
			private VesselScan scan;

			public VesselState(Vessel vessel)
			{
				Timestamp = Planetarium.GetUniversalTime();
				Origin = vessel;
				MainBody = vessel.mainBody;
				IsLaunch = false;
				IsLanded = vessel.Landed;
				IsPrelaunch = vessel.situation == Vessel.Situations.PRELAUNCH;
				IsSplashed = vessel.situation == Vessel.Situations.SPLASHED;
				IsLandedOrSplashed = IsSplashed || IsLanded;
				OnSurface = IsSplashed || IsLanded || IsPrelaunch;
				IsEVA = vessel.isEVA;
				HasFlagPlanted = false;
				Situation = vessel.situation;
				InOrbit = vessel.isInStableOrbit();
				ApA = vessel.orbit.ApA;
				ApR = vessel.orbit.ApR;
				PeA = vessel.orbit.PeA;
				PeR = vessel.orbit.PeR;
				atmDensity = vessel.atmDensity;
				MissionTime = vessel.missionTime;
				LaunchTime = vessel.launchTime;
				HasMovedOnSurface = false;
				altitude = vessel.altitude;
				IsInAtmosphere = vessel.IsInAtmosphere();
			}

			public VesselState(VesselState state)
			{
				Origin = state.Origin;
				MainBody = state.MainBody;
				IsLanded = state.IsLanded;
				IsLaunch = state.IsLaunch;
				IsEVA = state.IsEVA;
				HasFlagPlanted = state.HasFlagPlanted;
				Situation = state.Situation;
				InOrbit = state.InOrbit;
				ApA = state.ApA;
				ApR = state.ApR;
				PeA = state.PeA;
				PeR = state.PeR;
				atmDensity = state.atmDensity;
				MissionTime = state.MissionTime;
				MissionTime = state.LaunchTime;
				HasMovedOnSurface = state.HasMovedOnSurface;
				altitude = state.altitude;
				IsInAtmosphere = state.IsInAtmosphere;
			}

			public double Timestamp { get; }
			public Vessel Origin { get; }
			public CelestialBody MainBody { get; }
			public bool IsLanded { get; }
			public bool IsSplashed { get; }
			public bool IsLandedOrSplashed { get; }
			public bool IsEVA { get; private set; }
			public bool IsLaunch { get; private set; }
			public bool IsPrelaunch { get; }
			public bool OnSurface { get; }
			public bool IsInAtmosphere { get; }
			public bool HasFlagPlanted { get; private set; }
			public bool InOrbit { get; }
			public Vessel.Situations Situation { get; private set; }
			public bool HasMovedOnSurface { get; private set; }

			public double ApA { get; }
			public double ApR { get; }
			public double PeA { get; }
			public double PeR { get; }
			public double altitude { get; }

			public double atmDensity { get; }

			public double MissionTime { get; }
			public double LaunchTime { get; }

			public VesselScan ScanVessel()
			{
				if (scan == null) scan = new VesselScan(Origin);
				return scan;
			}

			public VesselState NonEva()
			{
				var state = new VesselState(this);
				state.IsEVA = false;
				return state;
			}

			public VesselState Docked()
			{
				var state = new VesselState(this);
				state.Situation = Vessel.Situations.DOCKED;
				return state;
			}

			public VesselState FlagPlanted()
			{
				var state = new VesselState(this);
				state.HasFlagPlanted = true;
				return state;
			}

			public VesselState MovedOnSurface()
			{
				var state = new VesselState(this);
				state.HasMovedOnSurface = true;
				return state;
			}


			public static VesselState CreateFlagPlantedFromVessel(Vessel vessel)
			{
				var state = new VesselState(vessel);
				state.HasFlagPlanted = true;
				return state;
			}


			public static VesselState CreateLaunchFromVessel(Vessel vessel)
			{
				var state = new VesselState(vessel);
				state.IsLaunch = true;
				return state;
			}

			public override string ToString()
			{
				return Origin.name + " is " + Situation + " orbit=" + InOrbit;
			}
		}
	}
}