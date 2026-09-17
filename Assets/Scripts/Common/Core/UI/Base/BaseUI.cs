using System;
using UnityEngine;

namespace Assets.Scripts.Common.UI.Base
{
    public class BaseUI<T> : BaseUIAbstract where T : Enum
    {
        [SerializeField] private T key;

        public T KeyEnum => key;
        public override Enum Key => key;
    }
}
