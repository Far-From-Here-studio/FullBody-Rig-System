
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

#if FFH_ANIMATIONPACKAGE
namespace FFH.Animation
{
    public class RigModulesEditorWindow : EditorWindow, ISupportsOverlays
    {
        public static RigModulesEditorWindow window;
        public bool m_ShowOverlay = true;
        public static bool initOverlays = false;
        public static UnityEngine.Object selection;
        //public static List<InstanceOverlay> m_Overlay;
        public static List<string> m_OverlayButtons;
        [SerializeField]
        public static List<float> Weights;
        [SerializeField]
        public static List<Rect> Rects;

        public static RigModules RigModuleTarget;
        public static AutoBonesTarget AutoBonesTarget;
        public Texture FFHLogo;
        public Texture BodySelector;
        public bool Pin;
        public float LeftHandWeight;

        public static RigModulesEditorWindow Init(AutoBonesTarget bonesTarget)
        {
            RigModuleTarget = bonesTarget.RigModules;
            AutoBonesTarget = bonesTarget;
            selection = bonesTarget.gameObject;
            //if (!initOverlays) m_Overlay = new List<InstanceOverlay>();
            if (!initOverlays) Weights = new List<float>();
            if (!initOverlays) m_OverlayButtons = new List<string>();
            // Get existing open window or if none, make a new one:
            window = (RigModulesEditorWindow)EditorWindow.GetWindow(typeof(RigModulesEditorWindow));
            window.titleContent = new GUIContent() { text = "Rig Controller" };
            window.Show();
            return window;
        }
        private void Start()
        {
            if (EditorApplication.isPlaying)
            { 
                this.Close();
                Init(AutoBonesTarget);
            }

        }

        void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            PresentationPanel();
            InitButtons();
            EditorGUI.EndChangeCheck();

            if (!selection) selection = AutoBonesTarget.gameObject;

            /*
            //Init Overlays taking the created buttons 
            if (!initOverlays)
            {
                foreach (var overlays in m_OverlayButtons)
                {
                    var myoverlay = InitOverlays(overlays, out var weight);
                    m_Overlay.Add(myoverlay);
                    Weights.Add(weight);
                }
                initOverlays = true;
            }

            m_ShowOverlay = EditorGUILayout.Toggle("Show Overlay", m_ShowOverlay);

            if (initOverlays)
            {
                //Hide or Unhide Overlays
                foreach (var overlaypanel in m_Overlay)
                {
                    if (m_ShowOverlay)
                        overlayCanvas.Add(overlaypanel);
                    else
                        overlayCanvas.Remove(overlaypanel);
                }
            }
            */
        }
        /*
        InstanceOverlay InitOverlays(string labelName, out float Weight)
        {
            var overlay = new InstanceOverlay(this);
            overlay.textLabel = labelName;
            Weight = overlay.RigWeight;
 

            return overlay;
        }*/

        void PresentationPanel()
        {

     
            GUILayout.BeginHorizontal();
            GUILayout.Box(FFHLogo);
            GUILayout.BeginVertical();
            GUILayout.Label("FAR FROM HERE STUDIO", EditorStyles.boldLabel);
            GUILayout.Label("FullBody Rig System", EditorStyles.boldLabel);
            GUILayout.Space(5f);
            if(selection)GUILayout.Label(selection.name, EditorStyles.boldLabel);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Box(BodySelector);

        }

