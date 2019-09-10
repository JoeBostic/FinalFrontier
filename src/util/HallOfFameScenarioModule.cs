namespace Nereid
{
	namespace FinalFrontier
	{
		[KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.FLIGHT, GameScenes.TRACKSTATION, GameScenes.SPACECENTER)]
		internal class HallOfFameScenarioModule : ScenarioModule
		{
			public override void OnSave(ConfigNode node)
			{
				Log.Info("saving hall of fame scenario module (at " + Planetarium.GetUniversalTime() + ")");
				HallOfFame.Instance().Save(node);
			}

			public override void OnLoad(ConfigNode node)
			{
				Log.Info("loading hall of fame scenario module");
				HallOfFame.Instance().Load(node);
			}
		}
	}
}