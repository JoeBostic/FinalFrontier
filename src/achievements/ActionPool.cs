using System.Collections.Generic;

namespace Nereid
{
	namespace FinalFrontier
	{
		internal class ActionPool : Pool<Action>
		{
			public static readonly Action ACTION_LAUNCH = new LaunchAction();
			public static readonly Action ACTION_DOCKING = new DockingAction();
			public static readonly Action ACTION_RECOVER = new RecoverAction();
			public static readonly Action ACTION_BOARDING = new BoardingAction();
			public static readonly EvaAction ACTION_EVA_NOATM = new EvaNoAtmosphereAction();
			public static readonly EvaAction ACTION_EVA_OXYGEN = new EvaWithOxygen();
			public static readonly EvaAction ACTION_EVA_INATM = new EvaInAtmosphereAction();
			public static readonly Action ACTION_CONTRACT = new ContractAction();
			public static readonly Action ACTION_SCIENCE = new ScienceAction();

			private static volatile ActionPool instance;

			private readonly List<Action> actions = new List<Action>();
			private readonly Dictionary<string, Action> codeMap = new Dictionary<string, Action>();

			private ActionPool()
			{
				Add(ACTION_LAUNCH);
				Add(ACTION_DOCKING);
				Add(ACTION_RECOVER);
				Add(ACTION_BOARDING);
				Add(ACTION_EVA_NOATM);
				Add(ACTION_EVA_OXYGEN);
				Add(ACTION_EVA_INATM);
				Add(ACTION_CONTRACT);
				Add(ACTION_SCIENCE);
			}

			//
			public static ActionPool Instance()
			{
				if (instance == null) {
					instance = new ActionPool();
					Log.Info("new action pool instance created");
				}

				return instance;
			}


			protected override string CodeOf(Action x)
			{
				return x.GetCode();
			}

			public Action GetActionForCode(string code)
			{
				return GetElementForCode(code);
			}
		}
	}
}