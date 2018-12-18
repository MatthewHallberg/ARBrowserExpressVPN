using UnityEngine.XR.ARExtensions;

namespace UnityEngine.XR.ARCore
{
    internal class ARCorePromise<T> : Promise<T>
    {
        protected override void OnKeepWaiting()
        {
            if (s_LastFrameUpdated == Time.frameCount)
                return;

            Api.ArPresto_update();
            s_LastFrameUpdated = Time.frameCount;
        }

        internal new void Resolve(T result)
        {
            base.Resolve(result);
        }

        static int s_LastFrameUpdated;
    }
}
