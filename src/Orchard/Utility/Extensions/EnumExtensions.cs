using Orchard.Reflection.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Orchard
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the textual description of the enum if it has one. e.g.
        /// 
        /// <code>
        /// enum UserColors
        /// {
        ///     [Description("Bright Red")]
        ///     BrightRed
        /// }
        /// UserColors.BrightRed.ToDescription();
        /// </code>
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
#if !(NETFX_CORE)
        public static string ToDescription(this Enum @enum)
        {
            var type = @enum.GetType();

            var memInfo = type.GetMember(@enum.ToString());

            if (memInfo.Length > 0)
            {
                var description = memInfo[0].GetDescription();

                if (description != null)
                    return description;
            }

            return @enum.ToString();
        }
#endif

        public static List<string> ToList(this Enum @enum)
        {
#if !(SL54 || WP)
            return new List<string>(Enum.GetNames(@enum.GetType()));
#else
            return @enum.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static TypeCode GetTypeCode(this Enum @enum)
        {
            return Enum.GetUnderlyingType(@enum.GetType()).GetTypeCode();
        }

        public static bool Has<T>(this Enum @enum, T value)
        {
            var typeCode = @enum.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Byte:
                    return (((byte)(object)@enum & (byte)(object)value) == (byte)(object)value);
                case TypeCode.Int16:
                    return (((short)(object)@enum & (short)(object)value) == (short)(object)value);
                case TypeCode.Int32:
                    return (((int)(object)@enum & (int)(object)value) == (int)(object)value);
                case TypeCode.Int64:
                    return (((long)(object)@enum & (long)(object)value) == (long)(object)value);
                default:
                    throw new NotSupportedException($"Enums of type {@enum.GetType().Name}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Is<T>(this Enum @enum, T value)
        {
            var typeCode = @enum.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Byte:
                    return (byte)(object)@enum == (byte)(object)value;
                case TypeCode.Int16:
                    return (short)(object)@enum == (short)(object)value;
                case TypeCode.Int32:
                    return (int)(object)@enum == (int)(object)value;
                case TypeCode.Int64:
                    return (long)(object)@enum == (long)(object)value;
                default:
                    throw new NotSupportedException($"Enums of type {@enum.GetType().Name}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Add<T>(this Enum @enum, T value)
        {
            var typeCode = @enum.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Byte:
                    return (T)(object)((byte)(object)@enum | (byte)(object)value);
                case TypeCode.Int16:
                    return (T)(object)((short)(object)@enum | (short)(object)value);
                case TypeCode.Int32:
                    return (T)(object)((int)(object)@enum | (int)(object)value);
                case TypeCode.Int64:
                    return (T)(object)((long)(object)@enum | (long)(object)value);
                default:
                    throw new NotSupportedException($"Enums of type {@enum.GetType().Name}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Remove<T>(this Enum @enum, T value)
        {
            var typeCode = @enum.GetTypeCode();
            switch (typeCode)
            {
                case TypeCode.Byte:
                    return (T)(object)((byte)(object)@enum & ~(byte)(object)value);
                case TypeCode.Int16:
                    return (T)(object)((short)(object)@enum & ~(short)(object)value);
                case TypeCode.Int32:
                    return (T)(object)((int)(object)@enum & ~(int)(object)value);
                case TypeCode.Int64:
                    return (T)(object)((long)(object)@enum & ~(long)(object)value);
                default:
                    throw new NotSupportedException($"Enums of type {@enum.GetType().Name}");
            }
        }
        /// <summary>
        /// Will try and parse an enum and it's default type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns>True if the enum value is defined.</returns>
        public static bool TryEnumIsDefined(Type type, object value)
        {
            if (type == null || value == null || !type.GetTypeInfo().IsEnum)
                return false;

            // Return true if the value is an enum and is a matching type.
            if (type == value.GetType())
                return true;

            if (TryEnumIsDefined<int>(type, value))
                return true;
            if (TryEnumIsDefined<string>(type, value))
                return true;
            if (TryEnumIsDefined<byte>(type, value))
                return true;
            if (TryEnumIsDefined<short>(type, value))
                return true;
            if (TryEnumIsDefined<long>(type, value))
                return true;
            if (TryEnumIsDefined<sbyte>(type, value))
                return true;
            if (TryEnumIsDefined<ushort>(type, value))
                return true;
            if (TryEnumIsDefined<uint>(type, value))
                return true;
            if (TryEnumIsDefined<ulong>(type, value))
                return true;

            return false;
        }

        public static bool TryEnumIsDefined<T>(Type type, object value)
        {
            // Catch any casting errors that can occur or if 0 is not defined as a default value.
            try
            {
                if (value is T && Enum.IsDefined(type, (T)value))
                    return true;
            }
            catch (Exception) { }

            return false;
        }

    }

}