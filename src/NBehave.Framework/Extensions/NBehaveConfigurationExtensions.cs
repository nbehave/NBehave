// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionStepConverterExtensions.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the ActionStepConverterExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NBehave.Narrator.Framework
{
    public static class NBehaveConfigurationExtensions
    {
        public static TextRunner Build(this NBehaveConfiguration configuration)
        {
            return new TextRunner(configuration);
        }

        public static FeatureResults Run(this NBehaveConfiguration configuration)
        {
            return configuration.Build().Run();
        }
    }
}
