using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VRC.UI.Core.Styles;

[assembly: MelonInfo(typeof(AnimatedBackground.Main), "AnimBackground", "1.0", "Four_DJ")]
[assembly: MelonGame("VRChat", "VRChat")]

namespace AnimatedBackground
{
    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            if (!Directory.Exists("UserData/AnimBackground"))
                Directory.CreateDirectory("UserData/AnimBackground");

            if (!File.Exists("UserData/AnimBackground/background.mp4"))
                return;

            MelonCoroutines.Start(WaitForUI());
        }

        private IEnumerator WaitForUI()
        {
            while (VRCUiManager.prop_VRCUiManager_0 == null)
                yield return null;

            while (VRC.UI.Core.UIManager.prop_UIManager_0 == null)
                yield return null;

            VRC.UI.Elements.QuickMenu quickMenu;

            while ((quickMenu = GameObject.Find("UserInterface").GetComponentInChildren<VRC.UI.Elements.QuickMenu>(true)) == null)
                yield return null;

            while (quickMenu.prop_MenuStateController_0 == null)
                yield return null;

            while (quickMenu.GetComponent<StyleEngine>() == null)
                yield return null;

            GameObject.DestroyImmediate(quickMenu.transform.Find("Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").gameObject);

            GameObject vapePlayer = GameObject.Instantiate(AssetBundle.LoadFromMemory(LoadRecourse("vapevideo")).LoadAsset<GameObject>("vapePlayer"));
            GameObject.DontDestroyOnLoad(vapePlayer);

            GameObject ogbackground = quickMenu.transform.Find("Container/Window/QMParent/BackgroundLayer01").gameObject;

            GameObject maskebackground = quickMenu.transform.Find("Container/Window/QMParent/BackgroundLayer02").gameObject;

            ogbackground.transform.parent = maskebackground.transform;

            UnityEngine.Object.DestroyImmediate(maskebackground.GetComponent<StyleElement>());
            maskebackground.GetComponent<Image>().sprite = LoadSprite(LoadRecourse("map.png"));
            maskebackground.AddComponent<Mask>();

            UnityEngine.Object.DestroyImmediate(ogbackground.GetComponent<StyleElement>());
            UnityEngine.Object.DestroyImmediate(ogbackground.GetComponent<Image>());

            VideoPlayer videoPlayer = vapePlayer.GetComponent<VideoPlayer>();

            videoPlayer.url = $"{Directory.GetCurrentDirectory()}/UserData/AnimBackground/background.mp4";

            ogbackground.AddComponent<RawImage>().texture = videoPlayer.targetTexture;

            yield break;
        }

        internal byte[] LoadRecourse(string name)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"AnimatedBackground.Recourses.{name}");

            byte[] ba = new byte[stream.Length];
            stream.Read(ba, 0, ba.Length);
            stream.Close();
            return ba;
        }

        internal Texture2D LoadTexture(byte[] bytes)
        {
            Texture2D texture = new Texture2D(1, 1);
            ImageConversion.LoadImage(texture, bytes);
            texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            texture.wrapMode = TextureWrapMode.Clamp;

            return texture;
        }

        internal Sprite LoadSprite(byte[] bytes)
        {
            Texture2D texture = LoadTexture(bytes);

            Rect rect = new Rect(0.0f, 0.0f, texture.width, texture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);
            Vector4 border = Vector4.zero;
            Sprite sprite = Sprite.CreateSprite_Injected(texture, ref rect, ref pivot, 100.0f, 0, SpriteMeshType.Tight, ref border, false);
            sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;

            return sprite;
        }
    }
}
