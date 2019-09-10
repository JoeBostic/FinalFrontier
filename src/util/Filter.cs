namespace Nereid
{
	namespace FinalFrontier
	{
		public interface Filter<T>
		{
			bool Accept(T x);
		}
	}
}