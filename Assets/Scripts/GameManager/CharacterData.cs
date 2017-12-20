using UnityEngine;

namespace Assets.Scripts.GameManager
{
    public class CharacterData
    {
        public int HP { get; set; }
        public float Energy { get; set; }
        public float Hunger { get; set; }
        public int Arrows { get; set; }
        public int Money { get; set; }
        public GameObject CharacterGameObject { get; private set; }

        public CharacterData(GameObject gameObject)
        {
            this.CharacterGameObject = gameObject;
            this.HP = 10;
            this.Hunger = 0;
            this.Money = 0;
            this.Arrows = 2;
            this.Energy = 10.0f;
        }
    }
}
