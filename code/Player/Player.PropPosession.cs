using Sandbox;

namespace TTT;

public class PosessedComponent : EntityComponent { }

public partial class Player
{
	[Net]
	public Prop PosessedProp { get; set; }

	[ConCmd.Server]
	public static void PosessProp( Vector3 pos, Rotation look )
	{
		var cl = ConsoleSystem.Caller;

		if ( !cl.Pawn.IsValid() || cl.Pawn is not Player ply )
			return;

		// spectators only
		if ( ply.LifeState != LifeState.Dead )
			return;

		var tr = Trace.Ray( pos, pos + look.Forward * 100 ).Size( Vector3.One * 1.2f ).WithAnyTags( "prop" ).Run();
		DebugOverlay.Line( pos, pos + look.Forward * 100, 5, false );

		if ( !tr.Hit || tr.Entity is not Prop prop )
			return;

		if ( prop.Static )
			return;

		// already being posessed
		if ( prop.Components.TryGet<PosessedComponent>( out var _ ) )
			return;

		// maybe need to verify it was added successfully?
		prop.Components.Add( new PosessedComponent() );
		ply.PosessedProp = prop;
		ply.Camera = new PropPossessorCamera();

	}
}
