using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //Character Ellen = CharacterManager.instance.CreateCharacter("Ellen");
            //Character Kurumi = CharacterManager.instance.CreateCharacter("Kurumi");
            //Character Adam = CharacterManager.instance.CreateCharacter("Adam");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Character Kurumi = CharacterManager.instance.CreateCharacter("Kurumi");
            List<string> lines = new List<string>()
            {
                "Hi there",
                "That's very... {wa 1} interesting."
            };
            yield return Kurumi.Say(lines);
        }
    }
}