using UnityEngine;

namespace Nereid
{
	namespace FinalFrontier
	{
		internal static class ImageLoader
		{
			public static Texture2D GetTexture(string pathInGameData)
			{
				Log.Detail("get texture " + pathInGameData);
				var texture = GameDatabase.Instance.GetTexture(pathInGameData, false);
				if (texture != null) {
					return texture;
				}

				Log.Error("texture " + pathInGameData + " not found");
				return null;
			}
		}
	}
}