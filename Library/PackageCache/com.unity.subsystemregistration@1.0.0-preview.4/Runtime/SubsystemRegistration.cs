using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental;

namespace UnityEngine
{
    public static class SubsystemRegistration
    {
        static readonly List<SubsystemDescriptor> k_SubsystemDescriptors = new List<SubsystemDescriptor>();

        /// <summary>
        /// Registers a <c>SubsystemDescriptor</c> with the Subsystem Manager so that features and implementation type are available.
        /// </summary>
        /// <param name="descriptor"> The <c>SubsystemDescriptor</c> that describes the subsystem implementation.</param>
        /// <returns><c>True</c> if the descriptor does not already exist and registration happens, otherwise, <c>False</c>.</returns>
        public static bool CreateDescriptor(SubsystemDescriptor descriptor)
        {
			
            foreach (var declaration in k_SubsystemDescriptors)
            {
                if (descriptor.subsystemImplementationType == declaration.subsystemImplementationType)
                    return false;
            }

            Internal_SubsystemDescriptors.Internal_AddDescriptor(descriptor);
            k_SubsystemDescriptors.Add(descriptor);

            return true;
        }
    }
 }
