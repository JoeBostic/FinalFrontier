using System.Diagnostics;
using Contracts;
using KSP.UI.Screens;

namespace Nereid
{
	namespace FinalFrontier
	{
		internal class EventObserver
		{
			private static readonly VesselObserver vesselObserver = VesselObserver.Instance();
			private readonly AltitudeInspector altitudeInspector = new AltitudeInspector();

			private readonly AtmosphereInspector atmosphereInspector = new AtmosphereInspector();

			//
			private readonly GeeForceInspector geeForceInspector = new GeeForceInspector();
			private readonly MachNumberInspector machInspector = new MachNumberInspector();

			private readonly MissionSummary missionSummary = new MissionSummary();
			private readonly OrbitInspector orbitInspector = new OrbitInspector();
			private readonly AchievementRecorder recorder;
			private CelestialBody currentSphereOfInfluence;
			private bool deepAthmosphere;

			private bool landedVesselHasMoved;

			//
			private Vector3d lastVesselSurfacePosition;

			// custom events
			private bool orbitClosed;

			// previous active vessel State
			private volatile VesselState previousVesselState;
			private long updateCycle;

			public EventObserver()
			{
				Log.Info("EventObserver:: registering events");
				//
				// recorder for recording in hall of fame
				recorder = new AchievementRecorder();
				//
				// Game
				GameEvents.onGamePause.Add(OnGamePause);
				GameEvents.onGameSceneLoadRequested.Add(OnGameSceneLoadRequested);
				GameEvents.onGameStateCreated.Add(OnGameStateCreated);
				GameEvents.onGameSceneSwitchRequested.Add(OnGameSceneSwitchRequested);
				GameEvents.onGUIRecoveryDialogSpawn.Add(OnGUIRecoveryDialogSpawn);
				GameEvents.onGUIRecoveryDialogDespawn.Add(onGUIRecoveryDialogDespawn);
				//
				// Docking
				GameEvents.onPartCouple.Add(OnPartCouple);
				GameEvents.onPartAttach.Add(OnPartAttach);
				// EVA
				GameEvents.onCrewOnEva.Add(OnCrewOnEva);
				GameEvents.onCrewBoardVessel.Add(OnCrewBoardVessel);
				// Vessel
				GameEvents.onCollision.Add(OnCollision);
				GameEvents.onVesselWasModified.Add(OnVesselWasModified);
				GameEvents.onStageActivate.Add(OnStageActivate);
				GameEvents.onJointBreak.Add(OnJointBreak);
				GameEvents.onLaunch.Add(OnLaunch);
				GameEvents.onVesselGoOnRails.Add(OnVesselGoOnRails);
				GameEvents.onVesselSOIChanged.Add(OnVesselSOIChanged);
				GameEvents.onVesselSituationChange.Add(OnVesselSituationChange);
				GameEvents.onVesselChange.Add(OnVesselChange);
				GameEvents.onVesselRecovered.Add(OnVesselRecovered);
				GameEvents.onVesselOrbitClosed.Add(OnVesselOrbitClosed); // wont work in 0.23
				GameEvents.VesselSituation.onFlyBy.Add(OnFlyBy);
				GameEvents.VesselSituation.onReachSpace.Add(OnReachSpace);
				GameEvents.Contract.onCompleted.Add(OnContractCompleted);
				GameEvents.Contract.onFailed.Add(OnContractFailed);
				GameEvents.VesselSituation.onOrbit.Add(OnOrbit);
				GameEvents.OnScienceRecieved.Add(OnScienceReceived);
				GameEvents.onFlightReady.Add(OnFlightReady);
				// Kerbals
				GameEvents.onKerbalAdded.Add(OnKerbalAdded);
				GameEvents.onKerbalRemoved.Add(OnKerbalRemoved);
				GameEvents.onKerbalStatusChange.Add(OnKerbalStatusChange);
				GameEvents.OnCrewmemberHired.Add(OnCrewmemberHired);
				//
				// Other
				GameEvents.OnProgressAchieved.Add(OnProgressAchieved);
				//
			}


			private void OnFlyBy(Vessel vessel, CelestialBody body)
			{
				// for later usage
			}

			private void OnFlightReady()
			{
				// for later usage
			}


