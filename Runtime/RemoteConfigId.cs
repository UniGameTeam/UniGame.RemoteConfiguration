namespace UniGame.RemoteConfiguration.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniModules.UniGame.Runtime;
    using UnityEngine;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
#if UNITY_EDITOR
    using UnityEditor;
    using UniModules.Editor;
#endif
    
#if ODIN_INSPECTOR
    [InlineProperty]
    [ValueDropdown("@UniGame.RemoteConfiguration.Runtime.RemoteConfigId.GetRemoteIds()")]
#endif
    [Serializable]
    public struct RemoteConfigId : IId<RemoteConfigId>
    {
        [SerializeField, HideInInspector]
        private string _value;

        public static implicit operator string(RemoteConfigId v) => v._value;

        public static explicit operator RemoteConfigId(string v) => new RemoteConfigId { _value = v };

        public override string ToString() => _value;

        public override int GetHashCode() => string.IsNullOrEmpty(_value) ? 0 : _value.GetHashCode();

        public RemoteConfigId FromString(string value)
        {
            _value = value;
            return this;
        }

        public override bool Equals(object obj)
        {
            if (!string.IsNullOrEmpty(_value) && obj is RemoteConfigId gameResourceId)
                return _value.Equals(gameResourceId._value);
            
            return false;
        }

#if UNITY_EDITOR
        
        public static IEnumerable<RemoteConfigId> GetRemoteIds()
        {
            var configurationAsset = AssetEditorTools.GetAsset<RemoteConfigurationAsset>();
            if (configurationAsset == null)
                yield break;

            var config = configurationAsset.remoteConfiguration;
            foreach (var id in config.remoteIds)
            {
                yield return (RemoteConfigId) id;
            }
        }

#endif        

    }
}