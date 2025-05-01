using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class PostBuildIOS
{
    [PostProcessBuild]
    public static void OnPostBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject proj = new PBXProject();
        proj.ReadFromFile(projPath);

#if UNITY_2019_3_OR_NEWER
        string frameworkTarget = proj.GetUnityFrameworkTargetGuid();
        string mainTarget = proj.GetUnityMainTargetGuid();
#else
        string frameworkTarget = proj.TargetGuidByName("UnityFramework");
        string mainTarget = proj.TargetGuidByName("Unity-iPhone");
#endif

        // Change Data folder to UnityFramework
        string dataFolderPath = "Data";
        string dataFolderGuid = proj.FindFileGuidByProjectPath(dataFolderPath);
        if (!string.IsNullOrEmpty(dataFolderGuid))
        {
            proj.RemoveFileFromBuild(mainTarget, dataFolderGuid);
            proj.AddFileToBuild(frameworkTarget, dataFolderGuid);
            Debug.Log("[Post Build] iOS : 'Data' folder reassigned to UnityFramework.");
        }

        // Change NativeCallProxy.h to Public
        string headerPath = "Libraries/Plugins/iOS/NativeCallProxy.h";
        string headerGuid = proj.FindFileGuidByProjectPath(headerPath);

        if (!string.IsNullOrEmpty(headerGuid))
        {
            proj.RemoveFileFromBuild(mainTarget, headerGuid);
            proj.AddPublicHeaderToBuild(frameworkTarget, headerGuid);

            Debug.Log("[Post Build] iOS : NativeCallProxy.h set as Public in UnityFramework.");
        }
        else
        {
            Debug.LogWarning("[Post Build] iOS : NativeCallProxy.h not found. Check Unity export path.");
        }

        proj.WriteToFile(projPath);
    }
}