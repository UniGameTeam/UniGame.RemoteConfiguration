namespace UniGame.RemoteConfiguration.Runtime
{
    using System;
    using Cysharp.Threading.Tasks;
    using Game.Modules.Assets.UniGame.RemoteConfiguration.Runtime;
    using UniModules.UniGame.Core.Runtime.Interfaces;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public abstract class RemoteValueConfigurationHandler : IRemoteValueHandler
    {
        public RemoteConfigId id;

#if ODIN_INSPECTOR
        [TitleGroup(nameof(value))]
        [HideLabel]
#endif
        public RemoteValue value;
        
        public RemoteConfigId Id => id;

        public RemoteValue Value => value;

        public virtual bool CanHandle(RemoteConfigId valueId, RemoteValue remoteValue)
        {
            if (id != valueId) return false;
            if (Equals(remoteValue, value)) return false;

            return true;
        }

        public void InitializeValue(RemoteValue defaultValue) => value = defaultValue;
        
        
        public async UniTask<bool> UpdateValue(RemoteConfigId valueId,RemoteValue remoteValue, IContext context)
        {
            if (!CanHandle(valueId,remoteValue)) return false;

            var token = context.LifeTime.TokenSource;
            var result = await OnUpdateValue(remoteValue, context).AttachExternalCancellation(token);
            if (!result) return false;
            value = remoteValue;

            return true;
        }

        protected abstract UniTask<bool> OnUpdateValue(RemoteValue remoteValue, IContext context);
    }
}