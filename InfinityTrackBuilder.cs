using Level;
using Units.Hero;
using UnityEngine;
using UnityEngine.UI;

namespace Runner
{
    class InfinityTrackBuilder : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _startPlatform;
        
        private readonly string _levelpath = "Prefabs/LevelConstruct/Level_";

        private System.Collections.Generic.Queue<GameObject> Levels;

        private int _startDestroyCounter = 2;
        private float nextPositionX;

        private HeroView _hero;


        private void OnEnable()
        {
            Levels = new System.Collections.Generic.Queue<GameObject>();
        }

        public void SetHero(HeroView hero)
        {
            _hero = hero;
        }

        public void CreateRoad()
        {
            float startX = _startPlatform.transform.position.x + _startPlatform.size.x / 2;

            nextPositionX = startX;

            for (int i = 0; i < 4; i++)
            {
                CreateOneLevel();
            }
        }

        private void PlayerFinishSomePlatform()
        {
            if (_startDestroyCounter != 0)
            {
                _startDestroyCounter -= 1;
            }
            else
            {
                GameObject last = Levels.Peek();

                Levels.Dequeue();

                Destroy(last.gameObject);

                CreateOneLevel();
            }
        }

        private void CreateOneLevel()
        {

            LevelParam track = Instantiate(Resources.Load(_levelpath + Random.Range(1, 11), typeof(LevelParam))) as LevelParam;

            _hero.HeroCompletedLevel = PlayerFinishSomePlatform;

            track.gameObject.transform.position = new Vector3(nextPositionX, _startPlatform.transform.position.y, 0);

            nextPositionX = track.gameObject.transform.GetComponent<LevelParam>().FinishPosX;

            Levels.Enqueue(track.gameObject);
        }

        public void RestartGame()
        {
            foreach (var level in Levels)
            {
                Destroy(level.gameObject);
            }
            Levels.Clear();
        }
    }
}
//_startPlatform.transform.GetComponent<Image>().rectTransform.rect.width - для UI