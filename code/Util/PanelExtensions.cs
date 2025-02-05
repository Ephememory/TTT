﻿using Sandbox;
using Sandbox.UI;

namespace TTT;

public static class PanelExtensions
{
	public static void Enabled( this Panel panel, bool enabled )
	{
		panel.Style.Display = enabled ? DisplayMode.Flex : DisplayMode.None;
	}

	public static void EnableFade( this Panel panel, bool enabled )
	{
		panel.SetClass( "fade-in", enabled );
		panel.SetClass( "fade-out", !enabled );
	}

	public static bool IsEnabled( this Panel panel )
	{
		return panel.Style.Display != DisplayMode.None;
	}

	public static void SetTexture( this Image image, Texture texture )
	{
		image.Style.BackgroundImage = texture ?? Texture.Load( FileSystem.Mounted, "/ui/none.png" );
	}

	public static void SetImage( this Image image, string imagePath )
	{
		image.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, imagePath, false ) ?? Texture.Load( FileSystem.Mounted, "/ui/none.png" );
	}
}
