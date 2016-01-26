using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFactory : MonoBehaviour 
{
    public enum Column { One, Two, Three, NumColumns }

	private static EnemyFactory mInstance; 

	[SerializeField] private Camera GameplayCamera;
	[SerializeField] private Material EnemyMaterial; 
	[SerializeField] private float EnemyScale = 1.5f; 
	[Range( 1, 100 )]
	[SerializeField] private int EnemyPoolSize = 10; 
	
	private GameObject [] mPool;
	private List<GameObject> mActive;
	private List<GameObject> mInactive;
	private float mColumnWidth;
	
	void Awake()
	{
		if( mInstance == null )
		{
			mInstance = this;

            // Work out the width of each column
			mColumnWidth = ( GameLogic.ScreenHeight * GameplayCamera.aspect * 0.8f ) / (int)Column.NumColumns;

			// Create the enemies, initialise the active and available lists, put all enemies in the available list
			mActive = new List<GameObject>();
			mInactive = new List<GameObject>();
			mPool = new GameObject[EnemyPoolSize];
            for (int count = 0; count < mPool.Length; count++)
			{
				GameObject enemy = new GameObject( "Enemy_PoolID" + ( count + 1 ) );
				CreateMesh m = enemy.AddComponent<CreateMesh>();
				m.Material = EnemyMaterial;
				enemy.transform.localScale = new Vector3( EnemyScale, EnemyScale, EnemyScale );
				enemy.transform.localRotation = Quaternion.AngleAxis( 180.0f, Vector3.forward );
				enemy.transform.parent = transform;
                mPool[count] = enemy;
				mInactive.Add( enemy );
				enemy.SetActive( false );
			}
		}
		else
		{
			Debug.LogError( "Only one EnemyFactory allowed - destorying duplicate" );
			Destroy( this.gameObject );
		}
	}

	public static GameObject Dispatch( Column column )
	{
		if( mInstance != null )
		{
			return mInstance.DoDispatch( column );
		}
		return null;
	}

	public static bool Return( GameObject enemy )
	{
		if( mInstance != null )
		{
			if( mInstance.mActive.Remove( enemy ) )
			{
				enemy.SetActive( false );
				mInstance.mInactive.Add( enemy ); 
			}
		}
		return false;
	}

	public static void Reset()
	{
		if( mInstance != null )
		{
			for( int count = 0; count < mInstance.mPool.Length; count++ )
			{
                mInstance.mPool[count].SetActive(false);
                mInstance.mInactive.Add(mInstance.mPool[count]); 
			}

			mInstance.mActive.Clear();
		}
	}

	private GameObject DoDispatch( Column column )
	{
		// Look for a free enemy and then dispatch them 
		GameObject result = null;
		if( mInactive.Count > 0 )
		{
			GameObject enemy = mInactive[0];
			Vector3 position = enemy.transform.position;
			position.x = -mColumnWidth + ( mColumnWidth * (float)column ); 
			position.y = GameLogic.ScreenHeight * 0.5f;
			position.z = 0.0f;
			enemy.transform.position = position;
			enemy.SetActive( true );
			mActive.Add( enemy );
			mInactive.Remove( enemy );
			result = enemy;
		}
		
		// Returns true if a free enemy was found and dispatched
		return result;
	}
}
