using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour 
{
	[SerializeField] private Camera GameplayCamera;
	[SerializeField] private float FireOffset;

	private Weapon mGun;
	private float mTargetPosition;
	private float mColumnSize;
	private float mStartY;

	public Weapon Weapon { get { return mGun; } }
	public int Column { get; private set; }

	void Start() 
	{
		mColumnSize = ( GameLogic.ScreenHeight * GameplayCamera.aspect * 0.8f ) / 3;

		Vector3 position = transform.position;
		position.y = GameLogic.ScreenHeight * -0.35f;
		mStartY = position.y; 
		transform.position = position;

		// Look for the gun
		mGun = GetComponentInChildren<Weapon>();

		Column = 1;
	}

	void Update()
	{
		Vector3 position = transform.position;
		if( mTargetPosition != position.x )
		{
			position.x = Mathf.SmoothStep( position.x, mTargetPosition, GameLogic.GameDeltaTime * GameLogic.PlayerSpeed );
			transform.position = position;
		}
	}

	public void Reset()
	{
		Vector3 position = new Vector3( 0.0f, mStartY, 0.0f );
		transform.position = position;
		mTargetPosition = 0.0f;

		Column = 1;
	}

	public void Fire()
	{
		if( mGun != null )
		{
			Vector3 position = transform.position;
			position.y += FireOffset;
			mGun.Fire( position );
		}
	}

	public void MoveLeft()
	{
		if( Column >= 1 && GameLogic.GameDeltaTime > 0.0f )
		{
			mTargetPosition -= mColumnSize;
			Column--;
		}
	}

	public void MoveRight()
	{
		if( Column <= 1 && GameLogic.GameDeltaTime > 0.0f )
		{
			mTargetPosition += mColumnSize;
			Column++;
		}
	}
}
