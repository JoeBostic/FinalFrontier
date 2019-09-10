using System.Collections.Generic;

namespace Nereid
{
	namespace FinalFrontier
	{
		internal class WindowManager
		{
			public static readonly WindowManager instance = new WindowManager();


			private readonly List<AbstractWindow> windows = new List<AbstractWindow>();

			public void AddWindow(AbstractWindow window)
			{
				windows.Add(window);
			}


			public void RemoveWindow(AbstractWindow window)
			{
				windows.Remove(window);
			}

			public void OnGUI()
			{
				foreach (var window in windows)
					if (window.IsVisible())
						window.OnGUI();
			}

			public void CloseAll()
			{
				foreach (var window in windows)
					if (window.IsVisible())
						window.SetVisible(false);
			}
		}
	}
}