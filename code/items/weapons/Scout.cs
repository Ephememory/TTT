using Sandbox;
using TTT.UI;

namespace TTT;

[Hammer.EditorModel( "models/weapons/w_spr.vmdl" )]
[Library( "ttt_weapon_scout", Title = "Scout" )]
public partial class Scout : Weapon
{
	private float _defaultFOV;
	private bool _isScoped;
	private Scope _sniperScopePanel;

	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed >= Info.DeployTime && Input.Pressed( InputButton.Attack2 ) )
		{
			if ( _isScoped )
				OnScopeEnd( Owner );
			else
				OnScopeStart( Owner );
		}

		base.Simulate( owner );
	}

	public override void CreateHudElements()
	{
		if ( Hud.GeneralHud.Instance == null )
			return;

		_sniperScopePanel = new Scope( "/ui/scout_scope.png" )
		{
			Parent = Hud.GeneralHud.Instance
		};
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		_defaultFOV = Owner.CameraMode.FieldOfView;
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		if ( ent is Player player )
			OnScopeEnd( player );

		_sniperScopePanel?.Delete();

		base.ActiveEnd( ent, dropped );
	}

	private void OnScopeStart( Player player )
	{
		player.CameraMode.FieldOfView = 10f;

		if ( IsServer )
			return;

		ViewModelEntity.EnableDrawing = false;
		HandsModelEntity.EnableDrawing = false;
		_isScoped = true;
		_sniperScopePanel.Show();
	}

	private void OnScopeEnd( Player player )
	{
		player.CameraMode.FieldOfView = _defaultFOV;

		if ( IsServer )
			return;

		ViewModelEntity.EnableDrawing = true;
		HandsModelEntity.EnableDrawing = true;
		_isScoped = false;
		_sniperScopePanel.Hide();
	}

	[Event.BuildInput]
	private new void BuildInput( InputBuilder builder )
	{
		if ( _isScoped )
			builder.AnalogLook *= 0.2f;
	}
}
