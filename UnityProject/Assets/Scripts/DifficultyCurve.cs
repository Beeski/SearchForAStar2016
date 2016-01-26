using UnityEngine;
using System;
using System.Collections;

public class DifficultyCurve : MonoBehaviour 
{
	[SerializeField] private float GameStartSpeed = 10f;
	[SerializeField] private float GameSpeedRamp = 0.1f;
	[SerializeField] private float PlayerStartSpeed = 20f;
	[SerializeField] private float PlayerSpeedRamp = 0.1f;
	[SerializeField] private float BulletStartSpeed = 20f;
	[SerializeField] private float BulletSpeedRamp = 0.1f;
	[SerializeField] private float TimeBetweenRows = 5.0f;
	[SerializeField] private float TimeBetweenWaves = 40.0f;

	private EnemyWave[] mWaves;
	private float mTimeToNextRow;
	private float mTimeToNextWave;
	private int mCurrentRow;
	private int mCurrentWave;

	public static float GameSpeed { get; private set; }
	public static float PlayerSpeed { get; private set; }
	public static float BulletSpeed { get; private set; }

	void Awake()
	{
		Reset();

		EnemyWave [] waves = { 
			new EnemyWave( 1, 1 ), 
			new EnemyWave( 1, 2 ), 
			new EnemyWave( 1, 3 ), 
			new EnemyWave( 1, 4 ), 
			new EnemyWave( 2, 3 ), 
			new EnemyWave( 2, 4 ), 
			new EnemyWave( 2, 5 ), 
			new EnemyWave( 3, 3 ), 
			new EnemyWave( 3, 4 ), 
			new EnemyWave( 3, 5 ), 
			new EnemyWave( 3, 6 ), 
			new EnemyWave( 3, 8 ) 
		};

		mWaves = waves;
	}

	void Start()
	{
		GameSpeed = GameStartSpeed;
		PlayerSpeed = PlayerStartSpeed;
		BulletSpeed = BulletStartSpeed;
	}

	public int SpawnCount()
	{
		int enemiesToSpawn = 0;

		if( mCurrentRow < mWaves[mCurrentWave].NumberOfRows )
		{
			mTimeToNextRow -= GameLogic.GameDeltaTime;
			if( mTimeToNextRow <= 0.0f )
			{
				mCurrentRow++;
				enemiesToSpawn = mWaves[mCurrentWave].EnemiesPerRow;
				mTimeToNextRow = TimeBetweenRows;
			}
		}
		else
		{
			mTimeToNextWave -= GameLogic.GameDeltaTime;
			if( mTimeToNextWave <= 0.0f )
			{
				if( ( mCurrentWave + 1 ) < mWaves.Length )
				{
					GameSpeed += GameSpeedRamp;
					PlayerSpeed += PlayerSpeedRamp;
					BulletSpeed += BulletSpeedRamp;
					mCurrentWave++;
				}
				mTimeToNextWave = TimeBetweenWaves;
				mCurrentRow = 0;
			}
		}

		return enemiesToSpawn;
	}

	public void Stop()
	{
		GameSpeed = 0.0f;
		PlayerSpeed = 0.0f;
		BulletSpeed = 0.0f;
	}

	public void Reset()
	{
		GameSpeed = GameStartSpeed;
		PlayerSpeed = PlayerStartSpeed;
		BulletSpeed = BulletStartSpeed;
		mTimeToNextRow = TimeBetweenRows;
		mTimeToNextWave = TimeBetweenWaves;
		mCurrentRow = 0;
		mCurrentWave = 0;
	}
}
