// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepConverterExtensions.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepConverterExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NBehave.Configuration;
using NBehave.Internal;

namespace NBehave.Extensions
{
    public static class NBehaveConfigurationExtensions
    {
        public static IRunner Build(this NBehaveConfiguration configuration)
        {
            return RunnerFactory.CreateTextRunner(configuration);
        }
    }
}
