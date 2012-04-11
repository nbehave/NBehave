// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepConverterExtensions.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepConverterExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using NBehave.Narrator.Framework.Internal;

namespace NBehave.Narrator.Framework.Extensions
{
    public static class NBehaveConfigurationExtensions
    {
        public static IRunner Build(this NBehaveConfiguration configuration)
        {
            return RunnerFactory.CreateTextRunner(configuration);
        }
    }
}
