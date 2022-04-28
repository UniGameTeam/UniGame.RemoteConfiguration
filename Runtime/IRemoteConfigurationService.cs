namespace UniGame.RemoteConfiguration.Runtime
{
    using Cysharp.Threading.Tasks;
    using Game.Modules.Assets.UniGame.RemoteConfiguration.Runtime;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;

    public interface IRemoteConfigurationService : IGameService
    {
        bool    IsInitialized { get; }
        bool    IsDebug       { get; }
        void    Update();
        UniTask UpdateAsync();

        public RemoteValue GetValue(RemoteConfigId id);
    }
}