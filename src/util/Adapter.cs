using System;
using System.Reflection;

namespace Nereid
{
	namespace FinalFrontier
	{
		public abstract class Adapter
		{
			private bool isInstalled;

			public bool IsInstalled()
			{
				return isInstalled;
			}

			protected void SetInstalled(bool installed)
			{
				isInstalled = installed;
			}

			protected Type GetType(string name)
			{
				Type type = null;
				AssemblyLoader.loadedAssemblies.TypeOperation(t => {
					if (t.FullName == name) type = t;
				});
				return type;
			}

			protected bool IsTypeLoaded(string name)
			{
				return GetType(name) != null;
			}

			public abstract void Plugin();
		}

		public class FARAdapter : Adapter
		{
			private MethodInfo methodGetMachNumber;

			public override void Plugin()
			{
				try {
					var type = GetType("ferram4.FARAeroUtil");
					if (type != null) {
						methodGetMachNumber = type.GetMethod("GetMachNumber", BindingFlags.Public | BindingFlags.Static, null, new[] {typeof(CelestialBody), typeof(double), typeof(Vector3d)}, null);
						if (methodGetMachNumber != null) SetInstalled(true);
					}
				} catch (Exception e) {
					Log.Error("plugin of F.A.R failed; exception: " + e.GetType() + " - " + e.Message);
					SetInstalled(false);
				}
			}

			public double GetMachNumber(CelestialBody body, double altitude, Vector3d velocity)
			{
				if (IsInstalled()) return (double) methodGetMachNumber.Invoke(null, new object[] {body, altitude, velocity});
				return 0.0;
			}
		}
	}
}