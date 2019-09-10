using System.Collections.Generic;

namespace Nereid
{
	namespace FinalFrontier
	{
		public interface Sorter<T>
		{
			void Sort(List<T> list);
		}
	}
}