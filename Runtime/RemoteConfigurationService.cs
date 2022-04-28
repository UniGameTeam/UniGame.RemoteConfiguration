namespace UniGame.RemoteConfiguration.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Firebase.RemoteConfig;
    using Game.Modules.Assets.UniGame.RemoteConfiguration.Runtime;
    using UniGame.UniNodes.GameFlow.Runtime;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Interfaces;
    using UniRx;
#if UNITY_EDITOR
    using UnityEditor;
#endif
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    public enum RemoteValueType : byte
    {
        StringValue,
        IntValue,
        FloatValue,
        BoolValue,
        JsonValue
    }

    [Serializable]
    public class RemoteConfigurationService : GameService, IRemoteConfigurationService
    {
        private readonly RemoteConfiguration       _configuration;
        private readonly IContext                  _context;
        private readonly bool                      _isDebug;
        private readonly List<IRemoteValueHandler> _handlers;
        
        private RemoteTimingConfiguration                       _timeConfiguration;
        private int                                             _cacheExpiration;
        private ReactiveDictionary<RemoteConfigId, RemoteValue> _values = new ReactiveDictionary<RemoteConfigId, RemoteValue>();

        public RemoteConfigurationService(RemoteConfiguration configuration, IContext context, bool isDebug = false)
        {
            _configuration = configuration;
            _context       = context;
            _isDebug       = isDebug;
            _handlers      = _configuration.handlers;
            
            Complete();
            InitializeRemote().Forget();
        }

        public bool IsInitialized { get; private set; }

        public bool IsDebug => _isDebug;

        public void Update()
        {
            UpdateAsync()
                .AttachExternalCancellation(LifeTime.TokenSource)
                .Forget();
        }

        public async UniTask UpdateAsync()
        {
            if (!IsInitialized) return;
            
            await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromMilliseconds(_cacheExpiration));
            
            var isComplete = await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                   
            await UniTask.SwitchToMainThread();
                            
            if(!isComplete) return;

            var values = FirebaseRemoteConfig.DefaultInstance.AllValues;

            await ApplyRemoteValues(values, _context);
        }

        public RemoteValue GetValue(RemoteConfigId id)
        {
            _values.TryGetValue(id, out var value);
            return value;
        }

        public async UniTask InitializeRemote()
        {
            var isInitialized = false;

            InitializeConfiguration();
            
            await InitializeHandlers();
            
            await FirebaseRemoteConfig.DefaultInstance
                .FetchAndActivateAsync()
                .AsUniTask();

            await ApplyRemoteTimeConfiguration(_timeConfiguration);

            IsInitialized = true;
            
            await UniTask.SwitchToMainThread();

            StartRemoteHandleUpdate().Forget();
        }

        public async UniTask StartRemoteHandleUpdate()
        {
            var delay = _timeConfiguration.fetchTimeoutInMilliseconds;
            do
            {
                await UniTask.Delay(delay);

                await UpdateAsync();
            } while (delay > 0 && LifeTime.IsTerminated == false);
        }

        /// <summary>
        /// setup default remote configuration data
        /// </summary>
        /// <param name="timeConfiguration"></param>
        /// <returns></returns>
        public async UniTask ApplyRemoteTimeConfiguration(RemoteTimingConfiguration timeConfiguration)
        {
            var settings = FirebaseRemoteConfig.DefaultInstance.ConfigSettings;
            settings.FetchTimeoutInMilliseconds         = (ulong) timeConfiguration.fetchTimeoutInMilliseconds;
            settings.MinimumFetchInternalInMilliseconds = (ulong) timeConfiguration.minimumFetchInternalInMilliseconds;
            
            _cacheExpiration = timeConfiguration.defaultCacheExpirationMilliseconds;
            
            await FirebaseRemoteConfig.DefaultInstance
                .SetConfigSettingsAsync(settings)
                .AsUniTask();
        }

        private void InitializeConfiguration()
        {
            _timeConfiguration = _isDebug
                ? _configuration.debugTimeConfiguration
                : _configuration.timeConfiguration;
        }

        private async UniTask InitializeHandlers()
        {
            var values = _configuration.values;
            foreach (var remoteValue in values)
            {
                await SetRemoteValue(remoteValue.Key, remoteValue.Value);
            }
        }

        private async UniTask ApplyRemoteValues(IDictionary<string,ConfigValue> values,IContext context)
        {
            var defaultValues = _configuration.values;
            foreach (var item in defaultValues)
            {
                var id           = item.Key;
                var defaultValue = item.Value;

                if(!values.TryGetValue(id,out var value))
                    continue;

                var valueType = defaultValue.ValueType;
                if(value.Source == ValueSource.DefaultValue)
                    continue;

                var remoteValue = CreateValue(value, valueType);
                await SetRemoteValue(id, remoteValue);
            }
            
        }

        private RemoteValue CreateValue(ConfigValue value, RemoteValueType valueType)
        {
            var result = valueType switch
            {
                RemoteValueType.BoolValue => RemoteValue.CreateValue(value.BooleanValue,valueType),
                RemoteValueType.StringValue => RemoteValue.CreateValue(value.StringValue,valueType),
                RemoteValueType.IntValue => RemoteValue.CreateValue(value.LongValue,valueType),
                RemoteValueType.FloatValue => RemoteValue.CreateValue(value.DoubleValue,valueType),
                RemoteValueType.JsonValue => RemoteValue.CreateValue(value.StringValue,valueType),
                _ => new RemoteValue(),
            };

            return result;
        }
        
        private async UniTask SetRemoteValue(RemoteConfigId id, RemoteValue remoteValue)
        {
            _values[id] = remoteValue;
            foreach (var handler in _handlers)
            {
                if(!handler.CanHandle(id,remoteValue))
                    continue;
                await handler.UpdateValue(id,remoteValue, _context)
                    .AttachExternalCancellation(LifeTime.TokenSource);
            }
        }
        
    }
    
}