public static class UInt32Extensions
{
    public static uint Insert(this uint a, uint b, byte width)
    {
        a = a << width;
        uint mask = 0b0;
        for (int i = 0; i < width; i++)
        {
            mask = mask << 1;
            mask += 1;
        }
        return a | (b & mask);
    }
}
