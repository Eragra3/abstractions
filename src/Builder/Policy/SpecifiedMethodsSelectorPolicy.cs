﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Builder.Selection;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Builder.Policy
{
    /// <summary>
    /// A <see cref="IMethodSelectorPolicy"/> implementation that calls the specific
    /// methods with the given parameters.
    /// </summary>
    public class SpecifiedMethodsSelectorPolicy : IMethodSelectorPolicy
    {
        private readonly List<Tuple<MethodInfo, IEnumerable<InjectionParameterValue>>> _methods =
            new List<Tuple<MethodInfo, IEnumerable<InjectionParameterValue>>>();

        /// <summary>
        /// Add the given method and parameter collection to the list of methods
        /// that will be returned when the selector's <see cref="IMethodSelectorPolicy.SelectMethods"/>
        /// method is called.
        /// </summary>
        /// <param name="method">Method to call.</param>
        /// <param name="parameters">sequence of <see cref="InjectionParameterValue"/> objects
        /// that describe how to create the method parameter values.</param>
        public void AddMethodAndParameters(MethodInfo method, IEnumerable<InjectionParameterValue> parameters)
        {
            _methods.Add(new Tuple<MethodInfo, IEnumerable<InjectionParameterValue>>(method, parameters));
        }

        /// <summary>
        /// Return the sequence of methods to call while building the target object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>Sequence of methods to call.</returns>
        public IEnumerable<SelectedMethod> SelectMethods(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            foreach (Tuple<MethodInfo, IEnumerable<InjectionParameterValue>> method in _methods)
            {
                Type typeToBuild = context.BuildKey.Type;
                SelectedMethod selectedMethod;
                ReflectionHelper typeReflector = new ReflectionHelper(method.Item1.DeclaringType);
                MethodReflectionHelper methodReflector = new MethodReflectionHelper(method.Item1);
                if (!methodReflector.MethodHasOpenGenericParameters && !typeReflector.IsOpenGeneric)
                {
                    selectedMethod = new SelectedMethod(method.Item1);
                }
                else
                {
                    Type[] closedMethodParameterTypes =
                        methodReflector.GetClosedParameterTypes(typeToBuild.GetTypeInfo().GenericTypeArguments);
                    selectedMethod = new SelectedMethod(
                        typeToBuild.GetMethodHierarchical(method.Item1.Name, closedMethodParameterTypes));
                }

                SpecifiedMemberSelectorHelper.AddParameterResolvers(
                        typeToBuild,
                        resolverPolicyDestination,
                        method.Item2,
                        selectedMethod);
                yield return selectedMethod;
            }
        }
    }
}
