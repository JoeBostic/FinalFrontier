namespace Nereid
{
	namespace FinalFrontier
	{
		public abstract class Activity
		{
			private static readonly ActivityPool ACTIVITY_POOL = ActivityPool.Instance();

			private readonly string code;
			private string name;

			public Activity(string code, string name)
			{
				this.code = code;
				this.name = name;
				ACTIVITY_POOL.RegisterActivity(this);
			}

			// name of the activity
			public virtual string GetName()
			{
				return name;
			}

			// unique code of the activity
			public string GetCode()
			{
				return code;
			}

			public override bool Equals(object right)
			{
				if (right == null) return false;
				var cmp = right as Activity;
				if (cmp == null) return false;
				return GetCode().Equals(cmp.code);
			}

			public override int GetHashCode()
			{
				return code.GetHashCode();
			}

			public abstract string CreateLogBookEntry(LogbookEntry entry);

			public override string ToString()
			{
				return code;
			}

			protected void Rename(string name)
			{
				this.name = name;
			}
		}
	}
}