			private void onGUIRecoveryDialogDespawn(MissionRecoveryDialog dialog)
			{
				// Mission Summary
				if (FinalFrontier.configuration.IsMissionSummaryEnabled() && missionSummary.MissionContainsCrewData()) {
					Log.Info("showing mission summary window");
					var summary = new MissionSummaryWindow();
					summary.SetMissionContents(missionSummary);
					summary.SetVisible(true);
				}
			}

			private void OnGUIRecoveryDialogSpawn(MissionRecoveryDialog dialog)
			{
				// not used
			}

			private void OnOrbit(Vessel vessel, CelestialBody body)
			{
				if (vessel == null) return;
				Log.Info("vessel " + vessel.name + " has reached orbit around " + body.name);
				if (vessel.isActiveVessel) CheckAchievementsForVessel(vessel);
			}

			// wont detect zero atmosphere, because state SUB_ORBITAL begins above atmosphere height
			private void OnReachSpace(Vessel vessel)
			{
				if (vessel == null) return;
				Log.Info("vessel " + vessel.name + " has reached space");
				if (vessel.isActiveVessel) CheckAchievementsForVessel(vessel);
			}

			// KSP 1.0 (wont work anymore)
			private void OnScienceReceived(float science, ScienceSubject subject, ProtoVessel vessel)
			{
				OnScienceReceived(science, subject, vessel, true);
			}


			private void OnScienceReceived(float science, ScienceSubject subject, ProtoVessel vessel, bool flag)
			{
				Log.Detail("EventObserver::OnScienceReceived: " + science + ", flag=" + flag);
				if (vessel == null) return;

				// no science, no record
				if (science <= 0) {
					if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("science ignored");
					return;
				}

				var halloffame = HallOfFame.Instance();
				//
				halloffame.BeginArwardOfRibbons();
				try {
					foreach (var kerbal in vessel.GetVesselCrew())
						// we want to check crew member only
						if (kerbal.IsCrew()) {
							halloffame.RecordScience(kerbal, science);
							CheckAchievementsForCrew(kerbal, false);
						}
				} finally {
					// commit ribbons
					halloffame.EndArwardOfRibbons();
				}
			}

			private void OnContractFailed(Contract contract)
			{
				//Log.Test("contract failed " + contract.Title);
			}

			private void OnContractCompleted(Contract contract)
			{
				var vessel = FlightGlobals.ActiveVessel;
				if (vessel == null) return;
				var halloffame = HallOfFame.Instance();
				//
				halloffame.BeginArwardOfRibbons();
				try {
					foreach (var kerbal in vessel.GetVesselCrew())
						// we want to check crew member only
						if (kerbal.IsCrew()) {
							halloffame.RecordContract(kerbal);
							CheckAchievementsForCrew(kerbal, false);
							CheckAchievementsForContracts(kerbal, contract);
						}
				} finally {
					halloffame.EndArwardOfRibbons();
				}
			}

			private void OnKerbalAdded(ProtoCrewMember kerbal)
			{
				Log.Info("kerbal added: " + kerbal.name);
				// just make sure this kerbal is in the hall of fame
				HallOfFame.Instance().GetEntry(kerbal);
				// and refresh
				HallOfFame.Instance().Refresh();
			}

			private void OnCrewmemberHired(ProtoCrewMember kerbal, int n)
			{
				Log.Info("crew member hired: " + kerbal.name + " (" + n + ")");
				// just make sure this kerbal is in the hall of fame
				HallOfFame.Instance().GetEntry(kerbal);
				// and refresh
				HallOfFame.Instance().Refresh();
			}

			private void OnKerbalRemoved(ProtoCrewMember kerbal)
			{
				Log.Info("kerbal removed: " + kerbal.name);
				HallOfFame.Instance().Remove(kerbal);
			}

			private void OnKerbalStatusChange(ProtoCrewMember kerbal, ProtoCrewMember.RosterStatus oldState, ProtoCrewMember.RosterStatus newState)
			{
				if (kerbal == null) return;
				Log.Info("kerbal status change: " + kerbal.name + " from " + oldState + " to " + newState + " at time " + Planetarium.GetUniversalTime());
				HallOfFame.Instance().Refresh();
				//
				// check for achievements caused by status changes
				// (crew member only)
				/* not working because of the way KSP handles cre respawning
				if (kerbal.IsCrew())
				{
				   Log.Detail("kerbal with status change is crew member");
				   CheckAchievementsForRosterStatus(kerbal, oldState, newState);
				}*/
			}

