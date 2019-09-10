using UnityEngine;

namespace Nereid
{
	namespace FinalFrontier
	{
		internal static class FFStyles
		{
			static FFStyles()
			{
				STYLE_SINGLE_RIBBON.fixedWidth = 120;
				STYLE_SINGLE_RIBBON.fixedHeight = 32;
				STYLE_SINGLE_RIBBON.stretchWidth = false;
				STYLE_SINGLE_RIBBON.stretchHeight = false;
				STYLE_SINGLE_RIBBON.contentOffset = Vector2.zero;
				STYLE_SINGLE_RIBBON.border = new RectOffset(0, 0, 0, 0);
				STYLE_SINGLE_RIBBON.padding = new RectOffset(0, 0, 0, 0);
				STYLE_SINGLE_RIBBON.margin = new RectOffset(2, 2, 2, 2);
				STYLE_STRETCHEDLABEL.stretchWidth = true;
				STYLE_STRETCHEDLABEL.clipping = TextClipping.Clip;
				STYLE_RLABEL.stretchWidth = true;
				STYLE_RLABEL.clipping = TextClipping.Clip;
				STYLE_RLABEL.alignment = TextAnchor.MiddleRight;
				STYLE_NARROW_BUTTON.stretchHeight = false;
				STYLE_NARROW_BUTTON.fixedHeight = 16;
				STYLE_BUTTON_LABEL.stretchWidth = true;
				STYLE_BUTTON_LABEL.clipping = TextClipping.Clip;
				STYLE_BUTTON_LABEL.font = STYLE_BUTTON.font;
				STYLE_BUTTON_LABEL.normal.textColor = STYLE_BUTTON.normal.textColor;
				STYLE_TITLE_LABEL.stretchWidth = true;
				STYLE_TITLE_LABEL.clipping = TextClipping.Clip;
				STYLE_TITLE_LABEL.font = STYLE_BUTTON.font;
				STYLE_TITLE_LABEL.alignment = TextAnchor.LowerLeft;
				STYLE_TITLE_LABEL.normal.textColor = STYLE_BUTTON.normal.textColor;
				STYLE_RIBBON_LABEL.stretchWidth = true;
				STYLE_RIBBON_LABEL.stretchHeight = false;
				STYLE_RIBBON_LABEL.fixedHeight = Ribbon.HEIGHT / 2;
				STYLE_RIBBON_LABEL.clipping = TextClipping.Clip;
				STYLE_RIBBON_LABEL.font = STYLE_BUTTON.font;
				STYLE_RIBBON_LABEL.alignment = TextAnchor.UpperLeft;
				STYLE_RIBBON_LABEL.border = new RectOffset(0, 0, 0, 8);
				STYLE_RIBBON_LABEL.margin = new RectOffset(10, 0, 0, 8);
				STYLE_RIBBON_LABEL.padding = new RectOffset(0, 0, 0, 0);
				STYLE_RIBBON_DESCRIPTION.stretchWidth = true;
				STYLE_RIBBON_DESCRIPTION.border = new RectOffset(0, 0, 0, 0);
				STYLE_RIBBON_DESCRIPTION.padding = new RectOffset(0, 0, 0, 0);
				STYLE_RIBBON_DESCRIPTION.margin = new RectOffset(3, 3, 3, 3);
				STYLE_RIBBON_DESCRIPTION.fontSize = 12;
				STYLE_RIBBON_AREA.fixedWidth = 430;
				STYLE_RIBBON_AREA.fixedHeight = 42;
				STYLE_RIBBON_AREA.padding = new RectOffset(0, 0, 0, 0);
				STYLE_RIBBON_AREA.clipping = TextClipping.Clip;
				STYLE_TOOLTIP.wordWrap = false;
				STYLE_TOOLTIP.alignment = TextAnchor.MiddleLeft;
				STYLE_TOOLTIP.stretchWidth = true;
				STYLE_TOOLTIP.normal.textColor = Color.white;
				STYLE_BG_TOOLTIP.wordWrap = false;
				STYLE_BG_TOOLTIP.alignment = TextAnchor.MiddleLeft;
				STYLE_BG_TOOLTIP.stretchWidth = true;
				STYLE_BG_TOOLTIP.normal.textColor = Color.black;
				STYLE_CONTROL_BUTTON.fixedWidth = 70;
				STYLE_CONTROL_BUTTON.stretchWidth = false;
				STYLE_RIBBON.fixedWidth = 60;
				STYLE_RIBBON.fixedHeight = 16;
				STYLE_RIBBON.border = new RectOffset(0, 0, 0, 0);
				STYLE_RIBBON.margin = new RectOffset(0, 0, 0, 0);
				STYLE_RIBBON.padding = new RectOffset(0, 0, 0, 0);
				STYLE_TEXTAREA.stretchHeight = true;
				STYLE_NARROW_TOGGLE.border = new RectOffset(0, 0, 0, 0);
				STYLE_NARROW_TOGGLE.margin = new RectOffset(0, 0, 0, 0);
				STYLE_NARROW_TOGGLE.padding = new RectOffset(0, 0, 0, 0);
				STYLE_NARROW_TOGGLE.alignment = TextAnchor.UpperLeft;
				STYLE_RIBBON_OFF5.stretchWidth = false;
				STYLE_RIBBON_OFF5.stretchHeight = false;
				STYLE_RIBBON_OFF5.fixedHeight = Ribbon.HEIGHT / 2;
				STYLE_RIBBON_OFF5.fixedWidth = Ribbon.WIDTH / 2;
				STYLE_RIBBON_OFF5.clipping = TextClipping.Clip;
				STYLE_RIBBON_OFF5.alignment = TextAnchor.UpperLeft;
				STYLE_RIBBON_OFF5.border = new RectOffset(0, 0, 0, 0);
				STYLE_RIBBON_OFF5.margin = new RectOffset(5, 5, 5, 5);
				STYLE_RIBBON_OFF5.padding = new RectOffset(0, 0, 0, 0);
				STYLE_LABEL_OFF5.stretchWidth = true;
				STYLE_LABEL_OFF5.alignment = TextAnchor.MiddleLeft;
				STYLE_LABEL_OFF5.border = new RectOffset(0, 0, 0, 0);
				STYLE_LABEL_OFF5.margin = new RectOffset(5, 5, 5, 5);
				STYLE_LABEL_OFF5.padding = new RectOffset(0, 0, 0, 0);
				STYLE_SCROLLVIEW.alignment = TextAnchor.UpperLeft;
				STYLE_STRETCHEDTEXTFIELD.stretchWidth = true;
				STYLE_TOGGLE.margin = new RectOffset(0, 120, 0, 0);
				STYLE_BUTTON_HOTYKEY.padding = new RectOffset(0, 0, 0, 0);
				//STYLE_BUTTON_HOTYKEY.border = new RectOffset(0, 0, 18, 0);
				STYLE_BUTTON_HOTYKEY.fixedWidth = 80;
				STYLE_BUTTON_HOTYKEY.alignment = TextAnchor.MiddleCenter;
				STYLE_BUTTON_HOTYKEY.stretchHeight = true;
				STYLE_BUTTON_HOTYKEY.stretchWidth = false;
			}

