using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nereid
{
	namespace FinalFrontier
	{
		// this class is not used yet
		public class RibbonSupersedeChain
		{
			private static readonly string ROOT_PATH = Utils.GetRootPath();
			private static readonly string PATH = ROOT_PATH + "/GameData/Nereid/FinalFrontier";
			private static readonly string FILE_NAME = "supersede.cfg";

			private readonly Dictionary<Ribbon, Ribbon> chain = new Dictionary<Ribbon, Ribbon>();

			public void Load()
			{
				Load(PATH + "/" + FILE_NAME);
			}

			public void Load(string filename)
			{
				StreamReader file = null;
				try {
					file = File.OpenText(filename);
					string line;
					while ((line = file.ReadLine()) != null) {
						var tokens = line.Split(' ');
						if (tokens.Count() > 1) {
							var code = tokens[0];
							var supersede = tokens[1];
							var ribbon = RibbonPool.Instance().GetRibbonForCode(code);
							if (ribbon == null) continue;
							var super = RibbonPool.Instance().GetRibbonForCode(supersede);
							if (super == null) continue;
							chain.Add(ribbon, super);
						}
					}
				} finally {
					if (file != null) file.Close();
				}
			}
		}
	}
}