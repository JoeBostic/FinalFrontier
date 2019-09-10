using System.Collections;
using System.Collections.Generic;

namespace Nereid
{
	namespace FinalFrontier
	{
		internal abstract class Pool<T> : IEnumerable<T>
		{
			private readonly Dictionary<string, T> map = new Dictionary<string, T>();
			private readonly List<T> pool = new List<T>();

			public IEnumerator GetEnumerator()
			{
				return pool.GetEnumerator();
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return pool.GetEnumerator();
			}

			protected abstract string CodeOf(T x);

			protected void Add(T x)
			{
				var code = CodeOf(x);
				try {
					Log.Detail("adding object " + x + " (" + code + ") to pool (" + x.GetType() + ")");
					pool.Add(x);
					map.Add(code, x);
					Log.Trace(pool.Count + " objects in pool");
				} catch {
					Log.Error("code '" + code + " not unique ' in " + GetType());
					throw;
				}
			}

			protected void Clear()
			{
				Log.Detail("emtying pool");
				pool.Clear();
				map.Clear();
			}

			public int Count()
			{
				return pool.Count;
			}

			public bool Contains(T x)
			{
				return GetElementForCode(CodeOf(x)) != null;
			}

			public T GetElementForCode(string code)
			{
				try {
					if (Log.IsLogable(Log.LEVEL.TRACE)) Log.Trace("get element for code " + code);
					return map[code];
				} catch (KeyNotFoundException) {
					Log.Detail("no element for code " + code + " found in pool (" + GetType() + ")");
					return default;
				}
			}

			public void Sort()
			{
				pool.Sort();
			}
		}
	}
}