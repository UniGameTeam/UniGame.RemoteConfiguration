namespace Game.Modules.Assets.UniGame.RemoteConfiguration.Runtime
{
    using System;
    using global::UniGame.RemoteConfiguration.Runtime;
    using Sirenix.OdinInspector;
    using UniModules.UniCore.Runtime.Utils;

    [Serializable]
#if ODIN_INSPECTOR
    [InlineProperty]
    [HideLabel]
#endif
    public struct RemoteValue
    {
        #region static data

        public static RemoteValue CreateValue(object value, RemoteValueType type)
        {
            var remoteValue = new RemoteValue();
            return remoteValue.SetValue(value,type);
        }

        #endregion
        
        public const string ValueLabel = "value:";

        public RemoteValueType valueType;

#if ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
        [LabelText(ValueLabel)]
        [ShowIf(nameof(IsNumber))]
#endif
        public int numberValue;

#if ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
        [LabelText(ValueLabel)]
        [ShowIf(nameof(IsFloat))]
#endif
        public float floatValue;
        
#if ODIN_INSPECTOR
        [HideLabel]
        [LabelText(ValueLabel)]
        [InlineProperty]
        [ShowIf(nameof(IsBool))]
#endif
        public bool boolValue;

#if ODIN_INSPECTOR
        [HideLabel]
        [LabelText(ValueLabel)]
        [InlineProperty]
        [ShowIf(nameof(IsString))]
#endif
        public string stringValue;

        public bool IsString => valueType == RemoteValueType.StringValue;
        public bool IsNumber => valueType == RemoteValueType.IntValue;
        public bool IsBool   => valueType == RemoteValueType.BoolValue;
        public bool IsFloat   => valueType == RemoteValueType.FloatValue;
        public bool IsJson   => valueType == RemoteValueType.JsonValue;

        public RemoteValueType ValueType => valueType;
        
        public RemoteValue SetValue(object newValue,RemoteValueType type = RemoteValueType.StringValue)
        {
            if (newValue == null) return this;
            
            valueType = type;
            
            switch (valueType)
            {
                case RemoteValueType.StringValue:
                    stringValue = newValue as string;
                    return this;
                case RemoteValueType.IntValue:
                    numberValue = (int)newValue; 
                    return this;
                case RemoteValueType.FloatValue:
                    floatValue = (float)newValue; 
                    return this;
                case RemoteValueType.BoolValue:
                    boolValue = (bool)newValue; 
                    return this;
                case RemoteValueType.JsonValue:
                    stringValue = newValue as string;
                    return this;
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
            }
            
            return this;
        }
        
        public override string ToString()
        {
            var result = valueType switch
            {
                RemoteValueType.BoolValue => boolValue.ToString(),
                RemoteValueType.StringValue => stringValue,
                RemoteValueType.IntValue => numberValue.ToStringFromCache(),
                RemoteValueType.FloatValue => floatValue.ToStringFromCache(),
                RemoteValueType.JsonValue => stringValue,
                _ => string.Empty,
            };

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is RemoteValue value)
            {
                return GetHashCode() == value.GetHashCode();
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var code = valueType switch
            {
                RemoteValueType.BoolValue => boolValue.GetHashCode(),
                RemoteValueType.StringValue => stringValue.GetHashCode(),
                RemoteValueType.IntValue => numberValue.GetHashCode(),
                RemoteValueType.JsonValue => stringValue.GetHashCode(),
                RemoteValueType.FloatValue => floatValue.GetHashCode(),
                _ => 0,
            };
            return code;
        }
    }
}