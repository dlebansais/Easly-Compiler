namespace Test
{
    public interface ICoverageOverLoopSource : IOverLoopSource<IItemType, ItemType>
    {
    }

    [System.Serializable]
    public class CoverageOverLoopSource : OverLoopSource<IItemType, ItemType>, ICoverageOverLoopSource
    {
    }
}
