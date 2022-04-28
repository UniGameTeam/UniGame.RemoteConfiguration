namespace UniGame.RemoteConfiguration.Runtime
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/Modules/RemoteConfiguration")]
    public class RemoteConfigurationAsset : ScriptableObject
    {
#if ODIN_INSPECTOR
        [HideLabel]
        [InlineProperty]
#endif
        public RemoteConfiguration remoteConfiguration = new RemoteConfiguration();
    }
}