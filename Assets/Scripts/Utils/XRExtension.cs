using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class XRExtension
{
  public static bool IsGreaterThan(this Vector3 a, Vector3 b) {
    return (a.sqrMagnitude > b.sqrMagnitude);
  }

  public static bool IsLessThan(this Vector3 a, Vector3 b) {
    return (a.sqrMagnitude < b.sqrMagnitude);
  }
}
