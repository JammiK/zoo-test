using System.Collections.Generic;

namespace Core.JG.Extensions
{
    public static class XorRandomHelper
    {
        public static List<T> TakeRandomElementsFromList<T>(XorRandom random, int requiredCount, IList<T> sourceList)
        {
            List<T> resultList = new List<T>(requiredCount);

            if (sourceList == null || sourceList.Count == 0 || requiredCount == 0)
                return resultList;
        
            while (resultList.Count < requiredCount)
                resultList.AddRange(sourceList);
        
            random.ShuffleList(resultList);
            if(resultList.Count > requiredCount)
                resultList.RemoveRange(requiredCount, resultList.Count - requiredCount);
            
            return resultList;
        }
    }
}