// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureResults.cs" company="NBehave">
//   Copyright (c) 2007, NBehave - http://nbehave.codeplex.com/license
// </copyright>
// <summary>
//   Defines the FeatureResults type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using NBehave.Narrator.Framework.Tiny;

namespace NBehave.Narrator.Framework
{
    [Serializable]
    public class FeatureResultEvent : GenericTinyMessage<FeatureResult>
    {

        public FeatureResultEvent(object sender, FeatureResult content) : base(sender, content)
        { }
    }
}