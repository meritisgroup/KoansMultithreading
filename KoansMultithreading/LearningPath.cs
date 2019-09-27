using KoansBase.Engine;
using System;

namespace KoansMultithreading
{
    public class LearningPath : Path
    {
        public LearningPath()
        {
            Types = new Type[] {
                typeof(AboutThread),
                typeof(AboutTask),
                typeof(AboutThreadSynchronization)
            };
        }
    }
}