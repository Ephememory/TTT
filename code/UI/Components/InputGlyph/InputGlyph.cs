﻿using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public class InputGlyph : Panel
{
	private InputButton _inputButton;
	private InputGlyphSize _inputGlyphSize;
	private GlyphStyle _glyphStyle;

	public InputGlyph()
	{
		StyleSheet.Load( "/UI/Components/InputGlyph/InputGlyph.scss" );
	}

	public void SetButton( InputButton inputButton )
	{
		if ( _inputButton == inputButton )
			return;

		_inputButton = inputButton;
		Update();
	}

	public override void SetProperty( string name, string value )
	{
		switch ( name )
		{
			case "button":
			{
				InputButton.TryParse( value, true, out _inputButton );
				SetButton( _inputButton );
				Update();

				break;
			}
			case "size":
			{
				InputGlyphSize.TryParse( value, true, out _inputGlyphSize );
				Update();

				break;
			}
			case "style":
			{
				_glyphStyle = value switch
				{
					"knockout" => GlyphStyle.Knockout,
					"light" => GlyphStyle.Light,
					"dark" => GlyphStyle.Dark,
					_ => _glyphStyle
				};

				Update();
				break;
			}
		}

		base.SetProperty( name, value );
	}

	private void Update()
	{
		var texture = Input.GetGlyph( _inputButton, _inputGlyphSize, _glyphStyle );
		Style.BackgroundImage = texture;
		Style.Width = texture.Width;
		Style.Height = texture.Height;
	}
}