			private void OnProgressAchieved(ProgressNode node)
			{
				var vessel = FlightGlobals.ActiveVessel;
				// records achieved while not in a vessel won't count
				if (vessel == null) return;
				// records achieved while on a launch pad won't count
				if (vessel.situation == Vessel.Situations.PRELAUNCH) return;
				// ok, now check the record
				CheckAchievementsForProgress(node);
			}

			private void OnVesselWasModified(Vessel vessel)
			{
				// not used
			}

			private void OnGamePause()
			{
				// not used
			}

			private void OnPartActionUICreate(Part part)
			{
				// not used
			}

			private void OnSceneChange()
			{
				// not used
			}

			private void OnPartAttach(GameEvents.HostTargetAction<Part, Part> action)
			{
				// not used
			}

			private void OnPartCouple(GameEvents.FromToAction<Part, Part> action)
			{
				// checko for docking manouvers
				//
				// we are just checking flights
				if (!HighLogic.LoadedSceneIsFlight) return;
				var from = action.from;
				var to = action.to;
				Log.Detail("part couple event");
				// eva wont count as docking
				if (from == null || from.vessel == null || from.vessel.isEVA) return;
				Log.Detail("from vessel " + from.vessel);
				if (to == null || to.vessel == null || to.vessel.isEVA) return;
				Log.Detail("to vessel " + to.vessel);
				var vessel = action.from.vessel.isActiveVessel ? action.from.vessel : action.to.vessel;
				if (vessel != null && vessel.isActiveVessel) {
					Log.Info("docking vessel " + vessel.name);
					var vesselState = new VesselState(vessel);
					// check achievements, but mark vessel as docked
					CheckAchievementsForVessel(vesselState.Docked());
					// record docking maneuver
					recorder.RecordDocking(vessel);
				}
			}

			private void OnVesselOrbitClosed(Vessel vessel)
			{
				orbitClosed = true;
				Log.Detail("EventObserver:: OnVesselOrbitClosed " + vessel.GetName());
				if (vessel.isActiveVessel) CheckAchievementsForVessel(vessel);
			}

			private void OnEnteringDeepAthmosphere(Vessel vessel)
			{
				Log.Detail("EventObserver:: OnEnteringDeepAthmosphere " + vessel.GetName());
				if (vessel.isActiveVessel) CheckAchievementsForVessel(vessel);
			}

			private void OnLandedVesselMove(Vessel vessel)
			{
				Log.Detail("EventObserver:: OnLandedVesselMove " + vessel.GetName());
				if (vessel.isActiveVessel) {
					var vesselState = new VesselState(vessel);
					CheckAchievementsForVessel(vesselState.MovedOnSurface());
				}
			}

			private void OnCollision(EventReport report)
			{
				// just for safety
				if (report.origin != null && report.origin.vessel != null) {
					var vessel = report.origin.vessel;
					if (vessel.isActiveVessel) {
						Log.Info("EventObserver:: collision detected for active vessel " + vessel.GetName());
						CheckAchievementsForVessel(vessel, report);
					}
				}
			}

			private void OnGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> change)
			{
				if (change.from == GameScenes.SPACECENTER && change.to != GameScenes.SPACECENTER) {
					Log.Info("clearing mission summary info");
					missionSummary.Clear();
				}
			}

			private void OnGameSceneLoadRequested(GameScenes scene)
			{
				Log.Info("EventObserver:: OnGameSceneLoadRequested: " + scene + " current=" + HighLogic.LoadedScene);
				previousVesselState = null;
			}

			private void OnJointBreak(EventReport report)
			{
				// not used
			}

			private void OnStageActivate(int stage)
			{
				var vessel = FlightGlobals.ActiveVessel;
				if (vessel == null) return;
				Log.Detail("stage " + stage + " activated for vessel " + vessel.name + " current mission time is " + vessel.missionTime);
			}

