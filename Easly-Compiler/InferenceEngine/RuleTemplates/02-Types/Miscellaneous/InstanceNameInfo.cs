namespace EaslyCompiler
{
    using CompilerNode;

    internal class InstanceNameInfo
    {
        public InstanceNameInfo(AncestorFeatureInfo item, IFeatureInstance instance, IFeatureName name)
        {
            Instance = instance;
            Name = name;
            Ancestor = item.Ancestor;
            if (item.Location.IsAssigned)
                Location = item.Location.Item;
            else
                Location = new Inheritance();
            SameIsKept = true;
            SameIsDiscontinued = true;
        }

        public IClassType Ancestor { get; set; }
        public IFeatureInstance Instance { get; set; }
        public IFeatureName Name { get; set; }
        public IInheritance Location { get; set; }
        public bool SameIsKept { get; set; }
        public bool SameIsDiscontinued { get; set; }
    }
}