        void InitButtons()
        {

            //create Buttons Rig Selector shrtcut with Weight
            var rect = new Rect(242, 173, 20, 100);
            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            var buttonHead = GUILayout.Button("");
            if (!initOverlays) m_OverlayButtons.Add("Head");
            Weights.Add(new float());
            Weights[0] = GUILayout.HorizontalSlider(Weights[0], 0, 1);
            GUILayout.EndVertical();
            GUILayout.EndArea();

            rect = new Rect(242, 412, 20, 100);
            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            var buttonHips = GUILayout.Button("");
            if (!initOverlays) m_OverlayButtons.Add("Hips");
            Weights.Add(new float());
            Weights[1] = GUILayout.HorizontalSlider(Weights[1], 0, 1);
            GUILayout.EndVertical();
            GUILayout.EndArea();

            rect = new Rect(310, 418, 20, 100);
            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            var buttonLeftHand = GUILayout.Button("");
            if (!initOverlays) m_OverlayButtons.Add("LeftHand");
            Weights.Add(new float());
            Weights[2] = GUILayout.HorizontalSlider(Weights[2], 0, 1);
            GUILayout.EndVertical();
            GUILayout.EndArea();

            rect = new Rect(170, 418, 20, 100);
            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            var buttonRightHand = GUILayout.Button("");
            if (!initOverlays) m_OverlayButtons.Add("RightHand");
            Weights.Add(new float());
            Weights[3] = GUILayout.HorizontalSlider(Weights[3], 0, 1);
            GUILayout.EndVertical();
            GUILayout.EndArea();

            rect = new Rect(205, 620, 20, 100);
            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            var buttonLeftFoot = GUILayout.Button("");
            if (!initOverlays) m_OverlayButtons.Add("LeftFoot");
            Weights.Add(new float());
            Weights[4] = GUILayout.HorizontalSlider(Weights[4],0,1);
            GUILayout.EndVertical();
            GUILayout.EndArea();

            rect = new Rect(275, 620, 20, 100);
           GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            var buttonRightFoot = GUILayout.Button("");
            if (!initOverlays) m_OverlayButtons.Add("RightFoot");
            Weights.Add(new float());
            Weights[5] = GUILayout.HorizontalSlider(Weights[5], 0, 1);
            GUILayout.EndVertical();
            GUILayout.EndArea();


            rect = new Rect(100, 620, 20, 100);
            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            var buttonFootIK = GUILayout.Button("");
            if (!initOverlays) m_OverlayButtons.Add("FootIK");
            Weights.Add(new float());
            Weights[6] = GUILayout.HorizontalSlider(Weights[6], 0, 1);
            GUILayout.EndVertical();
            GUILayout.EndArea();

            rect = new Rect(100, 380, 20, 100);
           GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            var buttonUpbody = GUILayout.Button("");
            if (!initOverlays) m_OverlayButtons.Add("Upbody");
            Weights.Add(new float());
            Weights[7] = GUILayout.HorizontalSlider(Weights[7], 0, 1);
            GUILayout.EndVertical();
            GUILayout.EndArea();


            rect = new Rect(100, 200, 20, 100);
            GUILayout.BeginArea(rect);
            GUILayout.BeginVertical();
            var buttonLookAt = GUILayout.Button("");
            if (!initOverlays) m_OverlayButtons.Add("LookAt");
            Weights.Add(new float());
            Weights[8] = GUILayout.HorizontalSlider(Weights[8], 0, 1);
            GUILayout.EndVertical();
            GUILayout.EndArea();


        }


        void namelabel()
        {

        }
        /*

        public class InstanceOverlay : EditorWindow
        {
            public RigModulesEditorWindow m_Window;
            public InstanceOverlay(RigModulesEditorWindow win) => m_Window = win;

            public string textLabel;
            public float RigWeight;

            public override VisualElement CreatePanelContent()
            {
                var root = new VisualElement(); // Create a root visual element

                var label = new Label() { text = textLabel };
                root.Add(label); // Add your label to the root

                var slider = new Slider(0, 1); // Add your float slider with range 0 to 1
                slider.RegisterValueChangedCallback(evt => { RigWeight = evt.newValue; }); // Set the float value whenever the slider's value changes
                root.Add(slider);

        

                return root;
            }
        }
        */
        [Overlay(typeof(RigModulesEditorWindow), "Persistent Overlay", defaultDisplay = true)]
        public class PersistentOverlay : Overlay
        {
            public override VisualElement CreatePanelContent()
            {
                var root = new VisualElement(); // Create a root visual element

                var label = new Label() { text = "FullBodyLookAt" };
                root.Add(label); // Add your label to the root

                var toggle = new Toggle() { label = "Pin Transform" }; // Add your boolean toggle
                root.Add(toggle); // Add your toggle to the root
                return root;
            }

        }
    }
}
#endif