			private void OnCrewBoardVessel(GameEvents.FromToAction<Part, Part> action)
			{
				Log.Detail("EventObserver:: crew board vessel " + action.from.GetType());
				if (action.from == null || action.from.vessel == null) return;
				var from = action.from;
				if (action.to == null || action.to.vessel == null) return;
				// boarding crew is still the active vessel
				var eva = action.from.vessel;
				var vessel = action.to.vessel;
				var nameOfKerbalOnEva = eva.vesselName;
				// find kerbal that return from eva in new crew
				var member = vessel.GetCrewMember(nameOfKerbalOnEva);
				if (member != null && member.IsCrew()) {
					Log.Detail(member.name + " returns from EVA to " + vessel.name);
					recorder.RecordBoarding(member);
					CheckAchievementsForCrew(member);
				} else {
					Log.Warning("boarding crew member " + nameOfKerbalOnEva + " not found in vesssel " + vessel.name);
				}
			}

			private void OnCrewOnEva(GameEvents.FromToAction<Part, Part> action)
			{
				Log.Detail("EventObserver:: crew on EVA");
				if (action.from == null || action.from.vessel == null) return;
				if (action.to == null || action.to.vessel == null) return;
				var vessel = action.from.vessel;
				var crew = action.to.vessel;
				// record EVA
				foreach (var member in crew.GetVesselCrew())
					// record crew member only
					if (member.IsCrew())
						recorder.RecordEva(member, vessel);
				// the previous vessel shoud be previous
				previousVesselState = new VesselState(vessel);
				// current vessel is crew
				CheckAchievementsForVessel(crew);
			}

			private void OnGameStateCreated(Game game)
			{
				Log.Detail("OnGameStateCreated ");
				//
				// do not load a game while in MAIN-MENU or SETTINGS
				// TODO: check if STILL NECESSARY????
				if (HighLogic.LoadedScene == GameScenes.MAINMENU || HighLogic.LoadedScene == GameScenes.SETTINGS) {
					// clear the hall of fame to avoid new games with ribbons from previos loads...
					HallOfFame.Instance().Clear();
					return;
				}

				// no game, no fun
				if (game == null) {
					Log.Warning("no game");
					return;
				}

				Log.Info("EventObserver:: OnGameStateCreated " + game.UniversalTime + ", game status: " + game.Status + ", scene " + HighLogic.LoadedScene);

				// we have to detect a closed orbit again...
				orbitClosed = false;

				ResetInspectors();

				vesselObserver.Revert(game.UniversalTime);
			}


			private void OnVesselRecovered(ProtoVessel vessel, bool flag)
			{
				// TODO: check what synopsis of "flag"
				if (vessel == null) {
					Log.Warning("vessel recover without a valid vessel detected");
					return;
				}

				//
				// update mission summary
				missionSummary.AddSummaryForCrewOfVessel(vessel);

				Log.Info("vessel recovered " + vessel.vesselName);
				// record recover of vessel
				recorder.RecordVesselRecovered(vessel);
				// check for kerbal specific achiements
				HallOfFame.Instance().BeginArwardOfRibbons();
				foreach (var member in vessel.GetVesselCrew()) CheckAchievementsForCrew(member);
				HallOfFame.Instance().EndArwardOfRibbons();
				//
				// refresh roster status
				HallOfFame.Instance().Refresh();
				//
			}


			private void OnLaunch(EventReport report)
			{
				Log.Detail("vessel launched");
				var vessel = FlightGlobals.ActiveVessel;
				if (vessel == null) return;
				ResetInspectors();
			}

			private void OnVesselGoOnRails(Vessel vessel)
			{
				// not used
			}

			private void CheckForLaunch(Vessel vessel, Vessel.Situations from, Vessel.Situations to)
			{
				if (from == Vessel.Situations.PRELAUNCH && to != Vessel.Situations.PRELAUNCH) {
					ResetInspectors();
					//
					Log.Detail("vessel mission time at launch: " + vessel.missionTime);
					//
					if (!vessel.isActiveVessel) return;
					//
					// check for Kerbin launch
					if (vessel.mainBody != null && vessel.mainBody.IsKerbin()) {
						Log.Info("launch of vessel " + vessel.name + " at kerbin detected at " + Utils.ConvertToKerbinTime(vessel.launchTime));
						vesselObserver.SetKerbinLaunchTime(vessel, vessel.launchTime);
					}

					//
					ResetLandedVesselHasMovedFlag();
					recorder.RecordLaunch(vessel);
					//
					var vesselState = VesselState.CreateLaunchFromVessel(vessel);
					CheckAchievementsForVessel(vesselState);
				}
			}

