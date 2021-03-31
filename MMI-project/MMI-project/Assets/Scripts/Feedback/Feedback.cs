using UnityEngine;

namespace Feedback
{
    public abstract class Feedback
    {
        protected GameObject parent;
        float lastPlayTime = 0;
        public Feedback(GameObject parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Called when you want to give the user feedback
        /// </summary>
        /// <param name="interval">Interval between feedback in MS</param>
        public void GiveFeedback(int interval)
        {
            float intervalMilliseconds = interval / 1000f;

            if (lastPlayTime + intervalMilliseconds <= Time.fixedTime && FeedbackImplementation())
            {
                lastPlayTime = Time.fixedTime;
            }
        }

        protected abstract bool FeedbackImplementation();

        public abstract void Success();
    }
}