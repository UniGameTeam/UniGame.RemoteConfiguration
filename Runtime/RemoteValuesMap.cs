namespace UniGame.RemoteConfiguration.Runtime
{
    using System;
    using Game.Modules.Assets.UniGame.RemoteConfiguration.Runtime;
    using UniModules.UniGame.Core.Runtime.DataStructure;

    [Serializable]
    public class RemoteValuesMap : SerializableDictionary<RemoteConfigId, RemoteValue> { }
}