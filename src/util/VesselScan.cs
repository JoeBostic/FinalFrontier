using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nereid
{
	namespace FinalFrontier
	{
		public class VesselScan
		{
			private readonly Dictionary<string, double> resourceCapacity = new Dictionary<string, double>();

			private readonly Dictionary<string, double> resourceLevel = new Dictionary<string, double>();

			public VesselScan(Vessel vessel)
			{
				this.vessel = vessel;
				//
				var sw = new Stopwatch();
				sw.Start();
				//
				Scan(vessel);
				//
				sw.Stop();
				if (Log.IsLogable(Log.LEVEL.INFO)) Log.Info("vessel scanned in " + sw.ElapsedMilliseconds + "ms");
			}

			public Vessel vessel { get; }
			public int parachutes { get; private set; }
			public int stowedParachutes { get; private set; }
			public int deployedParachutes { get; private set; }
			public int semiDeployedParachutes { get; private set; }
			public int activeParachutes { get; private set; }
			public int cutParachutes { get; private set; }

			public double GetResourceLevel(string resourceName)
			{
				try {
					return resourceLevel[resourceName];
				} catch {
					return 0.0;
				}
			}

			public double GetResourceCapacity(string resourceName)
			{
				try {
					return resourceCapacity[resourceName];
				} catch {
					return 0.0;
				}
			}

			public double GetResourcePercentage(string resourceName)
			{
				var level = GetResourceLevel(resourceName);
				var capacity = GetResourceCapacity(resourceName);
				if (capacity <= 0) return double.NaN;
				return level / capacity;
			}

			private void Scan(Vessel vessel)
			{
				if (vessel == null) return;
				foreach (var part in vessel.Parts)
					try {
						foreach (var r in part.Resources) {
							var resourceName = r.info.name;
							var level = GetResourceLevel(resourceName);
							resourceLevel[resourceName] = level + r.amount;
							var capacity = GetResourceCapacity(resourceName);
							resourceCapacity[resourceName] = capacity + r.maxAmount;
						}

						//
						// Parachutes
						foreach (var parachute in part.Modules.OfType<ModuleParachute>()) {
							parachutes++;
							switch (parachute.deploymentState) {
								case ModuleParachute.deploymentStates.ACTIVE:
									activeParachutes++;
									break;
								case ModuleParachute.deploymentStates.DEPLOYED:
									deployedParachutes++;
									break;
								case ModuleParachute.deploymentStates.STOWED:
									stowedParachutes++;
									break;
								case ModuleParachute.deploymentStates.SEMIDEPLOYED:
									semiDeployedParachutes++;
									break;
								case ModuleParachute.deploymentStates.CUT:
									cutParachutes++;
									break;
							}

							if (parachute.deploymentState == ModuleParachute.deploymentStates.STOWED) stowedParachutes++;
						}
					} catch (Exception e) {
						Log.Warning("failed to scan part " + part.name + " in vessel " + vessel.name + ": " + e.GetType());
					}
			}
		}
	}
}