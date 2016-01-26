using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour 
{
	[SerializeField] private Material BulletMaterial; 
	[SerializeField] private float BulletScale = 0.5f; 
	[SerializeField] private float RechargeTime = 0.25f; 
	[Range( 1, 100 )]
	[SerializeField] private int BulletPoolSize = 10; 

	private GameObject [] mPool;
	private List<GameObject> mActive;
	private List<GameObject> mInactive;
	private float mCharging;

	public List<GameObject> ActiveBullets { get { return mActive; } }

	void Awake()
	{
		// Create the bullets, initialise the active and available lists, put all bullets in the available list
		mActive = new List<GameObject>();
		mInactive = new List<GameObject>();
		mPool = new GameObject[BulletPoolSize];
        for (int count = 0; count < mPool.Length; count++)
		{
			GameObject bullet = new GameObject( "Bullet_PoolID" + ( count + 1 ) );
			CreateMesh m = bullet.AddComponent<CreateMesh>();
			m.Material = BulletMaterial;
			bullet.transform.localScale = new Vector3( BulletScale, BulletScale, BulletScale );
			bullet.transform.parent = transform;
            mPool[count] = bullet;
			mInactive.Add( bullet );
			bullet.SetActive( false );
		}
		mCharging = 0.0f;
	}

	void Update()
	{
		// Update the position of each active bullet, keep a track of bullets which have gone off screen 
		List<GameObject> oldBullets = new List<GameObject>(); 
		for( int count = 0; count < mActive.Count; count++ )
		{
			Vector3 position = mActive[count].transform.position;
			position.y += GameLogic.GameDeltaTime * GameLogic.BulletSpeed;
			mActive[count].transform.position = position;
			if( position.y > GameLogic.ScreenHeight * 0.5f )
			{
				mActive[count].SetActive( false );
				oldBullets.Add( mActive[count] ); 
			}
		}

		// Remove the bullets which have gone off screen, return them to the available list
		for( int count = 0; count < oldBullets.Count; count++ )
		{
            oldBullets[count].transform.parent = transform;
            mActive.Remove(oldBullets[count]);
			mInactive.Add( oldBullets[count] ); 
		}

		if( mCharging > 0.0f )
		{
			mCharging -= GameLogic.GameDeltaTime;
		}
	}

	public bool Fire( Vector3 position )
	{
		// Look for a free bullet and then fire it from the player position
		bool result = false;
		if( mInactive.Count > 0 && mCharging <= 0.0f )
		{
			GameObject bullet = mInactive[0];
            bullet.transform.parent = null;
            bullet.transform.position = position;
			bullet.SetActive( true );
			mActive.Add( bullet );
			mInactive.Remove( bullet );
			mCharging = RechargeTime;
			result = true;
		}

		// Returns true if a free bullet was found and fired
		return result;
	}
}
