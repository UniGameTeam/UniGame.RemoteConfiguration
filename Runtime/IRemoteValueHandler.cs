namespace UniGame.RemoteConfiguration.Runtime
{
    using Cysharp.Threading.Tasks;
    using Game.Modules.Assets.UniGame.RemoteConfiguration.Runtime;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface IRemoteValueHandler
    {
        bool CanHandle(RemoteConfigId id,RemoteValue value);

        void    InitializeValue(RemoteValue defaultValue);
        
        UniTask<bool> UpdateValue(RemoteConfigId valueId,RemoteValue value, IContext context);
    }
}