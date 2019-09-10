using System;
using System.Collections.Generic;

namespace Nereid
{
	namespace FinalFrontier
	{
		/**
		 * This class observes vessel data for different vessel and keeps track of some vessel states like gee force and launch time 
		 * even when the player switches between vessels.
		 * 
		 */
		public class VesselObserver
		{
			public const double NO_TIME = -1.0;
			private static volatile VesselObserver instance;

			private readonly Dictionary<Guid, VesselData> vesselData = new Dictionary<Guid, VesselData>();


			private VesselObserver()
			{
			}

			//
			public static VesselObserver Instance()
			{
				if (instance == null) {
					instance = new VesselObserver();
					Log.Info("new vessel observer instance created");
				}

				return instance;
			}

			private VesselData GetVesselData(Vessel vessel)
			{
				var id = vessel.id;
				VesselData data;
				if (!vesselData.ContainsKey(id)) {
					data = new VesselData();
					vesselData.Add(id, data);
				} else {
					data = vesselData[id];
				}

				return data;
			}

			public void SetKerbinLaunchTime(Vessel vessel, double time)
			{
				Log.Detail("set kerbin launch time for " + vessel.name + ": " + time);
				var data = GetVesselData(vessel);
				data.HomeworldLaunchTime = time;
			}

			public double GetHomeworldLaunchTime(Vessel vessel)
			{
				var id = vessel.id;
				if (vesselData.ContainsKey(id)) return vesselData[id].HomeworldLaunchTime;
				return NO_TIME;
			}

			public void SetGeeForceSustained(Vessel vessel, int geeForce)
			{
				Log.Detail("set sustained gee force for " + vessel.name + ": " + geeForce);
				var data = GetVesselData(vessel);
				data.GeeForce = geeForce;
			}

			public int GetGeeForceSustained(Vessel vessel)
			{
				var id = vessel.id;
				if (vesselData.ContainsKey(id)) return vesselData[id].GeeForce;
				return 1;
			}

			public void Revert(double time)
			{
				Log.Detail("reverting vessel observer to " + time);
				foreach (var id in vesselData.Keys) {
					var data = vesselData[id];
					if (data.HomeworldLaunchTime > time) data.HomeworldLaunchTime = NO_TIME;
				}
			}

			public override string ToString()
			{
				var result = "[VesselObserver::";
				foreach (var id in vesselData.Keys) result = result + id + "=" + vesselData[id].HomeworldLaunchTime + "/";
				result = result + "]";
				return result;
			}

			private class VesselData
			{
				public int GeeForce = 1;
				public double HomeworldLaunchTime = NO_TIME;
			}
		}
	}
}