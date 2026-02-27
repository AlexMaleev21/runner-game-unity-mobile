using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;

using UnityEditor;
using UnityEngine.Rendering;

namespace UnityEngine.AdaptivePerformance.Tests
{
    class AdaptivePerformanceRuntimeTests
    { 
        [UnityTest]
        public IEnumerator DummyAPRuntimeTests()
        {
			yield return null;
			Assert.AreEqual(1, 1);
        }
    }
}
