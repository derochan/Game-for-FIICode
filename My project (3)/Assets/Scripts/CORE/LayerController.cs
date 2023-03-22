//  Layer Controller (BCFC) //

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LayerController : MonoBehaviour
{
    public static LayerController instance;
    /// <summary>The graphic layer in the background behind everything.</summary>
    public GRAPHIC_LAYER background = new GRAPHIC_LAYER();
    /// <summary>The graphic layer in front of the characters but behind the dialogue.</summary>
    public GRAPHIC_LAYER cinematic = new GRAPHIC_LAYER();
    /// <summary>the graphic layer above everything.</summary>
    public GRAPHIC_LAYER foreground = new GRAPHIC_LAYER();

    [Tooltip("The RawImage or CanvasGroup prefab for a new image or movie on a layer.")]
    /// <summary>
    /// The RawImage or CanvasGroup prefab for a new image or movie on a layer.
    /// </summary>
    public GameObject graphicPrefab;

    public static float transitionSpeed = 3f;

    private void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class GRAPHIC_LAYER
    {
        public Transform panel;
        /// <summary>
        /// the active graphic on this layer
        /// </summary>
        [HideInInspector]
        public GRAPHIC_OBJECT currentGraphic = null;
        /// <summary>
        /// the old graphics on this object that have not been destroyed yet.
        /// </summary>
        [HideInInspector]
        public List<GRAPHIC_OBJECT> oldGraphics = new List<GRAPHIC_OBJECT>();
        ///useless at the moment.
        public Coroutine specialTransitionCoroutine = null;

        /// <summary>
        /// the speed at which this layer will handle object transitions
        /// </summary>
        [HideInInspector] public float transitionSpeed = 1f;

        public void SetTexture(Texture2D tex, float transitionSpeed = 1f)
        {
            this.transitionSpeed = transitionSpeed;
            //figure out if we already have this texture on a graphic. Will be the current graphic instance or an instance in the old graphics
            CreateGraphic(tex);
        }

        public void SetVideo(VideoClip clip, float transitionSpeed = 1f, bool useAudio = true)
        {
            this.transitionSpeed = transitionSpeed;
            CreateGraphic(clip, useAudio);
        }

        /// <summary>
        /// disable all graphics on this layer to clear it out.
        /// </summary>
        public void ClearLayer()
        {
            if (currentGraphic != null && !currentGraphic.isNull)
                currentGraphic.Disable();

            foreach (GRAPHIC_OBJECT graphic in oldGraphics)
            {
                graphic.Disable();
            }
        }

        /// <summary>
        /// Try to get an existing graphic with this texture on it.
        /// </summary>
        /// <param name="tex"></param>
        /// <returns></returns>
        void CreateGraphic(Texture2D tex)
        {
            if (currentGraphic != null && !currentGraphic.isNull && currentGraphic.renderer.texture == tex)
            {
                return;
            }

            foreach (GRAPHIC_OBJECT graphic in oldGraphics)
            {
                if (graphic.NameMatchesGraphic(tex))
                {
                    if (currentGraphic != null && !currentGraphic.isNull && !oldGraphics.Contains(currentGraphic))
                    {
                        oldGraphics.Add(currentGraphic);
                        currentGraphic.Disable(transitionSpeed);
                    }
                    oldGraphics.Remove(graphic);
                    currentGraphic = graphic;
                    currentGraphic.Enable(transitionSpeed);
                    return;
                }

            }

            //At this point, the graphic does not exist and we need to create it.
            GRAPHIC_OBJECT newGraphic = new GRAPHIC_OBJECT(this, tex);

            //if there is already a current graphic, set it to be an old one.
            if (currentGraphic != null && !currentGraphic.isNull && !oldGraphics.Contains(currentGraphic))
            {
                oldGraphics.Add(currentGraphic);
                currentGraphic.Disable(transitionSpeed);
            }

            currentGraphic = newGraphic;
            currentGraphic.Enable(transitionSpeed);
        }

        void CreateGraphic(VideoClip clip, bool useAudio = true)
        {
            if (currentGraphic != null && !currentGraphic.isNull && currentGraphic.NameMatchesGraphic(clip))
            {
                return;
            }

            foreach (GRAPHIC_OBJECT graphic in oldGraphics)
            {
                if (graphic.NameMatchesGraphic(clip))
                {
                    if (currentGraphic != null && !currentGraphic.isNull && !oldGraphics.Contains(currentGraphic))
                    {
                        oldGraphics.Add(currentGraphic);
                        currentGraphic.Disable(transitionSpeed);
                    }
                    oldGraphics.Remove(graphic);
                    currentGraphic = graphic;
                    currentGraphic.Enable(transitionSpeed);
                    return;
                }

            }

            //At this point, the graphic does not exist and we need to create it.
            GRAPHIC_OBJECT newGraphic = new GRAPHIC_OBJECT(this, clip, useAudio);

            //if there is already a current graphic, set it to be an old one.
            if (currentGraphic != null && !currentGraphic.isNull && !oldGraphics.Contains(currentGraphic))
            {
                oldGraphics.Add(currentGraphic);
                currentGraphic.Disable(transitionSpeed);
            }

            currentGraphic = newGraphic;
            currentGraphic.Enable(transitionSpeed);
        }

        /// <summary>
        /// A graphic object is an image or texture in a graphic layer.
        /// </summary>
        public class GRAPHIC_OBJECT
        {
            public bool isNull { get { return canvasgroup == null; } }
            /// <summary>
            /// the layer this object belongs to
            /// </summary>
            [HideInInspector] public GRAPHIC_LAYER layer;
            /// <summary>
            /// the alpha control of this object
            /// </summary>
            public CanvasGroup canvasgroup;
            /// <summary>
            /// the renderer for setting the display texture or video
            /// </summary>
            public RawImage renderer;

            /// <summary>
            /// is this a video graphic or a just a texture?
            /// </summary>
            bool isVideo { get { return video != null; } }
            /// <summary>
            /// If this graphic is playing a video, this is the video player.
            /// </summary>
            [HideInInspector] public VideoPlayer video = null;

            public GRAPHIC_OBJECT(GRAPHIC_LAYER layer, Texture2D tex)
            {
                this.layer = layer;
                GameObject ob = Object.Instantiate(LayerController.instance.graphicPrefab, LayerController.instance.transform) as GameObject;
                ob.SetActive(true);
                canvasgroup = ob.GetComponent<CanvasGroup>();
                renderer = ob.GetComponent<RawImage>();
                renderer.texture = tex;
                ob.transform.SetParent(layer.panel);

                canvasgroup.name = $"GRAPHIC - [{tex.name}]";
            }

            public GRAPHIC_OBJECT(GRAPHIC_LAYER layer, VideoClip clip, bool useVideoAudio = true)
            {
                this.layer = layer;
                GameObject ob = Object.Instantiate(LayerController.instance.graphicPrefab, LayerController.instance.transform) as GameObject;
                ob.SetActive(true);
                canvasgroup = ob.GetComponent<CanvasGroup>();
                renderer = ob.GetComponent<RawImage>();
                ob.transform.SetParent(layer.panel);

                //Create the texture for the video to use.
                RenderTexture tex = new RenderTexture(Mathf.RoundToInt(clip.width), Mathf.RoundToInt(clip.height), 0);
                renderer.texture = tex;

                //Create the player to handle the video
                video = renderer.gameObject.AddComponent<VideoPlayer>();
                video.source = VideoSource.VideoClip;
                video.clip = clip;
                video.renderMode = VideoRenderMode.RenderTexture;
                video.targetTexture = tex;
                video.isLooping = true;

                video.SetDirectAudioVolume(0, 0);//mute the audio so it fades in with the video
                if (!useVideoAudio)
                    video.SetDirectAudioMute(0, true);

                canvasgroup.name = $"GRAPHIC - [{clip.name}]";
            }

            public bool NameMatchesGraphic(Texture2D tex)
            {
                return canvasgroup.name == $"GRAPHIC - [{tex.name}]";
            }

            public bool NameMatchesGraphic(VideoClip clip)
            {
                return canvasgroup.name == $"GRAPHIC - [{clip.name}]";
            }


            public void Enable(float speed = 1f)
            {
                layer.transitionSpeed = speed;

                if (!isEnabling)
                {
                    _enabling = true;
                    if (settingVisibility != null)
                        LayerController.instance.StopCoroutine(settingVisibility);

                    settingVisibility = LayerController.instance.StartCoroutine(SetVisibility(1f));
                }
            }

            public void Disable(float speed = 1f)
            {
                layer.transitionSpeed = speed;

                if (!isDisabling)
                {
                    _disabling = true;
                    if (settingVisibility != null)
                        LayerController.instance.StopCoroutine(settingVisibility);

                    settingVisibility = LayerController.instance.StartCoroutine(SetVisibility(0f));
                }
            }

            bool _enabling = false;
            bool _disabling = false;
            public bool isEnabling { get { return _enabling; } }
            public bool isDisabling { get { return _disabling; } }
            Coroutine settingVisibility = null;
            IEnumerator SetVisibility(float alpha)
            {
                while (canvasgroup.alpha != alpha)
                {
                    canvasgroup.alpha = Mathf.MoveTowards(canvasgroup.alpha, alpha, layer.transitionSpeed * LayerController.transitionSpeed * Time.deltaTime);

                    //fade the volume of videos along with the texture itself.
                    if (isVideo)
                        video.SetDirectAudioVolume(0, canvasgroup.alpha);

                    yield return new WaitForEndOfFrame();
                }

                settingVisibility = null;
                _enabling = false;
                _disabling = false;
                //if the target is 0 then this object has faded out and should be destroyed
                if (alpha == 0)
                {
                    Destroy();
                }
            }

            void Destroy()
            {
                Object.DestroyImmediate(canvasgroup.gameObject);

                //remove this object from the old renderers of the layer
                if (layer.oldGraphics.Contains(this))
                    layer.oldGraphics.Remove(this);
                else if (layer.currentGraphic == this)
                    layer.currentGraphic = null;
            }
        }
    }
}