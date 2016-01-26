using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundScenery : MonoBehaviour 
{
	[SerializeField] private Material SceneryMaterial; 
	[SerializeField] private float SceneryMinScale = 0.25f; 
	[SerializeField] private float SceneryMaxScale = 0.75f; 
	[Range( 1, 1000 )]
	[SerializeField] private int SceneryPoolSize = 100; 
	
	private GameObject [] mPool;
	
	void Start()
	{
		// Create the scenery and position
		mPool = new GameObject[SceneryPoolSize];
		for( int count = 0; count < SceneryPoolSize; count++ )
		{
			GameObject sceneryItem = new GameObject( "Scenery_PoolID" + ( count + 1 ) );
			CreateMesh m = sceneryItem.AddComponent<CreateMesh>();
			m.Material = SceneryMaterial;
			float x = Random.Range( -GameLogic.ScreenBounds, GameLogic.ScreenBounds );
			float y = Random.Range( GameLogic.ScreenHeight * -0.5f, GameLogic.ScreenHeight * 0.5f );
			float scale = Random.Range( SceneryMinScale, SceneryMaxScale );
			sceneryItem.transform.position = new Vector3( x, y, 0.0f );
			sceneryItem.transform.localScale = new Vector3( scale, scale, scale );
			sceneryItem.transform.localRotation = Quaternion.AngleAxis( 180.0f, Vector3.forward );
			sceneryItem.transform.parent = transform;
			mPool[count] = sceneryItem;
		}
	}
	
	void Update()
	{
		// Update the position of each active sceneryItem, keep a track of scenery which have gone off screen 
		for( int count = 0; count < mPool.Length; count++ )
		{
			Vector3 position = mPool[count].transform.position;
			float scale = mPool[count].transform.localScale.x;
			position.y -= GameLogic.GameDeltaTime * GameLogic.GameSpeed * scale;

			if( position.y < GameLogic.ScreenHeight * -0.5f )
			{
				position.y = GameLogic.ScreenHeight * 0.5f;
			}

			mPool[count].transform.position = position;
		}
	}
}
