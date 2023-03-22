using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        string[] lines = new string[5]
        {
            "O fata mov se arunca de pe bloc si lasa in urma ei o dara alba de scantei",
            "Fii Cosmos, mi-ai spus odata, uita de el, uita de tot",
            "Raluca Badulescu",
            "Praf de stele-pe hainele mele, si as vrea sa zbor pana la ele",
            "suntem niste smechere"
        };
        // Start is called before the first frame update
        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.typewriter;
            architect.speed = 0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            string longline = "this is a very long line that makes no sense but I am just populating it with  stuff because, you know,we all like stuff,so yeah you are an idiot sandwich you know";
            if (Input.GetKeyDown(KeyCode.Space))
                if (architect.isBuilding)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();
                }
                else
                    architect.Build(longline);
            //architect.Build(lines[Random.Range(0, lines.Length)]);
            else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(longline);
                //architect.Append(lines[Random.Range(0, lines.Length)]);

            }
        }
    }
}
