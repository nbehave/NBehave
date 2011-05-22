using System.Collections.Generic;

namespace NBehave.Narrator.Framework.Messages
{
    using NBehave.Narrator.Framework.Tiny;

    public class FeaturesLoaded : GenericTinyMessage<IEnumerable<Feature>>
    {
        public FeaturesLoaded(object sender, IEnumerable<Feature> content)
            : base(sender, content)
        {
        }
    }
}
