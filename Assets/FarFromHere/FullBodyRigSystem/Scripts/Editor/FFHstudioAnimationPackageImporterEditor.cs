using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;

[InitializeOnLoad]
public class FFHstudioAnimationPackageImporterEditor : EditorWindow
{
    static SearchRequest SearchRequest;
    static AddRequest AddRequest;
    static RemoveRequest removeRequest;
    static EmbedRequest EmbedRequest;
    static ListRequest ListRequest;

    public static bool showOnStart;
    public static bool ListPackages = false;
    public static List<string> PackagesNames;

    public FFHPackagesData FFHData;
    public static FFHPackagesData data;


    private static bool defineSymbols;
    bool canDefine = true;
    // Start is called before the first frame update
    [MenuItem("FarFromHereStudio/Animation Packages")]
    public static void Init()
    {
        // Get existing open window or if none, make a new one:
        FFHstudioAnimationPackageImporterEditor window = (FFHstudioAnimationPackageImporterEditor)EditorWindow.GetWindow(typeof(FFHstudioAnimationPackageImporterEditor));
        window.Show();
        PackagesNames = new List<string>();
        List();
    }

    static FFHstudioAnimationPackageImporterEditor()
    {
        EditorApplication.update += Startup;
    }

    static void Startup()
    {
        data = AssetDatabase.LoadAssetAtPath<FFHPackagesData>("Assets/FarFromHere/FFH Package Manager/Editor/Data/FFH Packages.asset");
        showOnStart = false;
        showOnStart = GetStartupValue(data);
        if (showOnStart) Init();
        EditorApplication.update -= Startup;
    }

    public static bool GetStartupValue(FFHPackagesData FFHData)
    {
        return FFHData.ShowAtStart;
    }
    void OnGUI()
    {
        bool HideImport = true;
        if (ListPackages != true)
        {
            PackagesNames = new List<string>();
            //GetList
            List();
            ListPackages = true;
        }
        if (PackagesNames.Count == 0)
        {
            HideImport = true;
        }
        if (PackagesNames.Count > 0)
        {
            HideImport = false;
        }
        if (HideImport == true)
        {
            GUILayout.Label("Loading Packages", EditorStyles.miniLabel);
        }

        //GUI presentation Title
        GUILayout.Label("Package Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        for (var e = 0; e < FFHData.Packages.Length; e++)
        {
            FFHData.Packages[e].InstalledPackages = false;
            for (var i = 0; i < PackagesNames.Count; i++)
            {
                //Check FFH package installed or not
                if (FFHData.Packages[e].Name == PackagesNames[i] && FFHData.Packages[e].InstalledPackages == false)
                {
                    FFHData.Packages[e].InstalledPackages = true;
                }
            }
        }

        if (ListPackages && HideImport == false)
        {
            for (var p = 0; p < FFHData.Packages.Length; p++)
            {
                ImportRemovePackagesGUI(FFHData.Packages[p], FFHData.Packages[p].GUIState);
                if (FFHData.Packages[p].InstalledPackages == false) canDefine = false;
            }
            EditorGUILayout.Space();
        }
        CheckDefines();

        FFHData.ShowAtStart = GUILayout.Toggle(FFHData.ShowAtStart, "ShowOnStartup");
        EditorUtility.SetDirty(FFHData);
    }
    void CheckDefines()
    {
        if (!defineSymbols && canDefine)
        {
            string define = "FFH_ANIMATIONPACKAGE";
            BuildTargetGroup BuildTargetGroupSelected = EditorUserBuildSettings.selectedBuildTargetGroup;
            string existingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroupSelected);
            // Check if your define already exists in the current symbols
            if (!existingSymbols.Split(';').Contains(define))
            {
                // Append your new define symbol
                string newSymbols = existingSymbols + ";" + define;
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(BuildTargetGroupSelected), newSymbols);
            }
            defineSymbols = true;
        }
        else if (defineSymbols && !canDefine)
        {
            string define = "FFH_ANIMATIONPACKAGE";
            BuildTargetGroup buildTargetGroupSelected = EditorUserBuildSettings.selectedBuildTargetGroup;
            string existingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroupSelected);

