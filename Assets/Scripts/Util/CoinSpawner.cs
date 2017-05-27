using System.Collections;

/// <summary>
/// Determines if spawning a coin is necessary
/// and determines in which index it should be placed
/// </summary>
public static class CoinSpawner{
	// Keeps track of the number of coins to create
	private static int coin_count = -1;
	private static int coins_this_chunk;

	// The minimum and maximum starting coin index in the chunk
	private const int MIN_START = 0;
	private const int MAX_START = 10;

	// Keeps track of the indexes used
	private static int start_index;
	private static int cur_index;

	// Keeps track of the number of coins before an index change
	private const float SWITCH_CHANCE = 0.5f;
	private static int consecutive_coins;
	private static int min_consecutive;
	private static int max_consecutive;

	#region privatevars
    // Returns TimeWarp vars
    public static int CoinsThisChunk {
        get {return coins_this_chunk;}
    }
    #endregion

	public static int CoinIndex(ArrayList openIndicies){
		if (coin_count == -1){
			init(openIndicies);
		}

		if(consecutive_coins >= max_consecutive){
			cur_index = switchIndex(openIndicies);
			consecutive_coins = 0;
		}
		else if(consecutive_coins >= min_consecutive && coin_count % 5 == 0){
			if(UnityEngine.Random.value < SWITCH_CHANCE){
				cur_index = switchIndex(openIndicies);
			}
		}

		coin_count++;
		consecutive_coins++;
		if(coin_count >= coins_this_chunk){
			coin_count = -1;
		}

		return cur_index;
	}

	private static void init(ArrayList openIndicies){
		coin_count = 0;
		coins_this_chunk = SpawnController.Num_Items;
		start_index = Random.PickIndex(openIndicies);
		cur_index = start_index;
		consecutive_coins = 0;
		min_consecutive = coins_this_chunk / 4;
		max_consecutive = coins_this_chunk;
	}

	private static int switchIndex(ArrayList openIndicies){
		int[] validIndexes = new int[2];
		validIndexes[0] = (cur_index-1 >= 0)? 
			(int)openIndicies[cur_index-1]: 
			(int)openIndicies[openIndicies.Count-1];
		validIndexes[1] = (cur_index+1 < openIndicies.Count)? 
			(int)openIndicies[cur_index+1]: 
			(int)openIndicies[0];
		return Random.PickIndex(new ArrayList(validIndexes));
	}

}