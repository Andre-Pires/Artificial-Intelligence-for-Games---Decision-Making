using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameManager
{
    public class GameManager : MonoBehaviour
    {
        private const float UPDATE_INTERVAL = 0.5f;

        //public fields, seen by Unity in Editor
        public GameObject character;

        public AutonomousCharacter autonomousCharacter;
        public GameObject redFlag;

        public Text HPText;
        public Text EnergyText;
        public Text HungerText;
        public Text ArrowsText;
        public Text MoneyText;

        //private fields
        public List<GameObject> chests;

        public List<GameObject> trees;
        public List<GameObject> boars;
        public List<GameObject> beds;
        public Dictionary<Vector3, GameObject> redFlags;
        public Dictionary<Vector3, GameObject> greenFlags;
        public Dictionary<Vector3, GameObject> resources;

        public CharacterData characterData;

        private float nextUpdateTime = 0.0f;
        private Vector3 previousPosition;

        public void Start()
        {
            this.characterData = new CharacterData(this.character);
            this.previousPosition = this.character.transform.position;

            this.resources = new Dictionary<Vector3, GameObject>();

            this.chests = GameObject.FindGameObjectsWithTag("Chest").ToList();
            foreach (var chest in this.chests)
            {
                this.resources.Add(chest.transform.position, chest);
            }
            this.trees = GameObject.FindGameObjectsWithTag("Tree").ToList();
            foreach (var tree in this.trees)
            {
                this.resources.Add(tree.transform.position, tree);
            }

            this.beds = GameObject.FindGameObjectsWithTag("Bed").ToList();
            foreach (var bed in this.beds)
            {
                this.resources.Add(bed.transform.position, bed);
            }

            this.boars = GameObject.FindGameObjectsWithTag("Boar").ToList();
            foreach (var boar in this.boars)
            {
                this.resources.Add(boar.transform.position, boar);
            }

            this.redFlags = new Dictionary<Vector3, GameObject>();
            foreach (var flag in GameObject.FindGameObjectsWithTag("RedFlag"))
            {
                this.redFlags.Add(flag.transform.position, flag);
            }
            this.greenFlags = new Dictionary<Vector3, GameObject>();
            foreach (var flag in GameObject.FindGameObjectsWithTag("GreenFlag"))
            {
                this.greenFlags.Add(flag.transform.position, flag);
            }
        }

        public void Update()
        {
            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + UPDATE_INTERVAL;

                //lower energy depending on the distance traversed since the last update
                var distance = (this.character.transform.position - this.previousPosition).magnitude;
                this.previousPosition = this.character.transform.position;
                var velocity = distance / UPDATE_INTERVAL;
                if (velocity > 40)
                {
                    this.characterData.Energy -= 0.02f * distance;
                }
                else if (velocity > 0)
                {
                    this.characterData.Energy -= 0.01f * distance;
                }

                if (characterData.Energy < 0)
                    characterData.Energy = 0;

                //increase hunger over time (0.1f per second)
                this.characterData.Hunger += 0.05f;
                if (this.characterData.Hunger > 10.0f)
                {
                    this.characterData.Hunger = 10.0f;
                }
            }

            this.HPText.text = "HP: " + this.characterData.HP;
            this.EnergyText.text = "Energy: " + this.characterData.Energy;
            this.HungerText.text = "Hunger: " + this.characterData.Hunger;
            this.ArrowsText.text = "Arrows: " + this.characterData.Arrows;
            this.MoneyText.text = "Money: " + this.characterData.Money;
        }

        public void PlaceFlag(Vector3 position)
        {
            if ((position - this.characterData.CharacterGameObject.transform.position).sqrMagnitude <= 9.0f)
            {
                if (!this.redFlags.ContainsKey(position))
                {
                    var newFlag = GameObject.Instantiate(this.redFlag);
                    newFlag.transform.position = position;

                    this.redFlags.Add(position, newFlag);

                    this.autonomousCharacter.UpdateRedFlags(this.redFlags.Values);
                    this.autonomousCharacter.ConquerGoal.InsistenceValue -= 1.5f;
                }
            }
        }

        public void MeleeAttack(GameObject boar)
        {
            if (boar != null && boar.activeSelf && InBoarMeleeRange(boar))
            {
                this.boars.Remove(boar);
                GameObject.DestroyObject(boar);
                this.characterData.HP -= 2;
                if (characterData.HP < 0)
                    characterData.HP = 0;

                this.characterData.Hunger -= 2;
                if (characterData.Hunger < 0)
                    characterData.Hunger = 0;

                this.characterData.Energy -= 0.5f;
                if (characterData.Energy < 0)
                    characterData.Energy = 0;

                this.resources.Remove(boar.transform.position);
                this.autonomousCharacter.UpdateResources(this.resources.Values);
            }
        }

        public void Shoot(GameObject boar)
        {
            if (boar != null && boar.activeSelf && InShootRange(boar) && this.characterData.Arrows > 0)
            {
                this.boars.Remove(boar);
                GameObject.DestroyObject(boar);
                this.characterData.Hunger -= 2;
                if (characterData.Hunger < 0)
                    characterData.Hunger = 0;
                this.characterData.Arrows--;

                this.resources.Remove(boar.transform.position);
                this.autonomousCharacter.UpdateResources(this.resources.Values);
            }
        }

        public void PickUpChest(GameObject chest)
        {
            if (chest != null && chest.activeSelf && InChestRange(chest))
            {
                this.chests.Remove(chest);
                GameObject.DestroyObject(chest);
                this.characterData.Money += 5;

                this.resources.Remove(chest.transform.position);
                this.autonomousCharacter.UpdateResources(this.resources.Values);
            }
        }

        public void GetArrows(GameObject tree)
        {
            if (InTreeRange(tree))
            {
                this.characterData.Arrows = 10;
            }
        }

        public void Rest()
        {
            //you can rest anywhere and at anytime, but it doesn't give much energy
            this.characterData.Energy += 0.5f;
            if (this.characterData.Energy > 10.0f)
            {
                this.characterData.Energy = 10.0f;
            }
        }

        public void Sleep(GameObject bed)
        {
            if (InBedRange(bed))
            {
                this.characterData.Energy += 1.0f;
                if (this.characterData.Energy > 10.0f)
                {
                    this.characterData.Energy = 10.0f;
                }
            }
        }

        private bool CheckRange(GameObject obj, float maximumSqrDistance)
        {
            return (obj.transform.position - this.characterData.CharacterGameObject.transform.position).sqrMagnitude <= maximumSqrDistance;
        }

        public bool InBoarMeleeRange(GameObject boar)
        {
            return this.CheckRange(boar, 9.0f);
        }

        public bool InShootRange(GameObject boar)
        {
            return this.CheckRange(boar, 36.0f);
        }

        public bool InChestRange(GameObject chest)
        {
            return this.CheckRange(chest, 9.0f);
        }

        public bool InBedRange(GameObject bed)
        {
            return this.CheckRange(bed, 9.0f);
        }

        public bool InTreeRange(GameObject tree)
        {
            return this.CheckRange(tree, 9.0f);
        }
    }
}