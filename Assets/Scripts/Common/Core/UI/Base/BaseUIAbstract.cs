using System;
using UnityEngine;
namespace Assets.Scripts.Common.UI.Base
{
    public abstract class BaseUIAbstract : MonoBehaviour
    {
        public abstract Enum Key { get; }
    }
}