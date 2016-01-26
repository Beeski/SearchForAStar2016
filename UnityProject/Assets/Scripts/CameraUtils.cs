using UnityEngine;
using System.Collections;

public class CameraUtils
{
	// Calculate the frustum height at a given distance from the camera.
	public static float FrustumHeightAtDistance( float distance, float fov ) 
	{
		return 2.0f * distance * Mathf.Tan( fov * 0.5f * Mathf.Deg2Rad );
	}
	
	// Calculate the FOV needed to get a given frustum height at a given distance.
	public static float FOVForHeightAndDistance( float height, float distance ) 
	{
		return 2.0f * Mathf.Atan( height * 0.5f / distance ) * Mathf.Rad2Deg;
	}
	
	// Calculate the distance for a camera with a given fov and frustum height
	public static float DistanceForFOVAndHeight( float fov, float height )
	{
		return height * 0.5f / Mathf.Tan( fov * 0.5f * Mathf.Deg2Rad );
	}
}
