// Layer Testing Script //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class layerTesting : MonoBehaviour
{

    LayerController controller;

    public Texture2D tex;
    public VideoClip mov;

    public float transitionSpeed;

    // Start is called before the first frame update
    void Start()
    {
        controller = LayerController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        // To switch do Key for layer + A or S + *optional T, for transition*
        LayerController.GRAPHIC_LAYER layer = null;
        if (Input.GetKey(KeyCode.Q))
        {
            layer = controller.background;
        }
        if (Input.GetKey(KeyCode.W))
        {
            layer = controller.cinematic;
        }
        if (Input.GetKey(KeyCode.E))
        {
            layer = controller.foreground;
        }
        // If I press T, use transition.
        if (Input.GetKey(KeyCode.T))
        {
            // If transitioning we use the speed variable.
            if (Input.GetKeyDown(KeyCode.A))
            {
                layer.SetTexture(tex, transitionSpeed);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                layer.SetVideo(mov, transitionSpeed);
            }
        }
        else
        { // If I just press A, we just switch without transitioning.
            if (Input.GetKeyDown(KeyCode.A))
            {
                layer.SetTexture(tex, 2f); // I use 2f for a nearly instant but still nice looking transition.
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                layer.SetVideo(mov, 2f);
            }
        }
    }
}
