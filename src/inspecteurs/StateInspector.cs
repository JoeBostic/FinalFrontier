using System;

namespace Nereid
{
	namespace FinalFrontier
	{
		/**
		 * class for inspecting simple states, such as change in vessel altitute, mach number  or change in Gee-Force
		 */
		public abstract class StateInspector<T>
		{
			private volatile bool changed;

			public abstract void Inspect(T x);

			public void Clear()
			{
				changed = false;
			}

			public void Reset()
			{
				ResetState();
				Clear();
			}

			protected abstract void ResetState();

			public void Change()
			{
				changed = true;
			}

			public bool StateHasChanged()
			{
				return changed;
			}
		}

		public class MachNumberInspector : StateInspector<Vessel>
		{
			private int lastMachNumber;

			public override void Inspect(Vessel vessel)
			{
				if (vessel == null) return;
				if (vessel.situation != Vessel.Situations.FLYING) return;

				var mach = vessel.MachNumber();
				var machWholeNumber = (int) Math.Truncate(mach);
				if (machWholeNumber > lastMachNumber) {
					Log.Detail("mach number increasing to " + machWholeNumber);
					lastMachNumber = machWholeNumber;
					Change();
				} else if (machWholeNumber < lastMachNumber) {
					Log.Detail("mach number decreasing to " + machWholeNumber);
					lastMachNumber = machWholeNumber;
					Change();
				}
			}

			protected override void ResetState()
			{
				Log.Detail("mach number reset");
				lastMachNumber = 0;
			}
		}

		public class AltitudeInspector : StateInspector<Vessel>
		{
			private long lastAltitudeAsMultipleOf1k;

			public override void Inspect(Vessel vessel)
			{
				if (vessel == null) return;
				if (vessel.situation != Vessel.Situations.FLYING) return;

				var altitide = vessel.altitude;
				var alt1000k = 1000 * (int) Math.Truncate(altitide / 1000);
				if (alt1000k > lastAltitudeAsMultipleOf1k) {
					if (Log.IsLogable(Log.LEVEL.DETAIL)) Log.Detail("altitude increasing to " + alt1000k);
					lastAltitudeAsMultipleOf1k = alt1000k;
					Change();
				} else if (alt1000k < lastAltitudeAsMultipleOf1k) {
					if (Log.IsLogable(Log.LEVEL.DETAIL)) Log.Detail("altitude decreasing to " + alt1000k);
					lastAltitudeAsMultipleOf1k = alt1000k;
					Change();
				}
			}

			protected override void ResetState()
			{
			}
		}

		public class OrbitInspector : StateInspector<Vessel>
		{
			private const double MIN_CHANGE_ABS = 1000.0;
			private const double MIN_CHANGE_REL = 0.01;
			private double lastApA;
			private double lastPeA;

			public override void Inspect(Vessel vessel)
			{
				if (vessel == null) return;
				if (vessel.situation != Vessel.Situations.ORBITING) return;
				var orbit = vessel.orbit;
				if (orbit == null) return;

				var apa = orbit.ApA;
				var pea = orbit.PeA;
				var dapa = Math.Abs(lastApA - apa);
				var dpea = Math.Abs(lastPeA - pea);
				if (dpea >= MIN_CHANGE_ABS && dpea >= pea * MIN_CHANGE_REL) {
					lastPeA = pea;
					Change();
				}

				if (dapa >= MIN_CHANGE_ABS && dapa >= apa * MIN_CHANGE_REL) {
					lastApA = apa;
					Change();
				}
			}

			protected override void ResetState()
			{
			}
		}


		public class GeeForceInspector : StateInspector<Vessel>
		{
			public const double DURATION = 3.0;
			private const int MAX_GEE = 15;
			private int gSustained = 1;
			private readonly double[] gTimeOf = new double[MAX_GEE];
			private double maxGeeForce = 1.0;

			public GeeForceInspector()
			{
				ResetState();
			}


			public override void Inspect(Vessel vessel)
			{
				var gForce = vessel.geeForce;
				if (gForce > maxGeeForce) {
					if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("max gee force increased to " + gForce);
					maxGeeForce = gForce;
					var g = (int) gForce;
					var now = Planetarium.GetUniversalTime();
					if (g < MAX_GEE)
						if (gTimeOf[g] <= 0) {
							if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("gee force " + g + " reached at " + now);
							for (var i = 0; i < g; i++)
								if (gTimeOf[i] <= 0)
									gTimeOf[i] = now;
							gTimeOf[g] = now;
						}


					for (var i = 2; i < g && i < MAX_GEE; i++) {
						if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("check g " + i + " since " + gTimeOf[i] + " at " + now);
						if (gTimeOf[i] <= 0) break;
						if (gTimeOf[i] + DURATION > now) {
							if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("g " + i + " not reached");
							return;
						}

						if (Log.IsLogable(Log.LEVEL.DETAIL)) Log.Detail("g " + i + " sustained for " + (now - gTimeOf[i]) + " seconds");
						gSustained = i;
						Change();
					}
				}
			}

			public int GetGeeNumber()
			{
				return gSustained;
			}


			protected override void ResetState()
			{
				Log.Detail("reset of gee inspector state");
				maxGeeForce = 1.0;
				gSustained = 1;
				for (var i = 0; i < MAX_GEE; i++) gTimeOf[i] = 0.0;
			}
		}

		public class AtmosphereInspector : StateInspector<Vessel>
		{
			private bool inAtmosphere = true;

			public override void Inspect(Vessel vessel)
			{
				var inAtmosphere = vessel.IsInAtmosphere();
				if (this.inAtmosphere != inAtmosphere) {
					this.inAtmosphere = inAtmosphere;
					Change();
				}
			}

			protected override void ResetState()
			{
				var vessel = FlightGlobals.ActiveVessel;
				if (vessel != null)
					inAtmosphere = vessel.IsInAtmosphere();
				else
					inAtmosphere = false;
			}
		}
	} // end of FinalFrontier namespace */
}