namespace Nereid.FinalFrontier
{
	internal class ExternalAchievement : Achievement
	{
		private readonly string description;

		public ExternalAchievement(string code, string name, int prestige, bool first, string description)
			: base(code, name, prestige, first)
		{
			this.description = description;
		}

		public override string GetDescription()
		{
			return description;
		}
	}
}