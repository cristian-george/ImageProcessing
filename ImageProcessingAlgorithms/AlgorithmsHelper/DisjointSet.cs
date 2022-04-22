using System.Collections.Generic;

namespace ImageProcessingAlgorithms.AlgorithmsHelper
{
    internal class DisjointSet<T>
    {
        private readonly Dictionary<T, T> m_parent;
        private readonly Dictionary<T, int> m_rank;

        public DisjointSet()
        {
            m_parent = new Dictionary<T, T>();
            m_rank = new Dictionary<T, int>();
        }

        public void MakeSet(in T element)
        {
            m_parent[element] = element;
            m_rank[element] = 0;
        }

        public T FindSet(in T element)
        {
            if (m_parent[element].Equals(element) == false)
                m_parent[element] = FindSet(m_parent[element]);

            return m_parent[element];
        }

        public void UnionSets(in T firstSet, in T secondSet)
        {
            T parentFirstSet = FindSet(firstSet);
            T parentSecondSet = FindSet(secondSet);

            if (parentFirstSet.Equals(parentSecondSet))
            {
                return;
            }

            if (m_rank[parentFirstSet] < m_rank[parentSecondSet])
            {
                Helper.Swap(ref parentFirstSet, ref parentSecondSet);
            }

            m_parent[parentSecondSet] = parentFirstSet;
            if (m_rank[parentFirstSet] == m_rank[parentSecondSet])
                ++m_rank[parentFirstSet];
        }
    }
}
