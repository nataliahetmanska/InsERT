using System.Linq;
using System;
static class Utils
{
    public static int Levenstein(string src, string tgt)
    {
        if (src == null || tgt == null) 
            return 0;
        if (src.Length == 0 || tgt.Length == 0) 
            return 0;
        if (src == tgt) 
            return src.Length;

        int srcWords = src.Length;
        int tgtWords = tgt.Length;

        if (srcWords == 0)
            return tgtWords;

        if (tgtWords == 0)
            return srcWords;

        int[,] distance = new int[srcWords + 1, tgtWords + 1];

        for (int i = 0; i <= srcWords; i++)
            distance[i, 0] = i;
        for (int j = 0; j <= tgtWords; j++)
            distance[0, j] = j;

        for (int i = 1; i <= srcWords; i++)
            for (int j = 1; j <= tgtWords; j++)
                distance[i, j] = Math.Min(
                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), 
                    distance[i - 1, j - 1] + ((src[i - 1] == tgt[j - 1]) ? 0 : 1)
                    );

        return distance[srcWords, tgtWords];
    }

    public static double Similarity(string src, string tgt)
    {
        if (src == null || tgt == null) 
            return 0.0;
        if (src.Length == 0 || tgt.Length == 0) 
            return 0.0;
        if (src == tgt) 
            return 1.0;

        return (1.0 - ((double) Levenstein(src, tgt) / (double) Math.Max(src.Length, tgt.Length)));
    }
}