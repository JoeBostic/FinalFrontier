using System;
using UnityEngine;

namespace Nereid
{
	namespace FinalFrontier
	{
		public class Ribbon : IComparable<Ribbon>
		{
			public static readonly int WIDTH = 120;
			public static readonly int HEIGHT = 32;

			private readonly Achievement achievement;

			// ribbon that has to be superseded if any
			private readonly Ribbon supersede;

			private readonly Texture2D texture;

			// ribbon enabled?
			private readonly bool enabled = true;

			public Ribbon(string imagePath, Achievement achievement, Ribbon supersede = null)
			{
				this.achievement = achievement;
				this.supersede = supersede;
				texture = ImageLoader.GetTexture(imagePath);
				if (texture != null) {
					texture.filterMode = FilterMode.Trilinear;
				} else {
					// no texture => ribbon disabled
					Log.Warning("ribbon " + achievement.GetName() + " disabled");
					enabled = false;
				}
			}

			public int CompareTo(Ribbon right)
			{
				return achievement.CompareTo(right.achievement);
			}

			public bool IsEnabled()
			{
				return enabled;
			}

			public int GetWidth()
			{
				return WIDTH;
			}

			public int GetHeight()
			{
				return HEIGHT;
			}

			public Texture2D GetTexture()
			{
				return texture;
			}

			public string GetCode()
			{
				return achievement.GetCode();
			}

			public Achievement GetAchievement()
			{
				return achievement;
			}

			public Ribbon SupersedeRibbon()
			{
				return supersede;
			}

			public override bool Equals(object right)
			{
				if (right == null) return false;
				var cmp = right as Ribbon;
				// Ribbons are the same, if and only if the achievements are the same
				return achievement.Equals(cmp.achievement);
			}

			public override int GetHashCode()
			{
				return achievement.GetHashCode();
			}

			public string GetDescription()
			{
				if (achievement.GetDescription() == null) return ""; // this should never happen, but its better to be safe than sorry
				return achievement.GetDescription();
			}

			public string GetName()
			{
				return achievement.GetName() + " Ribbon";
			}

			public override string ToString()
			{
				return achievement.GetCode();
			}
		}
	}
}