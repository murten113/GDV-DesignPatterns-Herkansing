
namespace Managers
{
    public class ScoreManager
    {
        /// <summary>
        /// ScoreManager is a class that manages the player's score in the game.
        /// </summary>
        private int _score;

        /// <summary>
        /// Constructor for ScoreManager.
        /// </summary>
        public ScoreManager()
        {
            EventManager.OnItemPlaced += OnItemPlaced;
        }


        /// <summary>
        /// when an item is placed this method is called to update the score.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void OnItemPlaced(Item item, int x, int y)
        {
            _score += item.ScoreValue;
            UnityEngine.Debug.Log($"Score updated! New score: {_score}");
        }

        public int GetScore() => _score;

        /// <summary>
        /// Disposes the ScoreManager and unsubscribes from events.
        /// </summary>
        public void Dispose()
        {
            EventManager.OnItemPlaced -= OnItemPlaced;
        }
    }
}
