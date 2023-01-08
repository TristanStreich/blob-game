
public interface ISpawnChunk
{
    int Seed
    {
        get;
        set;
    }

    void init();

    void initWithSeed(int seed);
}
