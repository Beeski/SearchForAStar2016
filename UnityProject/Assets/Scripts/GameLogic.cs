using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour 
{
	[SerializeField] private TextMesh GameText;
	[SerializeField] private Camera GameplayCamera;
	[SerializeField] private float PlayerKillDistance = 10.0f;  
	[SerializeField] private float BulletKillDistance = 10.0f;
    [SerializeField] private float WaitTime = 1.0f;
	[SerializeField] private int MaxMissedEnemies = 3; 

	private enum State { TapToStart, Game, GameOver };

	private List<GameObject> mActiveEnemies;
	private DifficultyCurve mCurrentDifficulty;
	private PlayerCharacter mPlayerCharacter;
    private float mGameOverTime;
    private float mDistanceTravelled;
	private int mMissedEnemies;
	private State mGameStatus;

	public static float GameDeltaTime { get; private set; }
	public static float GameSpeed { get { return DifficultyCurve.GameSpeed; } }
	public static float PlayerSpeed { get { return DifficultyCurve.PlayerSpeed; } }
	public static float BulletSpeed { get { return DifficultyCurve.BulletSpeed; } }
	public static float ScreenBounds { get; private set; }
	public static float ScreenHeight { get; private set; }
	public static bool Paused { get; private set; }

	void Awake()
	{
		float distance = transform.position.z - GameplayCamera.transform.position.z;
		ScreenHeight = CameraUtils.FrustumHeightAtDistance( distance, GameplayCamera.fieldOfView );
		ScreenBounds = ScreenHeight * GameplayCamera.aspect * 0.5f;

		GameInput.OnTap += HandleOnTap;
		GameInput.OnSwipe += HandleOnSwipe;
		mActiveEnemies = new List<GameObject>();
		mCurrentDifficulty = GetComponentInChildren<DifficultyCurve>();
		mPlayerCharacter = GetComponentInChildren<PlayerCharacter>();
		mGameStatus = State.TapToStart;
        mGameOverTime = Time.timeSinceLevelLoad;
		mMissedEnemies = 0;
		Paused = false;
	}

	void Update()
	{
		GameDeltaTime = Paused ? 0.0f : Time.deltaTime;

		if( mGameStatus == State.Game )
		{
			mDistanceTravelled += GameSpeed * GameDeltaTime;
			GameText.text = string.Format( "Distance: {0:0.0} m", mDistanceTravelled );

			int enemies = mCurrentDifficulty.SpawnCount();
			if( enemies == 1 ) 
			{
                mActiveEnemies.Add(EnemyFactory.Dispatch((EnemyFactory.Column)Random.Range(0, 3)));
			}
			else if( enemies == 2 )
			{
				int config = Random.Range( 0, 3 );
				if( config == 0 )
				{
                    mActiveEnemies.Add(EnemyFactory.Dispatch(EnemyFactory.Column.One));
                    mActiveEnemies.Add(EnemyFactory.Dispatch(EnemyFactory.Column.Two));
				}
				else if( config == 1 )
				{
                    mActiveEnemies.Add(EnemyFactory.Dispatch(EnemyFactory.Column.One));
                    mActiveEnemies.Add(EnemyFactory.Dispatch(EnemyFactory.Column.Three));
				}
				else 
				{
                    mActiveEnemies.Add(EnemyFactory.Dispatch(EnemyFactory.Column.Two));
                    mActiveEnemies.Add(EnemyFactory.Dispatch(EnemyFactory.Column.Three));
				}
			}
			else if( enemies == 3 )
			{
                mActiveEnemies.Add(EnemyFactory.Dispatch(EnemyFactory.Column.One));
                mActiveEnemies.Add(EnemyFactory.Dispatch(EnemyFactory.Column.Two));
                mActiveEnemies.Add(EnemyFactory.Dispatch(EnemyFactory.Column.Three));
			}

			// Update the position of each active enemy, keep a track of enemies which have gone off screen 
			List<GameObject> oldEnemys = new List<GameObject>(); 
			for( int count = 0; count < mActiveEnemies.Count; count++ )
			{
				Vector3 position = mActiveEnemies[count].transform.position;
				position.y -= GameDeltaTime * GameSpeed;
				mActiveEnemies[count].transform.position = position;
				if( position.y < ScreenHeight * -0.5f )
				{
					EnemyFactory.Return( mActiveEnemies[count] );
					oldEnemys.Add( mActiveEnemies[count] ); 
					mMissedEnemies++;
				}
				else
				{
					Vector3 diff = mPlayerCharacter.transform.position - mActiveEnemies[count].transform.position;
					if( diff.sqrMagnitude < PlayerKillDistance )
					{
						// Touched enemny - Game over
						mCurrentDifficulty.Stop();
                        mGameOverTime = Time.timeSinceLevelLoad;
						mGameStatus = State.GameOver;
						GameText.text = string.Format( "You Dead!\nTotal Distance: {0:0.0} m", mDistanceTravelled );
					}
					else
					{
						for( int bullet = 0; bullet < mPlayerCharacter.Weapon.ActiveBullets.Count; bullet++ )
						{
							if( mPlayerCharacter.Weapon.ActiveBullets[bullet].activeInHierarchy )
							{
								Vector3 diffToBullet = mActiveEnemies[count].transform.position - mPlayerCharacter.Weapon.ActiveBullets[bullet].transform.position;
								if( diffToBullet.sqrMagnitude < BulletKillDistance )
								{
									EnemyFactory.Return( mActiveEnemies[count] );
									oldEnemys.Add( mActiveEnemies[count] ); 
									mPlayerCharacter.Weapon.ActiveBullets[bullet].SetActive( false );
									break;
								}
							}
						}
					}
				}
			}

			if( mMissedEnemies >= MaxMissedEnemies )
			{
                // Too many missed enemies - Game over
				mCurrentDifficulty.Stop();
                mGameOverTime = Time.timeSinceLevelLoad;
                mGameStatus = State.GameOver;
				GameText.text = string.Format( "You Been Invaded!\nTotal Distance: {0:0.0} m", mDistanceTravelled );
			}

			for( int count = 0; count < oldEnemys.Count; count++ )
			{
				mActiveEnemies.Remove( oldEnemys[count] );
			}
		}
	}

	private void Reset()
	{
		mPlayerCharacter.Reset();
		mCurrentDifficulty.Reset();
		EnemyFactory.Reset();
		mActiveEnemies.Clear();
		mMissedEnemies = 0;
		mDistanceTravelled = 0.0f;
	}

	private void HandleOnTap( Vector3 position )
	{
		switch( mGameStatus )
		{
		case State.TapToStart:
			Paused = false;
			mGameStatus = State.Game;
			break;
		case State.Game:
			mPlayerCharacter.Fire();
			break;
		case State.GameOver:
            if (Time.timeSinceLevelLoad - mGameOverTime > WaitTime)
            { 
			    Reset();
			    GameText.text = "Tap to Start";
			    mGameStatus = State.TapToStart;
			}
            break;
		}
	}

	
	private void HandleOnSwipe( GameInput.Direction direction )
	{
		if( mGameStatus == State.Game )
		{
			switch( direction )
			{
			case GameInput.Direction.Left:
				mPlayerCharacter.MoveLeft();
				break;
			case GameInput.Direction.Right:
				mPlayerCharacter.MoveRight();
				break;
			}
		}
	}
}
