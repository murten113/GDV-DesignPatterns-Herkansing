using Events;
using Items;

namespace Managers
{
    public class ScoreManager
    {
        private int _score;

        public ScoreManager()
        {
            EventManager.OnItemPlaced += OnItemPlaced;
        }

        private void OnItemPlaced(Item item, int x, int y)
        {
            _score += item.ScoreValue;
            // Here you can add code to update UI or trigger other effects
            UnityEngine.Debug.Log($"Score updated! New score: {_score}");
        }

        public int GetScore() => _score;

        // Call this if you ever want to unsubscribe (like on app quit or scene change)
        public void Dispose()
        {
            EventManager.OnItemPlaced -= OnItemPlaced;
        }
    }
}