			private void OnVesselSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> e)
			{
				var vessel = e.host;
				//
				if (vessel == null) {
					Log.Warning("vessel situation change without a valid vessel detected");
					return;
				}

				//
				// check for a (first) launch
				CheckForLaunch(vessel, e.from, e.to);

				if (vessel.isActiveVessel) {
					if (vessel.situation != Vessel.Situations.LANDED) ResetLandedVesselHasMovedFlag();
					//
					Log.Info("situation change for active vessel from " + e.from + " to " + e.to);
					CheckAchievementsForVessel(vessel);
				} else {
					if (vessel != null && vessel.IsFlag() && vessel.situation == Vessel.Situations.LANDED) {
						Log.Info("situation change for flag");
						var active = FlightGlobals.ActiveVessel;
						if (active != null && active.isEVA) {
							var vesselState = VesselState.CreateFlagPlantedFromVessel(active);
							CheckAchievementsForVessel(vesselState);
						}
					}
				}
			}

			private void OnVesselChange(Vessel vessel)
			{
				if (vessel == null) {
					Log.Warning("vessel change without a valid vessel detected");
					return;
				}

				// we have to detect a closed orbit again...
				orbitClosed = false;

				ResetInspectors();

				ResetLandedVesselHasMovedFlag();
				//
				Log.Info("EventObserver:: OnVesselChange " + vessel.GetName());
				if (!vessel.isActiveVessel) return;
				//
				previousVesselState = null;
				CheckAchievementsForVessel(vessel);
				//
				Log.Detail("vessel change finished");
			}

			private void OnVesselSOIChanged(GameEvents.HostedFromToAction<Vessel, CelestialBody> e)
			{
				orbitClosed = false;
				currentSphereOfInfluence = e.to;
				var vessel = e.host;
				Log.Detail("sphere of influence changed to " + currentSphereOfInfluence + " for vessel " + vessel);
				if (vessel != null) CheckAchievementsForVessel(vessel);
			}

			private void CheckAchievementsForVessel(VesselState previous, VesselState current, EventReport report, bool hasToBeFirst)
			{
				if (current != null)
					foreach (Ribbon ribbon in RibbonPool.Instance()) {
						var achievement = ribbon.GetAchievement();
						if (achievement.HasToBeFirst() == hasToBeFirst) {
							var vessel = current.Origin;
							// check situationchanges
							if (achievement.Check(previous, current)) recorder.Record(ribbon, vessel);
							// check events
							if (report != null && achievement.Check(report)) recorder.Record(ribbon, vessel);
						}
					}
				else
					Log.Warning("no current vessel state; achievemnts not checked");
			}

			private void CheckAchievementsForVessel(VesselState previous, VesselState current, EventReport report)
			{
				// first check all first achievements
				CheckAchievementsForVessel(previous, current, report, true);
				// now check the rest
				CheckAchievementsForVessel(previous, current, report, false);
			}


			private void CheckAchievementsForVessel(VesselState currentVesselState, EventReport report = null)
			{
				Log.Detail("EventObserver:: checkArchivements for vessel state");
				var sw = new Stopwatch();
				sw.Start();
				//
				CheckAchievementsForVessel(previousVesselState, currentVesselState, report);
				//
				sw.Stop();
				previousVesselState = currentVesselState;
				Log.Detail("EventObserver:: checkArchivements done in " + sw.ElapsedMilliseconds + " ms");
			}

			private void CheckAchievementsForVessel(Vessel vessel, EventReport report = null)
			{
				// just delegate
				CheckAchievementsForVessel(new VesselState(vessel), report);
			}

			private void CheckAchievementsForCrew(ProtoCrewMember kerbal, bool hasToBeFirst)
			{
				if (kerbal == null) return;
				// we want to check crew member only
				if (!kerbal.IsCrew()) return;
				// ok, lets check this kerbal
				var entry = HallOfFame.Instance().GetEntry(kerbal);
				if (entry != null)
					foreach (Ribbon ribbon in RibbonPool.Instance()) {
						var achievement = ribbon.GetAchievement();
						if (achievement.HasToBeFirst() == hasToBeFirst)
							if (achievement.Check(entry))
								recorder.Record(ribbon, kerbal);
					}
				else
					Log.Warning("no entry for kerbal " + kerbal.name + " in hall of fame");
			}

