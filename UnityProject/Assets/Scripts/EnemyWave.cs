using System;

[Serializable]
public class EnemyWave
{
	private int mEnemiesPerRow;
	private int mRows;

	public int EnemiesPerRow { get { return mEnemiesPerRow; } }
	public int NumberOfRows { get { return mRows; } }

	public EnemyWave( int numberOfEnemiesPerRow, int numberOfRows )
	{
		mEnemiesPerRow = numberOfEnemiesPerRow;
		mRows = numberOfRows;
	}
}
