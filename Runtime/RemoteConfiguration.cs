namespace UniGame.RemoteConfiguration.Runtime
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class RemoteConfiguration
    {
        public int                       initializationRetryDelay = 3000;
        public RemoteTimingConfiguration timeConfiguration        = new RemoteTimingConfiguration();
        public RemoteTimingConfiguration debugTimeConfiguration   = new RemoteTimingConfiguration();
        
        public List<string> remoteIds = new List<string>();
        
#if ODIN_INSPECTOR
        [InlineProperty]
#endif
        public RemoteValuesMap values = new RemoteValuesMap();
        
        [SerializeReference]
        [Space]
#if ODIN_INSPECTOR
        [InlineProperty]
#endif
        public List<IRemoteValueHandler> handlers = new List<IRemoteValueHandler>();
        
    }
    
    [Serializable]
    public class RemoteTimingConfiguration
    {
        public int fetchTimeoutInMilliseconds         = 2000;
        public int minimumFetchInternalInMilliseconds = 180000;
        public int defaultCacheExpirationMilliseconds = 1800000;
    }
}