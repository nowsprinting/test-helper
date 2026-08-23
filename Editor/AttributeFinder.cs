// Copyright (c) 2023-2026 Koji Hasegawa.
// This software is released under the MIT License.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace TestHelper.Editor
{
    /// <summary>
    /// Find attribute instances attached to symbols in the current AppDomain.
    /// Also finds attributes derived from <c>T</c> (e.g., <c>LoadSceneAttribute</c> via <c>BuildSceneAttribute</c>).
    /// </summary>
    internal static class AttributeFinder
    {
        internal static IEnumerable<T> FindOnFields<T>() where T : Attribute
        {
#if UNITY_2020_1_OR_NEWER
            return FindOnProviders<T>(TypeCache.GetFieldsWithAttribute<T>());
#else
            return FindOnProviders<T>(FindAllFields());
#endif
        }

#if !UNITY_2020_1_OR_NEWER
        /// <summary>
        /// TypeCache.GetFieldsWithAttribute is not available on Unity 2019; fall back to reflection over
        /// all loaded assemblies.
        /// </summary>
        private static IEnumerable<ICustomAttributeProvider> FindAllFields()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var field in type.GetFields(
                                 BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Instance | BindingFlags.Static))
                    {
                        yield return field;
                    }
                }
            }
        }
#endif

        internal static IEnumerable<T> FindOnAssemblies<T>() where T : Attribute
        {
            return FindOnProviders<T>(AppDomain.CurrentDomain.GetAssemblies());
        }

        internal static IEnumerable<T> FindOnTypes<T>() where T : Attribute
        {
            return FindOnProviders<T>(TypeCache.GetTypesWithAttribute<T>());
        }

        internal static IEnumerable<T> FindOnMethods<T>() where T : Attribute
        {
            return FindOnProviders<T>(TypeCache.GetMethodsWithAttribute<T>());
        }

        private static IEnumerable<T> FindOnProviders<T>(IEnumerable<ICustomAttributeProvider> providers)
            where T : Attribute
        {
            foreach (var provider in providers)
            {
                foreach (var attribute in provider.GetCustomAttributes(typeof(T), false))
                {
                    yield return (T)attribute;
                }
            }
        }
    }
}