			private void CheckAchievementsForContracts(ProtoCrewMember kerbal, Contract contract)
			{
				Log.Detail("EventObserver:: checkArchivements for contract");
				var sw = new Stopwatch();
				sw.Start();

				CheckAchievementsForContracts(kerbal, contract, true);
				CheckAchievementsForContracts(kerbal, contract, false);

				Log.Detail("EventObserver:: checkArchivements done in " + sw.ElapsedMilliseconds + " ms");
			}

			private void CheckAchievementsForContracts(ProtoCrewMember kerbal, Contract contract, bool hasToBeFirst)
			{
				if (kerbal == null) return;
				// we want to check crew member only
				if (!kerbal.IsCrew()) return;
				// ok, lets check the kerbal
				foreach (Ribbon ribbon in RibbonPool.Instance()) {
					var achievement = ribbon.GetAchievement();
					if (achievement.HasToBeFirst() == hasToBeFirst)
						if (achievement.Check(contract))
							recorder.Record(ribbon, kerbal);
				}
			}

			private void CheckAchievementsForRosterStatus(ProtoCrewMember kerbal, ProtoCrewMember.RosterStatus oldState, ProtoCrewMember.RosterStatus newState)
			{
				Log.Detail("EventObserver:: checkArchivements for roster status");
				var sw = new Stopwatch();
				sw.Start();

				CheckAchievementsForRosterStatus(kerbal, oldState, newState, true);
				CheckAchievementsForRosterStatus(kerbal, oldState, newState, false);

				Log.Detail("EventObserver:: checkArchivements done in " + sw.ElapsedMilliseconds + " ms");
			}

			private void CheckAchievementsForRosterStatus(ProtoCrewMember kerbal, ProtoCrewMember.RosterStatus oldState, ProtoCrewMember.RosterStatus newState, bool hasToBeFirst)
			{
				foreach (Ribbon ribbon in RibbonPool.Instance()) {
					var achievement = ribbon.GetAchievement();
					if (achievement.Check(kerbal, oldState, newState))
						// record crew member only
						if (kerbal.IsCrew())
							recorder.Record(ribbon, kerbal);
				}
			}


			private void CheckAchievementsForProgress(ProgressNode node)
			{
				var vessel = FlightGlobals.ActiveVessel;
				var halloffame = HallOfFame.Instance();
				if (vessel != null)
					foreach (Ribbon ribbon in RibbonPool.Instance()) {
						var achievement = ribbon.GetAchievement();
						if (achievement.Check(node)) {
							halloffame.BeginArwardOfRibbons();
							try {
								foreach (var member in vessel.GetVesselCrew())
									// record crew member only
									if (member.IsCrew())
										recorder.Record(ribbon, member);
							} finally {
								halloffame.EndArwardOfRibbons();
							}
						}
					}
			}

			private void CheckAchievementsForCrew(ProtoCrewMember kerbal)
			{
				// just for safety
				if (kerbal == null) return;
				// we want to check crew member only
				if (!kerbal.IsCrew()) return;
				// ok, lets check the kerbal
				Log.Detail("EventObserver:: checkArchivements for kerbal " + kerbal.name);
				var sw = new Stopwatch();
				sw.Start();
				//
				// first check all first achievements
				CheckAchievementsForCrew(kerbal, true);
				// now check the rest
				CheckAchievementsForCrew(kerbal, false);
				//
				sw.Stop();
				Log.Detail("EventObserver:: checkArchivements done in " + sw.ElapsedMilliseconds + " ms");
			}

