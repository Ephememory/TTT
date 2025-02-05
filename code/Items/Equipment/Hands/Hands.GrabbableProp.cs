using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public class GrabbableProp : IGrabbable
{
	private ModelEntity GrabbedEntity { get; set; }
	public string PrimaryAttackHint => GrabbedEntity.IsValid() ? "Throw" : string.Empty;
	public string SecondaryAttackHint => GrabbedEntity.IsValid() ? "Drop" : string.Empty;
	public bool IsHolding => GrabbedEntity is not null || _isThrowing;

	private readonly Player _owner;
	private bool _isThrowing = false;
	private readonly bool _hasTriggerTag = false;

	public GrabbableProp( Player owner, Entity grabPoint, ModelEntity grabbedEntity )
	{
		_owner = owner;

		// We want to be able to shoot whatever entity the player is holding.
		// Let's give it a tag that interacts with bullets and doesn't collide with players.
		_hasTriggerTag = grabbedEntity.Tags.Has( "trigger" );
		if ( !_hasTriggerTag )
			grabbedEntity.Tags.Add( "trigger" );

		GrabbedEntity = grabbedEntity;
		GrabbedEntity.EnableTouch = false;
		GrabbedEntity.EnableHideInFirstPerson = false;
		GrabbedEntity.SetParent( grabPoint, Hands.MiddleHandsAttachment, new Transform( Vector3.Zero ) );
	}

	public void Update( Player player )
	{
		// Incase someone walks up and picks up the carriable from the player's hands
		// we just need to reset "EnableHideInFirstPerson", all other parenting is handled on pickup.
		var carriableHasOwner = GrabbedEntity is Carriable && GrabbedEntity.Owner.IsValid();
		if ( carriableHasOwner )
		{
			GrabbedEntity.EnableHideInFirstPerson = true;
			GrabbedEntity = null;
		}

		if ( !GrabbedEntity.IsValid() || !_owner.IsValid() )
			Drop();
	}

	public Entity Drop()
	{
		var grabbedEntity = GrabbedEntity;
		if ( grabbedEntity.IsValid() )
		{
			if ( !_hasTriggerTag )
				grabbedEntity.Tags.Remove( "trigger" );

			grabbedEntity.EnableHideInFirstPerson = true;
			grabbedEntity.EnableTouch = true;
			grabbedEntity.SetParent( null );

			if ( grabbedEntity is Carriable carriable )
				carriable.OnCarryDrop( _owner );
		}

		GrabbedEntity = null;
		return grabbedEntity;
	}

	public void SecondaryAction()
	{
		if ( Host.IsClient )
		{
			GrabbedEntity = null;
			return;
		}

		_isThrowing = true;
		_owner.SetAnimParameter( "b_attack", true );

		var droppedEntity = Drop();
		if ( droppedEntity.IsValid() )
			droppedEntity.Velocity = _owner.Velocity + _owner.EyeRotation.Forward * Player.DropVelocity;

		_ = WaitForAnimationFinish();
	}

	private async Task WaitForAnimationFinish()
	{
		await GameTask.DelaySeconds( 0.6f );
		_isThrowing = false;
	}
}
