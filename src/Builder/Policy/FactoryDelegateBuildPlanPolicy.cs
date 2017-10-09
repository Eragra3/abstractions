﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Policy;

namespace Unity.Builder.Policy
{
    internal class FactoryDelegateBuildPlanPolicy : IBuildPlanPolicy
    {
        private readonly Func<IUnityContainer, Type, string, object> factory;

        public FactoryDelegateBuildPlanPolicy(Func<IUnityContainer, Type, string, object> factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Creates an instance of this build plan's type, or fills
        /// in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
        public void BuildUp(IBuilderContext context)
        {
            if ((context ?? throw new ArgumentNullException(nameof(context))).Existing == null)
            {
                var currentContainer = context.NewBuildUp<IUnityContainer>();
                context.Existing = factory(currentContainer, context.BuildKey.Type, context.BuildKey.Name);
                context.SetPerBuildSingleton();
            }
        }
    }
}
