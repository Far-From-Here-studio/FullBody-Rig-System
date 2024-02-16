using UnityEngine;

[System.Serializable]
public struct packageActive
{
    public bool InstalledPackages { get; set; }
    public string Name;
    public string Adress;
    public string folderGroupLabel;
    public bool GUIState { get; set; }

}

[CreateAssetMenu(fileName = "FFH Packages", menuName = "FFH/PackageData")]
public class FFHPackagesData : ScriptableObject
{
    public bool ShowAtStart;
    public packageActive[] Packages;
}