			// TODO: move to EventObserver
			private void FireCustomEvents()
			{
				// detect events only in flight
				if (HighLogic.LoadedScene != GameScenes.FLIGHT) return;
				//
				var vessel = FlightGlobals.ActiveVessel;
				if (vessel != null) {
					// undetected SOI change (caused by hyperedit or other mods)
					if (currentSphereOfInfluence == null || !currentSphereOfInfluence.Equals(vessel.mainBody)) OnVesselSOIChanged(new GameEvents.HostedFromToAction<Vessel, CelestialBody>(vessel, currentSphereOfInfluence, vessel.mainBody));
					// Orbit closed
					var inOrbit = vessel.isInStableOrbit();
					if (inOrbit && !orbitClosed) {
						Log.Info("orbit closed detected for vessel " + vessel.name);
						OnVesselOrbitClosed(vessel);
					}

					orbitClosed = inOrbit;
					//
					// deep atmosphere
					var atmDensity = vessel.atmDensity;
					if (!deepAthmosphere && atmDensity >= 10.0) {
						Log.Trace("vessel entering deep athmosphere");
						deepAthmosphere = true;
						OnEnteringDeepAthmosphere(vessel);
					} else if (deepAthmosphere && atmDensity < 10.0) {
						deepAthmosphere = false;
					}
				} else {
					orbitClosed = false;
					deepAthmosphere = false;
				}

				//
				// G-force increased
				var geeForceStateChanged = geeForceInspector.StateHasChanged();
				if (geeForceStateChanged) VesselObserver.Instance().SetGeeForceSustained(vessel, geeForceInspector.GetGeeNumber());
				// Mach increased
				// AtmosphereChanged
				// Orbit changed
				// gee force changed
				if (machInspector.StateHasChanged() || atmosphereInspector.StateHasChanged() || orbitInspector.StateHasChanged() || geeForceStateChanged) CheckAchievementsForVessel(vessel);
			}

			private void ResetLandedVesselHasMovedFlag()
			{
				Log.Detail("reset of LandedVesselHasMovedFlag");
				landedVesselHasMoved = false;
				lastVesselSurfacePosition = Vector3d.zero;
			}

			private bool checkIfLandedVesselHasMoved(Vessel vessel)
			{
				if (vessel != null) {
					// no rover, no driving vehilce, sorry
					if (vessel.vesselType != VesselType.Rover) return false;
					//
					if (vessel.situation == Vessel.Situations.LANDED) {
						var currentVesselPosition = vessel.GetWorldPos3D();
						var distance = Vector3d.Distance(currentVesselPosition, lastVesselSurfacePosition);
						if (distance > Constants.MIN_DISTANCE_FOR_MOVING_VEHICLE_ON_SURFACE) {
							if (lastVesselSurfacePosition != Vector3d.zero) return true;
							lastVesselSurfacePosition = currentVesselPosition;
						}
					} else {
						// not landed, so ignore this position
						lastVesselSurfacePosition = Vector3d.zero;
					}
				} else {
					// no vessel, so ignore this position
					lastVesselSurfacePosition = Vector3d.zero;
				}

				return false;
			}


			private void ResetInspectors()
			{
				if (Log.IsLogable(Log.LEVEL.DETAIL)) Log.Detail("reset inspectors");
				machInspector.Reset();
				altitudeInspector.Reset();
				geeForceInspector.Reset();
				atmosphereInspector.Reset();
				orbitInspector.Reset();
			}


			private void ClearInspectors()
			{
				if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("clearing inspectors");
				machInspector.Clear();
				altitudeInspector.Clear();
				geeForceInspector.Clear();
				atmosphereInspector.Clear();
				orbitInspector.Clear();
			}

			public void Update()
			{
				// game eventy occur in FLIGHT only
				if (!HighLogic.LoadedScene.Equals(GameScenes.FLIGHT)) return;

				updateCycle++;
				// test custom events every fifth update
				if (updateCycle % 5 == 0) {
					var vessel = FlightGlobals.ActiveVessel;
					if (vessel != null) {
						if (!landedVesselHasMoved && checkIfLandedVesselHasMoved(vessel)) {
							landedVesselHasMoved = true;
							OnLandedVesselMove(vessel);
						}

						geeForceInspector.Inspect(vessel);

						altitudeInspector.Inspect(vessel);
						if (altitudeInspector.StateHasChanged()) {
							if (Log.IsLogable(Log.LEVEL.DETAIL)) Log.Detail("reset of mach and altitude inspecteurs because of change in altitude");
							machInspector.Reset();
							altitudeInspector.Reset();
						}

						machInspector.Inspect(vessel);

						atmosphereInspector.Inspect(vessel);

						orbitInspector.Inspect(vessel);
					}
				}

				// test for custom events each second
				if (updateCycle % 60 == 0) {
					FireCustomEvents();
					ClearInspectors();
				}
			}
		}
	}
}