namespace EaslyCompiler
{
    using System.Collections.Generic;
    using Easly;

    internal class InheritedInstanceInfo
    {
        public InheritedInstanceInfo()
        {
            EffectiveInstance = new OnceReference<InstanceNameInfo>();
            PrecursorInstanceList = new List<InstanceNameInfo>();
            IsKept = false;
            IsDiscontinued = false;
        }

        public OnceReference<InstanceNameInfo> EffectiveInstance { get; set; }
        public IList<InstanceNameInfo> PrecursorInstanceList { get; set; }
        public bool IsKept { get; set; }
        public bool IsDiscontinued { get; set; }
    }
}