            // Check if your define exists in the current symbols
            if (existingSymbols.Split(';').Contains(define))
            {
                // Remove your define symbol
                string[] symbols = existingSymbols.Split(';');
                symbols = symbols.Where(s => s != define).ToArray();
                string newSymbols = string.Join(";", symbols);
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroupSelected), newSymbols);
            }
            defineSymbols = false;
        }
    }
    void ImportRemovePackagesGUI(packageActive package, bool GUIState)
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUIState = true;
        GUIState = EditorGUILayout.BeginFoldoutHeaderGroup(GUIState, package.folderGroupLabel);
        GUILayout.FlexibleSpace();
        GUILayout.Toggle(package.InstalledPackages, "Installed");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (GUIState == true)
        {
            var ImportButton = GUILayout.Button("Import Package");
            var RemoveButton = GUILayout.Button("Remove Package");
            var EmbedButton = GUILayout.Button("Embed Package");

            if (ImportButton)
            {
                Debug.Log(package.Name + " Installation..");
                var ID = package.Adress;
                Add(ID);
                ListPackages = false;
            }
            if (RemoveButton)
            {
                Debug.Log(package.Name + " UnInstallation..");
                var ID = package.Name;
                Remove(ID);
                ListPackages = false;
            }

            if (EmbedButton)
            {

                Debug.Log(package.Name + " .. : Embeding..");
                var ID = package.Name;
                Embed(ID);
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    static void List()
    {
        ListRequest = Client.List(false, true);
        EditorApplication.update += ListProgress;
    }
    static void ListProgress()
    {
        if (ListRequest.IsCompleted)
        {
            if (ListRequest.Status == StatusCode.Success)
            {
                foreach (var package in ListRequest.Result)
                {
                    PackagesNames.Add(package.name);
                }
            }
            else if (ListRequest.Status >= StatusCode.Failure)
            {
                Debug.Log(ListRequest.Error.message);
            }
            EditorApplication.update -= ListProgress;
        }
    }
    static void Add(string PackageNameOrID)
    {
        AddRequest = Client.Add(PackageNameOrID);
        EditorApplication.update += AddProgress;
    }
    static void AddProgress()
    {
        if (AddRequest.IsCompleted)
        {
            if (AddRequest.Status == StatusCode.Success)
            {
                Debug.Log("Installed: " + AddRequest.Result.packageId);
            }

            else if (AddRequest.Status >= StatusCode.Failure)
            {
                Debug.Log(AddRequest.Error.message);
            }

            EditorApplication.update -= AddProgress;
        }

    }

    static void Embed(string PackageNameOrID)
    {
        // AddHDRP a package to the project
        EmbedRequest = Client.Embed(PackageNameOrID);
        EditorApplication.update += EmbedProgress;
    }

    static void EmbedProgress()
    {
        if (EmbedRequest.IsCompleted)
        {
            if (EmbedRequest.Status == StatusCode.Success)
            {
                Debug.Log("Embeding Package.. : " + EmbedRequest.Result.packageId);

            }

            else if (EmbedRequest.Status >= StatusCode.Failure)
            {
                Debug.Log(EmbedRequest.Error.message);

            }
            EditorApplication.update -= EmbedProgress;
        }
    }

    static void Remove(string PackageNameOrID)
    {
        // AddHDRP a package to the project
        removeRequest = Client.Remove(PackageNameOrID);
        EditorApplication.update += RemoveProgress;
    }
    static void RemoveProgress()
    {
        if (removeRequest.IsCompleted)
        {
            if (removeRequest.Status == StatusCode.Success)
            {
                Debug.Log("UnInstalled: " + removeRequest.PackageIdOrName);
            }

            else if (removeRequest.Status >= StatusCode.Failure)
            {
                Debug.Log(removeRequest.Error.message);
            }
            EditorApplication.update -= RemoveProgress;
        }
    }
}


