namespace Nereid
{
	namespace FinalFrontier
	{
		internal class ActivityPool : Pool<Activity>
		{
			private static volatile ActivityPool instance;

			private static readonly object _lock = new object();

			private ActivityPool()
			{
			}

			public static ActivityPool Instance()
			{
				lock (_lock) {
					if (instance == null) {
						instance = new ActivityPool();
						Log.Info("new activity pool instance created");
					}

					return instance;
				}
			}

			protected override string CodeOf(Activity x)
			{
				return x.GetCode();
			}

			public void RegisterActivity(Activity activity)
			{
				lock (_lock) {
					Log.Detail("registering activity " + activity.GetCode() + ": " + activity.GetName());
					Add(activity);
				}
			}
		}
	}
}