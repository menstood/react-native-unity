using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;
using System.Xml;

public class PostProcessingBuildAndroid
{
    [PostProcessBuild(1)]
    public static void OnPostBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.Android) return;

        var manifestPath = Path.Combine(pathToBuiltProject, "src", "main", "AndroidManifest.xml");
        if (!File.Exists(manifestPath)) return;

        XmlDocument manifest = new XmlDocument();
        manifest.Load(manifestPath);

        XmlNode appNode = manifest.SelectSingleNode("/manifest/application");

        if (appNode != null)
        {
            foreach (XmlNode activityNode in appNode.SelectNodes(".//activity"))
            {
                var intentFilters = activityNode.SelectNodes("intent-filter");
                for (int i = intentFilters.Count - 1; i >= 0; i--)
                {
                    activityNode.RemoveChild(intentFilters[i]);
                }
            }
        }

        manifest.Save(manifestPath);
        Debug.Log("[Post Build] Android : Removed intent-filters from AndroidManifest.xml Success!.");
    }
}
