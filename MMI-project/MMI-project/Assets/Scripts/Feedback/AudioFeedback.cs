using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Feedback
{
    class AudioFeedback : Feedback
    {
        readonly AudioSource audioSource;
        public static AudioClip tickSound;
        public static AudioClip successSound;

        public AudioFeedback(GameObject parent) : base(parent)
        {
            audioSource = parent.GetComponent<AudioSource>();
        }

        public override void Success()
        {
            audioSource.Stop();
            audioSource.PlayOneShot(successSound);
        }

        protected override bool FeedbackImplementation()
        {
            audioSource.Stop();
            audioSource.PlayOneShot(tickSound);
            return true;
        }
        
    }
}
