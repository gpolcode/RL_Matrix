namespace RLMatrix.Agents.Common
{
    public class Episode<T>
    {
        Guid? guidCache;
        private List<TransitionPortable<T>> TempBuffer = new();
        public List<TransitionPortable<T>> CompletedEpisodes = new();

        public void AddTransition(T state, bool isDone, int[] discreteActions, float[] continuousActions = null, float reward = 1f)
        {
            Guid? nextGuid = null;
            if (guidCache == null)
            {
                guidCache = Guid.NewGuid();
            }
            if (!isDone)
            {
                nextGuid = Guid.NewGuid();
            }

            continuousActions ??= new float[0];

            var transition = new TransitionPortable<T>((Guid)guidCache, state, discreteActions, continuousActions, reward, nextGuid);
            TempBuffer.Add(transition);
            guidCache = nextGuid;
            if (isDone)
            {
                CompletedEpisodes.AddRange(TempBuffer);
                TempBuffer.Clear();
            }
        }
    }
}