			public static GUIStyle STYLE_SINGLE_RIBBON { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_RIBBON { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_WINDOW { get; } = new GUIStyle(HighLogic.Skin.window);

			public static GUIStyle STYLE_TOOLTIP { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_BG_TOOLTIP { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_BUTTON { get; } = new GUIStyle(HighLogic.Skin.button);

			public static GUIStyle STYLE_NARROW_BUTTON { get; } = new GUIStyle(HighLogic.Skin.button);

			public static GUIStyle STYLE_CONTROL_BUTTON { get; } = new GUIStyle(HighLogic.Skin.button);

			public static GUIStyle STYLE_TOGGLE { get; } = new GUIStyle(HighLogic.Skin.toggle);

			public static GUIStyle STYLE_NARROW_TOGGLE { get; } = new GUIStyle(HighLogic.Skin.toggle);

			public static GUIStyle STYLE_TEXTAREA { get; } = new GUIStyle(HighLogic.Skin.textArea);

			public static GUIStyle STYLE_TEXTFIELD { get; } = new GUIStyle(HighLogic.Skin.textField);

			public static GUIStyle STYLE_STRETCHEDTEXTFIELD { get; } = new GUIStyle(HighLogic.Skin.textField);

			public static GUIStyle STYLE_STRETCHEDLABEL { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_RLABEL { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_LABEL_OFF5 { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_BUTTON_LABEL { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_TITLE_LABEL { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_RIBBON_LABEL { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_RIBBON_OFF5 { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_RIBBON_DESCRIPTION { get; } = new GUIStyle(HighLogic.Skin.label);

			public static GUIStyle STYLE_RIBBON_AREA { get; } = new GUIStyle(HighLogic.Skin.scrollView);

			public static GUIStyle STYLE_SCROLLVIEW { get; } = new GUIStyle(HighLogic.Skin.scrollView);

			public static GUIStyle STYLE_BUTTON_HOTYKEY { get; } = new GUIStyle(HighLogic.Skin.textField);
		}
	}
}